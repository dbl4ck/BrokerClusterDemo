using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Apache.NMS;
using Apache.NMS.Util;
using Apache.NMS.ActiveMQ;

namespace BrokerClusterDemo.Consumer
{
    class Program
    {
        private static string window_title = "Consumer";
        private static double loop_time = 10;
        private static string url = "failover:(tcp://localhost:61616,tcp://localhost:61617,tcp://localhost:61618)?randomize=false";
        private static string queueName = "queue://queue1";

        static void Main(string[] args)
        {
            Console.Title = window_title;

            IConnectionFactory factory = new ConnectionFactory(url);
            using (IConnection connection = factory.CreateConnection())
            using (ISession session = connection.CreateSession())
            {
                var queue = SessionUtil.GetQueue(session, queueName);
                var destination = SessionUtil.GetDestination(session, queueName, DestinationType.Queue);
                var consumer = session.CreateConsumer(destination);
                
                consumer.Listener += OnReceivedListener;

                connection.Start();

                // main loop
                while (true)
                {
                    Thread.Sleep((int)loop_time);
                }
            }
        }

        private static void OnReceivedListener(IMessage message)
        {
            if (message == null)
                return;

            if ((message as ITextMessage) == null)
                return;

            Console.WriteLine(((ITextMessage)message).Text);
        }
    }
}
