using System.Collections;
using Assets.Scripts.Exceptions;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Movement","Camera")]
    public class CameraLookAtPosition : SimpleNode
    {

        public override string Name
        {
            get { return "Camera Look At Position"; }
        }

        public override string Description
        {
            get { return "Camera looks at vector3 position"; }
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
            Add("Using Vector 3?", PropertyType.Bool,null,true).WithSubParams(
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

            var useVector3 = (bool) ValueOf("Using Vector 3?");

            Vector3 lookAtPos;
            if(useVector3)
            {
                lookAtPos = (RPGVector3)Parameter("Using Vector 3?").ValueOf("Vector3");
            }
            else
            {
                var gameObject = (GameObject)Parameter("Using Vector 3?").ValueOf("GameObject");
                if(gameObject != null)
                {
                    lookAtPos = gameObject.transform.position;
                }
                else
                {
                    throw new NodeParameterNotFoundException("GameObject parameter is null.");
                }
            }


            if((bool)ValueOf("Smooth Look?"))
            {
                var speed = (float)Parameter("Smooth Look?").ValueOf("Speed");
                var v = lookAtPos - camera.transform.position;
                while (Vector3.Angle(v, camera.transform.forward) > 1f)
                {
                    v = lookAtPos - camera.transform.position;
                    var lookAt = Quaternion.LookRotation(lookAtPos - camera.transform.position);
                    camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, lookAt, Time.deltaTime * speed);
                    yield return null;
                }
            }
            else
            {
                var lookAt = Quaternion.LookRotation(lookAtPos - camera.transform.position);
                camera.transform.rotation = lookAt;
            }
        }
    }
}