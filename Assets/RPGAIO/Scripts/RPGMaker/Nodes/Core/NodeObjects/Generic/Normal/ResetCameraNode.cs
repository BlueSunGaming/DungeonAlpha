using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Camera","")]
    public class ResetCameraNode : SimpleNode
    {
        public override string Name
        {
            get { return "Reset Camera"; }
        }

        public override string Description
        {
            get { return "Resets camera to default camera mode."; }
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

        protected override void Eval(NodeChain nodeChain)
        {
            GetObject.RPGCamera.cameraMode = Rm_RPGHandler.Instance.DefaultSettings.DefaultCameraMode;
        }
    }
}