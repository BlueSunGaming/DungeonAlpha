using System;
using System.Collections;
using Assets.Scripts.Beta.NewImplementation;
using Assets.Scripts.Exceptions;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Animation","")]
    public class PlayAnimationNode : SimpleNode
    {
        public override string Name
        {
            get { return "Play Legacy Animation"; }
        }

        public override string Description
        {
            get { return "Plays an animation."; }
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
            Add("GameObject", PropertyType.GameObject, null, null, PropertySource.InputOnly, PropertyFamily.Object);
            Add("Animation Name", PropertyType.String,null, "", PropertySource.EnteredOrInput);
            Add("Play Time", PropertyType.Float,null, 1, PropertySource.EnteredOrInput);
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
            var gameObject = (GameObject)ValueOf("GameObject");
            var animationToPlay = (string)ValueOf("Animation Name");
            var timeToPlay = Convert.ToSingle(ValueOf("Play Time"));

            var anim = gameObject.GetComponent<Animation>();

            var rpgController = gameObject.GetComponent<RPGController>();
            if(rpgController != null)
            {
                anim = rpgController.Animation;
                anim.CrossFade(animationToPlay);
            }
            else
            {
                gameObject.GetComponent<Animation>().CrossFade(animationToPlay);
            }

            if (anim == null) throw new NodeParameterMissingException("Animation component not found on gameobject");

            yield return new WaitForSeconds(timeToPlay);
        }
    }
}