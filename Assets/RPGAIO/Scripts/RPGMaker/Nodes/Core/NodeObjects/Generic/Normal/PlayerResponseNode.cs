using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Dialog", "")]
    public class PlayerResponseNode : SimpleNode
    {
        [JsonIgnore]
        public string DialogText
        {
            get { return (string) ValueOf("Dialog"); }
        }

        public override string Name
        {
            get { return "Player Response"; }
        }

        public override string Description
        {
            get { return "An dialog option a player can reply with.\n\nYou can prefix this node with condition nodes to prevent it from showing in certain circumstances."; }
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
            Add("Dialog", PropertyType.TextArea, null, "Hello");
        }

        protected override void Eval(NodeChain nodeChain)
        {

        }
    }
}