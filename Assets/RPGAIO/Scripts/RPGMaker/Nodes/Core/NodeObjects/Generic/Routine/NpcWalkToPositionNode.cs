using System.Collections;
using Assets.Scripts.Beta.NewImplementation;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class NpcWalkToPositionNode : SimpleNode
    {
        public override string Name
        {
            get { return "NPC Walk To Position"; }
        }

        public override string Description
        {
            get { return "Makes an NPC walk to a position and then stop."; }
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
            Add("GameObject", PropertyType.GameObject, null, null, PropertySource.InputOnly, PropertyFamily.Object);
            Add("Target Position", PropertyType.Vector3, null, RPGVector3.Zero, PropertySource.EnteredOrInput, PropertyFamily.Object);
            Add("Move Speed", PropertyType.Float, null, 5.0f, PropertySource.EnteredOrInput);
            Add("Wait to finish?", PropertyType.Bool, null, false);
            Add("Run Instead of Walk?", PropertyType.Bool, null, false);
        }

        public override IEnumerator Routine(NodeChain nodeChain)
        {

            var gameObject = (GameObject)ValueOf("GameObject");
            var targetPos = (RPGVector3)ValueOf("Target Position");
            var moveSpeed = (float)ValueOf("Move Speed");
            var waitToFinish = (bool)ValueOf("Wait to finish?");
            var runInsteadOfWalk = (bool)ValueOf("Run Instead of Walk?");   

            var control = gameObject.GetComponent<RPGController>();
            var queue = new RPGActionQueue();

            var originalAiSetting = control.ControlledByAI;

            var moveAnim = runInsteadOfWalk ? control.Character.LegacyAnimations.RunAnim : control.Character.LegacyAnimations.WalkAnim;
            queue.Add(RPGActionFactory.MoveToPosition(targetPos, 1, moveSpeed).WithAnimation(moveAnim));
            queue.Add(RPGActionFactory.WaitForSeconds(0.1f));
            queue.Add(RPGActionFactory.PlayAnimation(control.Character.LegacyAnimations.IdleAnim));
            queue.Add(RPGActionFactory.BasicJump(0));

            control.ForceStopHandlingActions();
            control.BeginActionQueue(queue);


            if(waitToFinish)
            {
                while(Vector3.Distance(control.transform.position, targetPos) > 2.5f)
                {
                    yield return null;
                }
            }

            yield return null;
        }

        protected override void Eval(NodeChain nodeChain)
        {
        }
    }
}