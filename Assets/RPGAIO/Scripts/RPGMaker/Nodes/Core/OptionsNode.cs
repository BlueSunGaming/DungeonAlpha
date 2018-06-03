using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public abstract class OptionsNode : Node
    {
        public override NodeType NodeType
        {
            get
            {
                return NodeType.Options;
            }
        }

        public override bool HasMaxNextLinks
        {
            get
            {
                return false;
            }
        }

        public override bool CanRemoveLinks
        {
            get { return true; }
        }

        protected override object EvaluateNode(NodeChain nodeChain)
        {
            return Eval(nodeChain);
        }

        protected override void SetupNextLinks()
        {
            NextNodeLinks = new List<StringField> { new StringField(), new StringField(), new StringField() };
        }

        protected abstract int Eval(NodeChain nodeChain);

        protected OptionsNode()
        {
        }
    }
}