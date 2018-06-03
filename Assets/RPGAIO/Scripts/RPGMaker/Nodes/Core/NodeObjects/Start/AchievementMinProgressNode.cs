using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class AchievementMinProgress : StartNode
    {
        [JsonIgnore]
        public override string Name
        {
            get { return "Achievement Min Progress"; }
        }

        [JsonIgnore]
        public override string Description
        {
            get { return "Achievement min progress value."; }
        }

        [JsonIgnore]
        public override string SubText
        {
            get { return ""; }
        }

        public override bool CanBeLinkedTo
        {
            get { return false; }
        }

        public override bool IsStartNode
        {
            get { return true; }
        }

        public override bool ShowTarget
        {
            get { return false; }
        }

        public override bool CanBeDeleted
        {
            get { return false; }
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

        }
    }
}