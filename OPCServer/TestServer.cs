using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaServer;

namespace OPCServer
{
    internal class OpcServer : ServerManager
    {
        protected override void OnRootNodeManagerStarted(RootNodeManager nodeManager)
        {
            Console.WriteLine("Creating Node Managers.");

            OpcNodeManager opcNode = new OpcNodeManager(this);
            opcNode.Startup();
        }
    }
}
