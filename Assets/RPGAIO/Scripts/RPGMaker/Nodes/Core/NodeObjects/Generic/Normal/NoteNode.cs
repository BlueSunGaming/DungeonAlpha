using System;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Notes", "")]
    public class NoteNode : SimpleNode
    {
        public override string Name
        {
            get { return "Note"; }
        }

        public override string Description
        {
            get { return ""; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return false;
            }
        }

        public override int MaxNextLinks
        {
            get { return 0; }
        }

        public override string NextNodeLinkLabel(int index)
        {
            return "";
        }

        protected override void SetupParameters()
        {
            Add("Note", PropertyType.TextArea, null, "");
        }

        protected override void Eval(NodeChain nodeChain)
        {
            
        }
    }
}