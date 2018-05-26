using System;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public class MonsterKillsNode : BooleanNode
    {
        public override string Name
        {
            get { return "Monster Kills"; }
        }

        public override string Description
        {
            get { return "Checks player monster kills."; }
        }

        public override string SubText
        {
            get { return "True if Kills >= X"; }
        }

        protected override void SetupParameters()
        {
            Add("Required Kills", PropertyType.Int, null, 1);
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            return GetObject.PlayerSave.GenericStats.MonstersKilled >= Convert.ToInt32(ValueOf("Required Kills"));
        }
    }
}