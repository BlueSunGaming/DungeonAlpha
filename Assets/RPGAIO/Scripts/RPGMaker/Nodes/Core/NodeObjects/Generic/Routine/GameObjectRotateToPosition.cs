using System.Collections;
using Assets.Scripts.Exceptions;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Movement", "GameObject")]
    public class GameObjectRotateToPosition : SimpleNode
    {

        public override string Name
        {
            get { return "GameObject Rotate To Position"; }
        }

        public override string Description
        {
            get { return "Gameobject rotates to vector3 euler rotation"; }
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
            Add("Euler Rotation", PropertyType.Vector3, null, RPGVector3.Zero, PropertySource.EnteredOrInput,PropertyFamily.Object);
            Add("Local Rotation?", PropertyType.Bool, null, true);
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
            var eulerRotation = (RPGVector3)ValueOf("Euler Rotation");
            var useLocalPos = (bool)ValueOf("Local Rotation?");

            var targetRot = eulerRotation;

            if ((bool)ValueOf("Smooth Move?"))
            {
                var speed = (float)Parameter("Smooth Move?").ValueOf("Speed");


                Debug.Log("WORLD: " + gameObject.transform.eulerAngles);
                Debug.Log("LOCAL: " + gameObject.transform.localEulerAngles);
                Debug.Log("TARGET: " + targetRot);

                if(useLocalPos)
                {
                    while (Vector3.Distance(gameObject.transform.localEulerAngles, targetRot) > 0.05f)
                    {
                        gameObject.transform.localEulerAngles = Vector3.MoveTowards(gameObject.transform.localEulerAngles, targetRot, speed * Time.deltaTime);
                        yield return null;
                    }
                }
                else
                {
                    while (Vector3.Distance(gameObject.transform.eulerAngles, targetRot) > 0.05f)
                    {
                        gameObject.transform.eulerAngles = Vector3.MoveTowards(gameObject.transform.eulerAngles, targetRot, speed * Time.deltaTime);
                        yield return null;
                    }
                }
            }
            else
            {
                if (useLocalPos)
                {
                    gameObject.transform.localEulerAngles = targetRot;
                }
                else
                {
                    gameObject.transform.eulerAngles = targetRot;
                }
            }
        }
    }
}