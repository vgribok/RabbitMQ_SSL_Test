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

            string password;
            string username = GetCredentials(out password);

            try
            {
                SendInClear(username, password);
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                sslRules.Any(rule => SendWithSsl(rule, username, password));
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

        private static string GetCredentials(out string password)
        {
            string username;
            Console.Write("Enter RabbitMQ username (blank for \"guest\"):");
            username = Console.ReadLine();
            Console.Write("Enter RabbitMQ password (blank for \"guest\"):");
            password = Console.ReadLine();

            if (string.IsNullOrEmpty(username))
                username = "guest";
            if (string.IsNullOrEmpty(password))
                password = "guest";
            return username;
        }

        private static bool SendWithSsl(SslPolicyErrors allowedSslRule, string username, string password)
        {
            Console.Write("Sending a message using SSL (5671) endpoint with allowed error type: \"{0}\"... ", allowedSslRule);

            try
            {
                string message = string.Format("Message sent via SSL with allowed error type: \"{0}\".",
                    allowedSslRule);
                Rmq.Send(message, username, password, useSsl: true, acceptableSslErrors: allowedSslRule);
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

        private static void SendInClear(string username, string password)
        {
            Console.Write("Sending a message using regular, non-SSL endpoint... ");
            Rmq.Send("No SSL - regular endpoint.", username, password, useSsl: false);
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
