using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public abstract class SimpleNode : Node
    {
        public override NodeType NodeType
        {
            get
            {
                return NodeType.Simple;
            }
        }

        public override bool HasMaxNextLinks
        {
            get
            {
                return true;
            }
        }

        public override int MaxNextLinks
        {
            get
            {
                return 1;
            }
        }
        public override bool CanRemoveLinks
        {
            get
            {
                return false;
            }
        }

        protected override object EvaluateNode(NodeChain nodeChain)
        {
            Eval(nodeChain);
            return true;
        }

        protected override void SetupNextLinks()
        {
            NextNodeLinks = new List<StringField> { new StringField { ID = "" } };
        }

        protected abstract void Eval(NodeChain nodeChain);

        protected SimpleNode()
        {
        }
    }
}