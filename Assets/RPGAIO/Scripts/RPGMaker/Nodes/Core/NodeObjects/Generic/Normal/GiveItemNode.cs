using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Player", "")]
    public class RemoveQuestItemNode : SimpleNode
    {
        public override string Name
        {
            get { return "Remove Quest Item From Player"; }
        }

        public override string Description
        {
            get { return "Removes all instances of a quest item from a player's inventory."; }
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

        protected override void SetupParameters()
        {
            Add("Item", PropertyType.QuestItem, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var itemId = (string)ValueOf("Item");
            GetObject.PlayerCharacter.Inventory.RemoveItem(itemId);

        }
    }
}