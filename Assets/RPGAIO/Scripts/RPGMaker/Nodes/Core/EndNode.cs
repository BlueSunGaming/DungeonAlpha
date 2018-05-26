using System.Collections.Generic;
using LogicSpawn.RPGMaker.API;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class EndNode : SimpleNode
    {
        public override string Name
        {
            get { return "EndNode"; }
        }

        public override string Description
        {
            get { return "End of a node chain used as a end point for a node iteration."; }
        }

        public override string SubText
        {
            get { return "Exit point"; }
        }

        public override bool IsStartNode
        {
            get { return false; }
        }

        public override bool ShowTarget
        {
            get { return false; }
        }

        public override bool CanBeDeleted
        {
            get { return true; }
        }

        public override string NextNodeLinkLabel(int index)
        {
            return "";
        }

        public override int MaxNextLinks
        {
            get { return 0; }
        }

        protected override void SetupNextLinks()
        {
            NextNodeLinks = new List<StringField>();
        }

        protected override void SetupParameters()
        {
        }

        protected override void Eval(NodeChain nodeChain)
        {
        }
    }
}