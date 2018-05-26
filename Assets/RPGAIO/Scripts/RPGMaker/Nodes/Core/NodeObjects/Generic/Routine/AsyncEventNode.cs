using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class AsyncEventNode : OptionsNode
    {
        public override string Name
        {
            get { return "Async Event"; }
        }

        public override string Description
        {
            get { return "Run multiple nodes at once"; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return true;
            }
        }

        public override string NextNodeLinkLabel(int index)
        {
            if (index == 0) return "Next";
            
            return index.ToString(CultureInfo.InvariantCulture);
        }

        public override bool IsRoutine
        {
            get { return true; }
        }

        protected override void SetupParameters()
        {
            Add("Mode", PropertyType.StringArray, new[]{"Wait For All","Wait For N","Do Not Wait"}, 0)
                .WithSubParams(
                    SubParam("N:",PropertyType.Int,null,1).If(p => (int)p.Value == 1)
                );
        }

        protected override int Eval(NodeChain nodeChain)
        {
            return 0;
        }

        public override IEnumerator Routine(NodeChain nodeChain)
        {
            var mode = Convert.ToInt32(ValueOf("Mode"));
            var n = mode == 1 ? (int)Parameter("Mode").ValueOf("N:") : -1;

            var eventContainers = new List<EventContainer>();
            var nodes = new List<Node>();
            for (int i = 1; i < NextNodeLinks.Count; i++)
            {
                var nodeLink = NextNodeLinks[i];
                if(!string.IsNullOrEmpty(nodeLink.ID))
                {
                    var foundNode = nodeChain.Nodes.FirstOrDefault(node => node.ID == nodeLink.ID);
                    if(foundNode != null)
                    {
                        nodes.Add(foundNode);
                    }
                }
            }

            foreach(var node in nodes)
            {
                if(node.IsRoutine)
                {
                    var nodeChainToRun = new NodeChain(nodeChain, node);
                    var ec = GetObject.EventHandler.RunNodeChain(nodeChainToRun);
                    eventContainers.Add(ec);
                }
                else
                {
                    node.Evaluate(nodeChain);
                }
            }

            if (mode == 0)
            {
                while (!eventContainers.All(e => e.Done))
                {
                    yield return null;
                }
            }
            else if (mode == 1)
            {
                while (!eventContainers[n].Done)
                {
                    yield return null;
                }
            }
        }
    }
}