using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Player", "")]
    public class PlayerGenderNode : OptionsNode
    {
        protected override void SetupNextLinks()
        {
            NextNodeLinks = new List<StringField>();
            var genderDefinitions = Rm_RPGHandler.Instance.Player.GenderDefinitions;

            for (int i = 0; i < genderDefinitions.Count; i++)
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
            get { return "Player Gender Check"; }
        }

        public override string Description
        {
            get { return "Continues based on what gender the player is"; }
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
            var genderDefinitions = Rm_RPGHandler.Instance.Player.GenderDefinitions;
            if (index <= genderDefinitions.Count - 1)
            {
                return genderDefinitions[index].Name;
            }
            return "NULL";
        }

        protected override void SetupParameters()
        {
        }

        protected override int Eval(NodeChain nodeChain)
        {
            var playerGenderId = GetObject.PlayerCharacter.PlayerGenderID;
            var genderDefinitions = Rm_RPGHandler.Instance.Player.GenderDefinitions;
            var raceDef = genderDefinitions.FirstOrDefault(c => c.ID == playerGenderId);
            var indexOf = Array.IndexOf(genderDefinitions.ToArray(), raceDef);
            if(indexOf == -1)
            {
                Debug.LogError("PlayerGenderCheck is missing a gender, remake the node to fix this.");
            }
            return indexOf;
        }
    }
}