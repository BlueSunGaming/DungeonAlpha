using System;
using LogicSpawn.RPGMaker.API;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Scene", "")]
    public class LoadSceneNode : SimpleNode
    {
        public override string Name
        {
            get { return "Load Scene"; }
        }

        public override string Description
        {
            get { return "Loads a scene by name"; }
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
            return "Next";
        }

        protected override void SetupParameters()
        {
            Add("Name", PropertyType.String, null, "", PropertySource.EnteredOrInput); //param value format: (index, index/string/value), e.g. 0,abc-123 => 0 represents attribute, abc-123 represents attribute
        }

        protected override void Eval(NodeChain nodeChain)
        {
            RPG.LoadLevel((string)ValueOf("Name"));
        }
    }
}