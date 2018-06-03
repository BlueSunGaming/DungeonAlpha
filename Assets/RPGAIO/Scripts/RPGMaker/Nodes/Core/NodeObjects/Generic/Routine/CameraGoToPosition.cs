using System.Collections;
using Assets.Scripts.Exceptions;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Movement", "Camera")]
    public class CameraGoToPosition : SimpleNode
    {

        public override string Name
        {
            get { return "Camera Goto Position"; }
        }

        public override string Description
        {
            get { return "Camera moves to vector3 position"; }
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
            Add("Using Vector 3?", PropertyType.Bool, null, true).WithSubParams(
                    SubParam("Vector3", PropertyType.Vector3, null, RPGVector3.Zero, PropertySource.EnteredOrInput, PropertyFamily.Object).IfTrue(),
                    SubParam("GameObject", PropertyType.GameObject, null, null, PropertySource.InputOnly, PropertyFamily.Object).IfFalse()
                );

            Add("Smooth Look?", PropertyType.Bool, null, true).WithSubParams(
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
            var camera = GetObject.RPGCamera;
            camera.cameraMode = CameraMode.Manual;

            var useVector3 = (bool)ValueOf("Using Vector 3?");

            Vector3 targetPos;
            if (useVector3)
            {
                targetPos = (RPGVector3)Parameter("Using Vector 3?").ValueOf("Vector3");
            }
            else
            {
                var gameObject = (GameObject)Parameter("Using Vector 3?").ValueOf("GameObject");
                if (gameObject != null)
                {
                    targetPos = gameObject.transform.position;
                }
                else
                {
                    throw new NodeParameterNotFoundException("GameObject parameter is null.");
                }
            }

            if ((bool)ValueOf("Smooth Look?"))
            {
                var speed = (float)Parameter("Smooth Look?").ValueOf("Speed");

                while (Vector3.Distance(camera.transform.position, targetPos) > 0.05f)
                {
                    camera.transform.position = Vector3.MoveTowards(camera.transform.position, targetPos, speed * Time.deltaTime);
                    yield return null;
                }
            }
            else
            {
                camera.transform.position = targetPos;
            }
        }
    }

    [NodeCategory("Test", "")]
    public class TestNode : SimpleNode
    {

        public override string Name
        {
            get { return "TEST"; }
        }

        public override string Description
        {
            get { return "Camera moves to vector3 position"; }
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
            Add("Using Vector 3?", PropertyType.Bool, null, true).WithSubParams(
                    SubParam("Vector3", PropertyType.Vector3, null, RPGVector3.Zero, PropertySource.EnteredOrInput, PropertyFamily.Object).IfTrue().WithSubParams(
                            SubParam("ABC", PropertyType.Vector3, null, RPGVector3.Zero, PropertySource.EnteredOrInput, PropertyFamily.Object),
                            SubParam("ABC123", PropertyType.Vector3, null, RPGVector3.Zero, PropertySource.EnteredOrInput, PropertyFamily.Object).WithSubParams(
                                    SubParam("123ABC", PropertyType.Vector3, null, RPGVector3.Zero, PropertySource.EnteredOrInput, PropertyFamily.Object)
                            )
                        ),
                    SubParam("GameObject", PropertyType.GameObject, null, null, PropertySource.InputOnly, PropertyFamily.Object).IfFalse()
                );

            Add("Smooth Look?", PropertyType.Bool, null, true).WithSubParams(
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
            var camera = GetObject.RPGCamera;
            camera.cameraMode = CameraMode.Manual;

            var useVector3 = (bool)ValueOf("Using Vector 3?");

            Vector3 targetPos;
            if (useVector3)
            {
                targetPos = (RPGVector3)Parameter("Using Vector 3?").ValueOf("Vector3");
            }
            else
            {
                var gameObject = (GameObject)Parameter("Using Vector 3?").ValueOf("GameObject");
                if (gameObject != null)
                {
                    targetPos = gameObject.transform.position;
                }
                else
                {
                    throw new NodeParameterNotFoundException("GameObject parameter is null.");
                }
            }

            if ((bool)ValueOf("Smooth Look?"))
            {
                var speed = (float)Parameter("Smooth Look?").ValueOf("Speed");

                while (Vector3.Distance(camera.transform.position, targetPos) > 0.05f)
                {
                    camera.transform.position = Vector3.MoveTowards(camera.transform.position, targetPos, speed * Time.deltaTime);
                    yield return null;
                }
            }
            else
            {
                camera.transform.position = targetPos;
            }
        }
    }
}