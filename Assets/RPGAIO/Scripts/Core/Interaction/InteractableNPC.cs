using System.Linq;
using Assets.Scripts.Core.Interaction;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;

namespace Assets.Scripts.Testing
{
    public class InteractableNPC : InteractiveObjectMono
    {
        public NpcCharacterMono NPC;
        void Awake()
        {
            Type = InteractableType.NPC;
            NPC = GetComponent<NpcCharacterMono>();
        }

        public override void Interaction()
        {
            if (!NPC.Character.Alive || NPC.Controller.HandlingActions) return;

            //inform user that npc doesnt exist if that's the case
            if((DialogHandler.Instance.DialogNpc as InteractableNPC) != this)
            {
                //Debug.Log("You interacted with NPC:  " + NPC.NPC.Name);
                //Debug.Log(NPC.NPC.Interaction.ConversationNodeId);
                DialogHandler.Instance.BeginDialog(NPC.NPC.Interaction.ConversationNodeId, this);
            }
        }

        public override string GetName()
        {
            return NPC.NPC.Name;
        }

        public override Texture2D GetImage()
        {
            return NPC.NPC.Image;
        }

        protected override void OnEnable()
        {
            if (Initialised) return;
            Initialised = true;
        }
    }
}