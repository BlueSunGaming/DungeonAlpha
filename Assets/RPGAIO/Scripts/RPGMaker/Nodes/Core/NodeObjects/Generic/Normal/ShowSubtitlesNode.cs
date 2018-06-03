using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Subtitle", "")]
    public class ShowSubtitlesNode : SimpleNode
    {
        public override string Name
        {
            get { return "Show Subtitles"; }
        }

        public override string Description
        {
            get { return "Shows lines of text as subtitles. Using multiple nodes will continue to add more subtitles to show."; }
        }

        public override string SubText
        {
            get { return "Blank entires are ignored"; }
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
            Add("1:Text", PropertyType.TextArea, null, "");
            Add("1:Auto Set Duration?", PropertyType.Bool, null, true)
                .WithSubParams(SubParam("1:Duration", PropertyType.Float, null, 5.0f).IfFalse()); 
            Add("1:Audio", PropertyType.Sound, null, "");

            Add("2:Text", PropertyType.TextArea, null, "");
            Add("2:Auto Set Duration?", PropertyType.Bool, null, true)
                .WithSubParams(SubParam("2:Duration", PropertyType.Float, null, 5.0f).IfFalse()); 
            Add("2:Audio", PropertyType.Sound, null, "");

            Add("3:Text", PropertyType.TextArea, null, "");
            Add("3:Auto Set Duration?", PropertyType.Bool, null, true)
                .WithSubParams(SubParam("3:Duration", PropertyType.Float, null, 5.0f).IfFalse()); 
            Add("3:Audio", PropertyType.Sound, null, "");

        }

        protected override void Eval(NodeChain nodeChain)
        {
            var textToShow1 = (string)ValueOf("1:Text");
            var audioToPlay1 = (string)ValueOf("1:Audio");

            float timeToShow1;
            var autoSetDuration1 = (bool)ValueOf("1:Auto Set Duration?");
            if (!autoSetDuration1)
            {
                timeToShow1 = (float)Parameter("1:Auto Set Duration?").ValueOf("1:Duration");
            }
            else
            {
                var clip = (AudioClip)Resources.Load(audioToPlay1);
                timeToShow1 = clip.length;
            }

            var textToShow2 = (string)ValueOf("2:Text");
            var audioToPlay2 = (string)ValueOf("2:Audio");

            float timeToShow2;
            var autoSetDuration2 = (bool)ValueOf("2:Auto Set Duration?");
            if (!autoSetDuration2)
            {
                timeToShow2 = (float)Parameter("2:Auto Set Duration?").ValueOf("2:Duration");
            }
            else
            {
                var clip = (AudioClip)Resources.Load(audioToPlay2);
                timeToShow2 = clip.length;
            }

            var textToShow3 = (string)ValueOf("3:Text");
            var audioToPlay3 = (string)ValueOf("3:Audio");

            float timeToShow3;
            var autoSetDuration3 = (bool)ValueOf("3:Auto Set Duration?");
            if (!autoSetDuration3)
            {
                timeToShow3 = (float)Parameter("3:Auto Set Duration?").ValueOf("3:Duration");
            }
            else
            {
                var clip = (AudioClip)Resources.Load(audioToPlay3);
                timeToShow3 = clip.length;
            }


            if (!string.IsNullOrEmpty(textToShow1))
                GetObject.UIHandler.SubtitleUI.ShowSubtitles(SubtitleTemplate.Line(textToShow1, timeToShow1, audioToPlay1));

            if (!string.IsNullOrEmpty(textToShow2))
                GetObject.UIHandler.SubtitleUI.ShowSubtitles(SubtitleTemplate.Line(textToShow2, timeToShow2, audioToPlay2));

            if (!string.IsNullOrEmpty(textToShow3))
                GetObject.UIHandler.SubtitleUI.ShowSubtitles(SubtitleTemplate.Line(textToShow3, timeToShow3, audioToPlay3));
        }
    }
}