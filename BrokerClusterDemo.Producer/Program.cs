using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Apache.NMS;
using Apache.NMS.Util;
using Apache.NMS.ActiveMQ;

namespace BrokerClusterDemo.Producer
{
    class Program
    {
        private static string window_title = "Producer";
        private static double loop_time = 2000;
        private static string url = "failover:(tcp://localhost:61616,tcp://localhost:61617,tcp://localhost:61618)?randomize=false";
        private static string queueName = "queue://queue1";


        static void Main(string[] args)
        {
            Console.Title = window_title;

            IConnectionFactory factory = new ConnectionFactory(url);
            using (IConnection connection = factory.CreateConnection())
            using (ISession session = connection.CreateSession())
            {
                var destination = SessionUtil.GetDestination(session, queueName, DestinationType.Queue);
                var producer = session.CreateProducer(destination);

                // main loop
                while (true)
                {
                    string text = GenerateMessage();
                    var message = producer.CreateTextMessage(text);

                    producer.Send(message);

                    Console.WriteLine(message.Text);

                    Thread.Sleep((int)loop_time);
                }
            }

            
        }

        private static string GenerateMessage()
        {
            return $"hello {Guid.NewGuid().ToString().Substring(0, 5)}";
        }
    }
}
