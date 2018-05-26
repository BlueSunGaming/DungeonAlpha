using Assets.Scripts.Beta;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Spawn", "Items")]
    public class SpawnItemNode : SimpleNode
    {
        public override string Name
        {
            get { return "Spawn Normal Item"; }
        }

        public override string Description
        {
            get { return "Spawns a normal item from the Item DB to a position."; }
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
            Add("Item", PropertyType.Item,null,null);
            Add("Quantity", PropertyType.Int, null, 1);
            Add("Position", PropertyType.Vector3, null, new RPGVector3(1, 1, 1), PropertySource.EnteredOrInput, PropertyFamily.Object); 
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var itemId = (string)ValueOf("Item");
            var quantity = (int)ValueOf("Quantity");
            var item = Rm_RPGHandler.Instance.Repositories.Items.Get(itemId);
            if (item != null)
            {
                var spawnPos = (RPGVector3)ValueOf("Position");

                var stackable = item as IStackable;
                if (stackable != null)
                {
                    stackable.CurrentStacks = quantity;
                    LootSpawner.Instance.SpawnItem(spawnPos, item);
                }
                else
                {
                    for (int i = 0; i < quantity; i++)
                    {
                        LootSpawner.Instance.SpawnItem(spawnPos + new RPGVector3(0, 0.2f * i, 0), Rm_RPGHandler.Instance.Repositories.Items.Get(itemId));
                    }
                }
            }
        }
    }
}