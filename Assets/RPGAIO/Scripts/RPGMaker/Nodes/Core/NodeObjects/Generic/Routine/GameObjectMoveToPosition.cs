using System.Collections;
using Assets.Scripts.Exceptions;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Movement", "GameObject")]
    public class GameObjectMoveToPosition : SimpleNode
    {

        public override string Name
        {
            get { return "GameObject Move To Position"; }
        }

        public override string Description
        {
            get { return "Gameobject moves to vector3 position"; }
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
            Add("Using Vector 3?", PropertyType.Bool, null, true).WithSubParams(
                SubParam("Vector3", PropertyType.Vector3, null, RPGVector3.Zero, PropertySource.EnteredOrInput, PropertyFamily.Object).IfTrue(),
                SubParam("Target GameObject", PropertyType.GameObject, null, null, PropertySource.InputOnly, PropertyFamily.Object).IfFalse()
                );

            Add("Smooth Move?", PropertyType.Bool, null, true).WithSubParams(
                SubParam("Speed", PropertyType.Float, null, 5).IfTrue()
                );
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
            var gameObject = (GameObject) ValueOf("GameObject");

            var useVector3 = (bool)ValueOf("Using Vector 3?");

            Vector3 targetPos;
            if (useVector3)
            {
                targetPos = (RPGVector3)Parameter("Using Vector 3?").ValueOf("Vector3");
            }
            else
            {
                var targetGameObject = (GameObject)Parameter("Using Vector 3?").ValueOf("Target GameObject");
                if (targetGameObject != null)
                {
                    targetPos = targetGameObject.transform.position;
                }
                else
                {
                    throw new NodeParameterNotFoundException("GameObject parameter is null.");
                }
            }

            if ((bool)ValueOf("Smooth Move?"))
            {
                var speed = (float)Parameter("Smooth Move?").ValueOf("Speed");

                while (Vector3.Distance(gameObject.transform.position, targetPos) > 0.05f)
                {
                    gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPos, speed * Time.deltaTime);
                    yield return null;
                }
            }
            else
            {
                gameObject.transform.position = targetPos;
            }
        }
    }
}