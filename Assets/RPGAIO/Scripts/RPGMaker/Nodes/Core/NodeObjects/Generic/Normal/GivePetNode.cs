using System;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Pet", "")]
    public class GivePetNode : SimpleNode
    {
        public override string Name
        {
            get { return "Give Pet"; }
        }

        public override string Description
        {
            get { return "Gives the player a new pet."; }
        }

        public override string SubText
        {
            get { return "Give/Replace Player's Pet"; }
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
            Add("Pet", PropertyType.PetDefinition,null, ""); 
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var petId = (string)ValueOf("Pet");
            var petDef = Rm_RPGHandler.Instance.Player.PetDefinitions.FirstOrDefault(p => p.ID == petId);
            if (petDef != null)
            {
                var petData = new PetData(petDef);
                var playerPos = GetObject.PlayerMonoGameObject.transform;
                PetMono.SpawnPet(petData, playerPos.position - playerPos.forward);
            }
        }
    }
}