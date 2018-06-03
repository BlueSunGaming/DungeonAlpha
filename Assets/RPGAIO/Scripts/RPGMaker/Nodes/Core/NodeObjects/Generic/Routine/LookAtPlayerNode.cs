using System.Collections;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class LookAtPlayerNode : SimpleNode
    {

        public override string Name
        {
            get { return "Look At Player"; }
        }

        public override string Description
        {
            get { return "Waits for given seconds"; }
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
            var playerModel = GetObject.PlayerController.CharacterModel;
            var pivot = GetObject.PlayerMono.transform.Find("cameraPivot");

            camera.cameraMode = CameraMode.Manual;
            var targetPos = pivot != null ? pivot.transform.position + pivot.transform.forward  : playerModel.transform.position + (playerModel.transform.forward * 1.2f) + (playerModel.transform.up * 1.7f);
            var lookAtPos = pivot != null ? pivot.transform.position : playerModel.transform.position + (playerModel.transform.up * 1.7f);

            var v = lookAtPos - camera.transform.position;
            var lookAt = Quaternion.LookRotation(lookAtPos - camera.transform.position);
            camera.transform.position = targetPos;
            camera.transform.rotation = lookAt;
            camera.transform.LookAt(lookAtPos);

            //Lerp way
            
//            while (Vector3.Distance(camera.transform.position, targetPos) > 0.05f)
//            {
//                camera.transform.position = Vector3.Lerp(camera.transform.position, targetPos, 5 * Time.deltaTime);
//                var lookAt = Quaternion.LookRotation(lookAtPos - camera.transform.position);
//                camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, lookAt, Time.deltaTime * 5f);
//                yield return null;
//            }
//            Debug.Log(Vector3.Angle(v, camera.transform.forward));
//            v = lookAtPos - camera.transform.position;
//            while (Vector3.Angle(v, camera.transform.forward) > 1f)
//            {
//                var s = Vector3.Angle(v, camera.transform.forward);
//                v = lookAtPos - camera.transform.position;
//                var lookAt = Quaternion.LookRotation(lookAtPos - camera.transform.position);
//                camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, lookAt, Time.deltaTime * 5f);
//                yield return null;
//            }

            yield return null;
        }
    }
}