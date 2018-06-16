using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("GameObject", "")]
    public class BuildingUpgrade : SimpleNode
    {
        public override string Name
        {
            get { return "Upgrade a Building"; }
        }

        public override string Description
        {
            get { return "Upgrade a Building if possible"; }
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
            Add("GameObject", PropertyType.GameObject, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
            Add("Destroy After", PropertyType.Float, null, 0);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            GameObject gameObject = (GameObject)ValueOf("GameObject");
            var destroyAfter = (float)ValueOf("Destroy After");

            if (gameObject != null)
            {
                Object.Destroy(gameObject, destroyAfter);
            }
        }
    }
}
