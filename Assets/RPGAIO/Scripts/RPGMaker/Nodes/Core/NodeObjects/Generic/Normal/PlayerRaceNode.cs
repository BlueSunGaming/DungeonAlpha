using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Player", "")]
    public class PlayerRaceNode : OptionsNode
    {
        protected override void SetupNextLinks()
        {
            NextNodeLinks = new List<StringField>();
            var raceDefinitions = Rm_RPGHandler.Instance.Player.RaceDefinitions;

            for (int i = 0; i < raceDefinitions.Count; i++)
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
            get { return "Player Race Check"; }
        }

        public override string Description
        {
            get { return "Continues based on what race the player is"; }
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
            var raceDefinitions = Rm_RPGHandler.Instance.Player.RaceDefinitions;
            if(index <= raceDefinitions.Count - 1)
            {
                return raceDefinitions[index].Name;
            }
            return "NULL";
        }

        protected override void SetupParameters()
        {
        }

        protected override int Eval(NodeChain nodeChain)
        {
            var playerRaceId = GetObject.PlayerCharacter.PlayerRaceID;
            var raceDefinitions = Rm_RPGHandler.Instance.Player.RaceDefinitions;
            var raceDef = raceDefinitions.FirstOrDefault(c => c.ID == playerRaceId);
            var indexOf = Array.IndexOf(raceDefinitions.ToArray(), raceDef);
            if(indexOf == -1)
            {
                Debug.LogError("PlayerRaceCheck is missing a race, remake the node to fix this.");
            }
            return indexOf;
        }
    }
}