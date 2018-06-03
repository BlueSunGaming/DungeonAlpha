using System;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Audio", "")]
    public class StopSoundNode : SimpleNode
    {
        public override string Name
        {
            get { return "Stop Sound"; }
        }

        public override string Description
        {
            get { return "Stop sound by ID."; }
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
            Add("ID", PropertyType.String, null, "");
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var soundClipID = (string)ValueOf("ID");
            AudioPlayer.Instance.StopSoundById(soundClipID);
        }
    }
}