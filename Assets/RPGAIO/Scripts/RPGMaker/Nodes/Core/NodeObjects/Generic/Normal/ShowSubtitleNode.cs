using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Subtitle", "")]
    public class ShowSubtitleNode : SimpleNode
    {
        public override string Name
        {
            get { return "On Screen Text (Subtitle)"; }
        }

        public override string Description
        {
            get { return "Shows a line of text on the screen for a duration using the Subtitle UI."; }
        }

        public override string SubText
        {
            get { return "Show text for a duration"; }
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
            Add("Text", PropertyType.TextArea, null, "");
            Add("Auto Set Duration?", PropertyType.Bool, null, false)
                .WithSubParams(SubParam("Duration", PropertyType.Float, null, 5.0f).IfFalse());
            Add("Audio", PropertyType.Sound, null, "");
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var textToShow = (string)ValueOf("Text");
            float timeToShow;
            var autoSetDuration = (bool)ValueOf("Auto Set Duration?");
            var soundPath = (string)ValueOf("Audio");


            if(!autoSetDuration)
            {
                timeToShow = (float) Parameter("Auto Set Duration?").ValueOf("Duration");
            }
            else
            {
                var clip = (AudioClip) Resources.Load(soundPath);
                timeToShow = clip.length;
            }

            GetObject.UIHandler.SubtitleUI.ShowSubtitle(textToShow, timeToShow, soundPath);
        }
    }
}