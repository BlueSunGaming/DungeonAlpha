using Assets.Scripts.Testing;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Dialog", "")]
    public class OpenDialogNode : SimpleNode
    {
        public override string Name
        {
            get { return "Open Dialog"; }
        }

        public override string Description
        {
            get { return "Opens a dialog at the player's position with a manually specified interactive target"; }
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
            Add("Dialog", PropertyType.Dialog, null, "" ,PropertySource.EnteredOrInput);
            Add("Specify Interactable Object", PropertyType.Bool, null, false)
                .WithSubParams(SubParam("Interaction Object", PropertyType.GameObject, null, null,PropertySource.EnteredOrInput, PropertyFamily.Object).IfTrue(),
                               SubParam("Name", PropertyType.String, null, "", PropertySource.EnteredOrInput).IfFalse(),
                               SubParam("Image", PropertyType.Texture2D, null, null, PropertySource.EnteredOnly, PropertyFamily.Object).IfFalse()
                               );
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var dialogId = (string) ValueOf("Dialog");
            var specifyObject = (bool)ValueOf("Specify Interactable Object");
            var gameObjectToUse = Parameter("Specify Interactable Object").ValueOf("Interaction Object") as GameObject;

            if(!specifyObject)
            {
                gameObjectToUse = null;
            }

            var dialogName = (string)Parameter("Specify Interactable Object").ValueOf("Name");
            var dialogSpritePath = (string)Parameter("Specify Interactable Object").ValueOf("Image");
            var dialogSprite = string.IsNullOrEmpty(dialogSpritePath) ? null : (Texture2D)Resources.Load(dialogSpritePath);

            InteractiveObjectMono interactiveObject = null;
            if(gameObjectToUse != null)
            {
                interactiveObject = gameObjectToUse.GetComponent<InteractiveObjectMono>();
            }

            if(DialogHandler.Instance.Interacting)
            {
                Debug.LogWarning("[RPGAIO] Ending active dialog before running new one through node event.");
                DialogHandler.Instance.EndDialog();
            }

            if(interactiveObject == null && gameObjectToUse != null)
            {
                Debug.LogWarning("[RPGAIO] Specified an interactive object to use but it is not an NPC or Interactable Object.");
            }


            //Set values if gameObject is null
            DialogHandler.Instance.CustomDialogName = "";
            DialogHandler.Instance.CustomDialogSprite = null;

            if(gameObjectToUse == null)
            {
                DialogHandler.Instance.CustomDialogName = dialogName;
                DialogHandler.Instance.CustomDialogSprite = GeneralMethods.CreateSprite(dialogSprite);
            }

            DialogHandler.Instance.BeginDialog(dialogId, interactiveObject);
        }
    }
}