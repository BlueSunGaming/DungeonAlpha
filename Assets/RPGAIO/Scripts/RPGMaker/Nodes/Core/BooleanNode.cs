using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public abstract class BooleanNode : Node
    {
        public override NodeType NodeType
        {
            get
            {
                return NodeType.TrueOrFalse;
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
                return 2;
            }
        }


        public override bool CanRemoveLinks
        {
            get { return false; }
        }

        protected override object EvaluateNode(NodeChain nodeChain)
        {
            return Eval(nodeChain);
        }

        protected override void SetupNextLinks()
        {
            NextNodeLinks = new List<StringField> { new StringField(), new StringField() };
        }

        public override string NextNodeLinkLabel(int index)
        {
            if (index == 0) return "True";
            if (index == 1) return "False";
            else return "";
        }
        protected abstract bool Eval(NodeChain nodeChain);

        protected BooleanNode()
        {
        }
    }
}