using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ_Caller;
using System.Net.Security;

namespace RabbitMQ_SSL_Test
{
    static class Program
    {
        static readonly SslPolicyErrors[] sslRules =
                {
                    SslPolicyErrors.None, SslPolicyErrors.RemoteCertificateNameMismatch, SslPolicyErrors.RemoteCertificateChainErrors, SslPolicyErrors.RemoteCertificateNotAvailable,
                    SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateChainErrors | SslPolicyErrors.RemoteCertificateNotAvailable,
                };

        static void Main(string[] args)
        {
            Console.WriteLine("Will attempt to send two simple text messages to the \"SSL Test Q\" queue.");
            if (args.Length > 0)
                Rmq.serverName = args[0];
            else
            {
                Console.WriteLine("Using \"{0}\" as a destination RabbitMQ hostname. Specify another as a command line argument, if necessary.\r\n", Rmq.serverName);
            }

            try
            {
                SendInClear();
                sslRules.Any(SendWithSsl);
            }
            finally
            {
                if (Environment.UserInteractive)
                {
                    Console.Write("\r\nPress any key to exit.");
                    Console.ReadKey();
                }
            }
        }

        private static bool SendWithSsl(SslPolicyErrors allowedSslRule)
        {
            Console.Write("Sending a message using SSL (5671) endpoint with allowed error type: \"{0}\"... ", allowedSslRule);

            try
            {
                string message = string.Format("Message sent via SSL with allowed error type: \"{0}\".",
                    allowedSslRule);
                Rmq.Send(message, useSsl: true, acceptableSslErrors: allowedSslRule);
                Console.WriteLine("Success!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed due to \"{0}\".", ex.GetExceptionChain().Last().Message);
            }
            Console.WriteLine();
            return false;
        }

        private static void SendInClear()
        {
            Console.Write("Sending a message using regular, non-SSL endpoint... ");
            Rmq.Send("No SSL - regular endpoint.", useSsl: false);
            Console.WriteLine("Success!\r\n");
        }

        public static List<Exception> GetExceptionChain(this Exception topException)
        {
            var exList = new List<Exception>();
            Exception prev = null;
            for (var ex = topException; ex != null && ex != prev; ex = ex.GetBaseException())
            {
                exList.Add(ex);
                prev = ex;
            }
            return exList;
        }
    }
}
