using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Dialog", "")]
    public class DialogNode : OptionsNode
    {
        [JsonIgnore]
        public string DialogText
        {
            get { return (string) ValueOf("Dialog"); }
        }

        [JsonIgnore]
        public string DialogAudio
        {
            get { return (string)ValueOf("Voice"); }
        }


        public override string Name
        {
            get { return "NPC Response"; }
        }

        public override string Description
        {
            get { return "NPC dialog"; }
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
            return (index + 1).ToString();
        }

        protected override void SetupParameters()
        {
            Add("Dialog", PropertyType.TextArea, null, "Hello");
            Add("Voice", PropertyType.Sound, null, null);
        }

        protected override int Eval(NodeChain nodeChain)
        {
            Debug.Log("Evaluated a dialog node.");
            return -1;
        }
    }
}