using System;
using EventStore.ClientAPI.Embedded;
using EventStore.Core;
using EventStore.Common.Log;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace Node
{
    public class NodeRunner
    {
        readonly ClusterVNode node;

        public NodeRunner(int nodeNumber)
        {
            string BaseDir() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"node{nodeNumber}");
            string LogDir() => Path.Combine(BaseDir(), "logs");
            string DataDir() => Path.Combine(BaseDir(), "data");

            LogManager.Init("EventStoreNode", LogDir(), BaseDir());

            var ports = FreePorts();

            var nodeBuilder = EmbeddedVNodeBuilder.AsSingleNode()
                                                  .DoNotVerifyDbHashes()
                                                  .WithExternalTcpOn(new IPEndPoint(IPAddress.Loopback, nodeNumber + 1000))
                                                  .WithExternalHttpOn(new IPEndPoint(IPAddress.Loopback, nodeNumber + 2000))
                                                  .
                                                  .RunOnDisk(DataDir());

            node = nodeBuilder.Build();
        }

        public void Start() { node.StartAndWaitUntilReady().Wait(); }
        public void Stop() { node.Stop(); }

        (IPEndPoint tcpPort, IPEndPoint httpPort) FreePorts()
        {
            int FreeTcpPort()
            {
                TcpListener l = new TcpListener(IPAddress.Loopback, 0);
                l.Start();
                int port = ((IPEndPoint)l.LocalEndpoint).Port;
                l.Stop();
                return port;
            }

            IPEndPoint LoopBackOnFreePort() => new IPEndPoint(IPAddress.Loopback, FreeTcpPort());

            return (LoopBackOnFreePort(), LoopBackOnFreePort());
        }
    }
}
