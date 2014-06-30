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
            //Rmq.serverName = "www.tfs.ultidev.com";

            //RMQ.Send("Wassup.", useSsl: false);
            Rmq.Send("Wassup with SSL!", useSsl: true, acceptableSslErrors: SslPolicyErrors.RemoteCertificateNameMismatch);
        }
    }
}
