using System;
using System.Collections;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class PlaySoundNode : SimpleNode
    {
        public override string Name
        {
            get { return "Play Sound"; }
        }

        public override string Description
        {
            get { return "Plays a sound."; }
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

        public override bool IsRoutine
        {
            get { return true; }
        }

        public override string NextNodeLinkLabel(int index)
        {
            return "Next";
        }

        protected override void SetupParameters()
        {
            Add("Sound", PropertyType.Sound, null, null);
            Add("With Duration?", PropertyType.Bool, null, false).WithSubParams(
                    SubParam("Duration",PropertyType.Float,null,0.0f).IfTrue()
                );
            Add("Use Play Once ID?", PropertyType.Bool, null, false).WithSubParams(
                    SubParam("ID",PropertyType.String,null,"").IfTrue()
                );
            Add("Wait to finish?", PropertyType.Bool, null, false);
        }

        public override IEnumerator Routine(NodeChain nodeChain)
        {
            var soundClipPath = (string)ValueOf("Sound");
            var soundClip = Resources.Load(soundClipPath) as AudioClip;

            var usePlayOnceId = (bool)ValueOf("Use Play Once ID?");
            var playOnceId = usePlayOnceId ? (string)Parameter("Use Play Once ID?").ValueOf("ID") : "";

            var useDuration = (bool)ValueOf("With Duration?");
            var duration = useDuration ? (float)Parameter("With Duration?").ValueOf("Duration") : -1f;

            var waitToFinish = (bool)ValueOf("Wait to finish?");

            AudioPlayer.Instance.Play(soundClip, AudioType.SoundFX,Vector3.zero,null, playOnceId, duration);

            if(waitToFinish)
            {
                if(useDuration)
                {
                    yield return new WaitForSeconds(duration);
                }
                else
                {
                    yield return new WaitForSeconds(soundClip.length);
                }
            }

            yield return null;
        }

        protected override void Eval(NodeChain nodeChain)
        {
        }
    }
}