using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Animation","")]
    public class FadeScreenNode : SimpleNode
    {
        public override string Name
        {
            get { return "Fade Screen"; }
        }

        public override string Description
        {
            get { return "Fades the screen in or out."; }
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
            Add("Fade Mode?", PropertyType.StringArray,new[]{"Fade Out","Fade In"}, 0);
            Add("Fade Color?", PropertyType.StringArray,new[]{"Black","White"}, 0);
            Add("Fade Time", PropertyType.Float,0, 2);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            
        }

        public override bool IsRoutine
        {
            get { return true; }
        }


        public override IEnumerator Routine(NodeChain nodeChain)
        {
            var fadeOut = (int)ValueOf("Fade Mode?") == 0;
            var fadeToBlack = (int)ValueOf("Fade Color?") == 0;
            var fadeTime = Convert.ToSingle(ValueOf("Fade Time"));

            var fadeObject = GameObject.FindGameObjectWithTag("ScreenFader");
            var fadeImage = fadeObject.GetComponent<Image>();

            var targetFadeColor = fadeToBlack ? Color.black : Color.white;
            var t = 0.0f;

            while(t < 1)
            {
                if (fadeOut)
                {
                    var startColor = Color.clear;
                    fadeImage.color = Color.Lerp(startColor, targetFadeColor, t);
                }
                else
                {
                    var startColor = targetFadeColor;
                    fadeImage.color = Color.Lerp(startColor, Color.clear, t);
                }

                t += Time.deltaTime / fadeTime;
                yield return null;
            }

            if (fadeOut)
            {
                fadeImage.color = targetFadeColor;
            }
            else
            {
                fadeImage.color = Color.clear;
            }

            yield return null;
        }
    }
}