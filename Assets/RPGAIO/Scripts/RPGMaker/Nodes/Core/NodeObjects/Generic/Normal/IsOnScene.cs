using UnityEngine.SceneManagement;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Comparison", "")]
    public class IsOnScene : BooleanNode
    {
        public override string Name
        {
            get { return "Is On Scene Check"; }
        }

        public override string Description
        {
            get { return "Returns true if the player is on the scene."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        protected override void SetupParameters()
        {
            Add("Scene Name", PropertyType.String, null, "", PropertySource.EnteredOrInput);
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            var sceneName = (string)ValueOf("Scene Name");
            return SceneManager.GetActiveScene().name == sceneName;
        }
    }
}