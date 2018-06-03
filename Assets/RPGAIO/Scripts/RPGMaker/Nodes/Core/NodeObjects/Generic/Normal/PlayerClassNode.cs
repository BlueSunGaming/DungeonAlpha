using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Player", "")]
    public class PlayerClassNode : OptionsNode
    {
        protected override void SetupNextLinks()
        {
            NextNodeLinks = new List<StringField>();
            var classDefs = Rm_RPGHandler.Instance.Player.ClassNameDefinitions;

            for (int i = 0; i < classDefs.Count; i++)
            {
                NextNodeLinks.Add(new StringField());
            }
        }
        public override bool CanRemoveLinks
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "Player Class Check"; }
        }

        public override string Description
        {
            get { return "Continues based on what class the player is"; }
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
            var classDefs = Rm_RPGHandler.Instance.Player.ClassNameDefinitions;
            if(index <= classDefs.Count - 1)
            {
                return classDefs[index].Name;
            }
            return "NULL";
        }

        protected override void SetupParameters()
        {
        }

        protected override int Eval(NodeChain nodeChain)
        {
            var playerClass = GetObject.PlayerCharacter.PlayerClassNameID;
            var classDefs = Rm_RPGHandler.Instance.Player.CharacterDefinitions;
            var classDef = classDefs.FirstOrDefault(c => c.ID == playerClass);
            var indexOf = Array.IndexOf(classDefs.ToArray(), classDef);
            if(indexOf == -1)
            {
                Debug.LogError("PlayerClassCheck is missing a class, remake the node to fix this.");
            }
            return indexOf;
        }
    }
}   