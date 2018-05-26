using System;
using System.Collections;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class WaitForSubtitlesEndNode : SimpleNode
    {

        public override string Name
        {
            get { return "Wait For End of Subtitles"; }
        }

        public override string Description
        {
            get { return "Waits for any subtitles to stop showing"; }
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

        public override bool IsRoutine
        {
            get { return true; }
        }

        protected override void Eval(NodeChain nodeChain)
        {
        }

        public override IEnumerator Routine(NodeChain nodeChain)
        {
            var subtitleUI = GetObject.UIHandler.SubtitleUI;
            while (subtitleUI.IsShowingSubtitles)
            {
                yield return null;
            }
        }
    }
}