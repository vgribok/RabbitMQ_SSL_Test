using System;
using System.Net.Security;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQ_Caller
{
    public class Rmq
    {
        public static string serverName = Environment.MachineName;

        public static void Send(string messageText, string username, string password, bool useSsl, SslPolicyErrors acceptableSslErrors = SslPolicyErrors.None)
        {
            messageText += string.Format(" {0}", DateTime.Now);

            var factory = new ConnectionFactory {HostName = serverName, Ssl = {Enabled = useSsl}, UserName = username, Password = password };

            if (useSsl)
            {
                factory.Ssl.ServerName = serverName;
                factory.Ssl.AcceptablePolicyErrors = acceptableSslErrors;
            }

            using (IConnection connection = factory.CreateConnection(10))
            {
                using (IModel channel = connection.CreateModel())
                {
                    const string queueName = "SSL Test Q";
                    channel.QueueDeclare(queueName, false, false, false, null);

                    byte[] body = Encoding.UTF8.GetBytes(messageText);

                    channel.BasicPublish(string.Empty, queueName, null, body);
                    //Console.WriteLine(" [x] Sent {0}", messageText);
                }
            }
        }
    }
}