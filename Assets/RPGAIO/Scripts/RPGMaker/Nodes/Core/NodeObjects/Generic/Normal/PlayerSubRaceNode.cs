using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Player", "")]
    public class PlayerSubRaceNode : OptionsNode
    {
        protected override void SetupNextLinks()
        {
            NextNodeLinks = new List<StringField>();
            var subRaceDefinitions = Rm_RPGHandler.Instance.Player.SubRaceDefinitions;

            for (int i = 0; i < subRaceDefinitions.Count; i++)
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
            get { return "Player Sub-Race Check"; }
        }

        public override string Description
        {
            get { return "Continues based on what sub-race the player is"; }
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
            var subRaceDefinitions = Rm_RPGHandler.Instance.Player.SubRaceDefinitions;
            if (index <= subRaceDefinitions.Count - 1)
            {
                return subRaceDefinitions[index].Name;
            }
            return "NULL";
        }

        protected override void SetupParameters()
        {
        }

        protected override int Eval(NodeChain nodeChain)
        {
            var playersubRaceId = GetObject.PlayerCharacter.PlayerSubRaceID;
            var subRaceDefinitions = Rm_RPGHandler.Instance.Player.SubRaceDefinitions;
            var raceDef = subRaceDefinitions.FirstOrDefault(c => c.ID == playersubRaceId);
            var indexOf = Array.IndexOf(subRaceDefinitions.ToArray(), raceDef);
            if(indexOf == -1)
            {
                Debug.LogError("PlayerSubRaceCheck is missing a sub-race, remake the node to fix this.");
            }
            return indexOf;
        }
    }
}