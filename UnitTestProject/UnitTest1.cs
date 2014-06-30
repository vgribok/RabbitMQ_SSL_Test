using System;
using System.Net.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RabbitMQ_Caller;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void SendTest()
        {
            //RMQ.Send("Wassup.", useSsl: false);
            Rmq.Send("Wassup with SSL!", username: "guest", password: "guest", useSsl: true, acceptableSslErrors: SslPolicyErrors.RemoteCertificateNameMismatch);
        }
    }
}
