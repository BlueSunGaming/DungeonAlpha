using System;
using System.Collections;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Player", "")]
    public class ResumePlayerNode : SimpleNode
    {
        public override string Name
        {
            get { return "Resume Player"; }
        }

        public override string Description
        {
            get { return "Resumes control of the player."; }
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
        }

        protected override void Eval(NodeChain nodeChain)
        {
            GetObject.PlayerController.Interacting = false;
        }
    }
}