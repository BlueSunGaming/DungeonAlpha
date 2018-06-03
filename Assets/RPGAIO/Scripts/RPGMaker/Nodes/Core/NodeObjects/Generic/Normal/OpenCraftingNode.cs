using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Game", "")]
    public class OpenCraftingNode : SimpleNode
    {
        public override string Name
        {
            get { return "Open Crafting"; }
        }

        public override string Description
        {
            get { return "Opens the crafting window."; }
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
            Add("Close If Player Moves", PropertyType.Bool, null, true, PropertySource.EnteredOnly);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            RPG.Events.OnOpenCrafting(new RPGEvents.OpenCraftingEventArgs());
            var closeIfPlayerMoves = (bool) ValueOf("Close If Player Moves");
            if(closeIfPlayerMoves)
            {
                GetObject.UIHandler.CraftingUI.OpenedByDialog = true;
            }
        }
    }
}