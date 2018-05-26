using System.Collections;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class LookAtDialogObjectNode : SimpleNode
    {

        public override string Name
        {
            get { return "Look At Dialog Target"; }
        }

        public override string Description
        {
            get { return "Looks at the target (NPC or otherwise) of the current dialog if there is one."; }
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
            if(DialogHandler.Instance.DialogNodeChain != null)
            {
                var dialogNpc = DialogHandler.Instance.DialogNpc;
                var pivot = dialogNpc.transform.Find("cameraPivot");

                camera.cameraMode = CameraMode.Manual;

                var targetPos = pivot != null ? pivot.transform.position + pivot.transform.forward *2 : dialogNpc.transform.position + (dialogNpc.transform.forward * 1.2f) + (dialogNpc.transform.up * 1.7f);
                var lookAtPos = pivot != null ? pivot.transform.position : dialogNpc.transform.position + (dialogNpc.transform.up * 1.7f);
                 
                var v = lookAtPos - camera.transform.position;

                var lookAt = Quaternion.LookRotation(lookAtPos - camera.transform.position);
                camera.transform.position = targetPos;
                camera.transform.rotation = lookAt;
                camera.transform.LookAt(lookAtPos);

                //lerp way
//                while (Vector3.Distance(camera.transform.position, targetPos) > 0.05f)
//                {
//                    camera.transform.position = Vector3.Lerp(camera.transform.position, targetPos, 5 * Time.deltaTime);
//                    var lookAt = Quaternion.LookRotation(lookAtPos - camera.transform.position);
//                    camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, lookAt, Time.deltaTime * 10f);
//                    yield return null;
//                }
//                Debug.Log(Vector3.Angle(v, camera.transform.forward));
//                v = lookAtPos - camera.transform.position;
//                while (Vector3.Angle(v, camera.transform.forward) > 1f)
//                {
//                    var s = Vector3.Angle(v, camera.transform.forward);
//                    v = lookAtPos - camera.transform.position;
//                    var lookAt = Quaternion.LookRotation(lookAtPos - camera.transform.position);
//                    camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, lookAt, Time.deltaTime * 10f);
//                    Debug.Log(s);
//                    yield return null;
//                }
            }

            yield return null;
        }
    }
}