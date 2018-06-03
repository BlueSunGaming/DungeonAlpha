using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Camera","")]
    public class EnableManualCameraNode : SimpleNode
    {
        public override string Name
        {
            get { return "Enable Manual Camera"; }
        }

        public override string Description
        {
            get { return "Sets camera control mode to manual."; }
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
            GetObject.RPGCamera.cameraMode = CameraMode.Manual;
        }
    }
}