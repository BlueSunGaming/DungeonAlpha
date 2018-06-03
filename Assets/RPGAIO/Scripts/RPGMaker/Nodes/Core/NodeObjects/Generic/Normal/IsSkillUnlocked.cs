using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Skills", "")]
    public class IsSkillUnlocked : BooleanNode
    {
        public override string Name
        {
            get { return "Is Skill Unlocked"; }
        }

        public override string Description
        {
            get { return "Returns true if the player has unlocked the skill."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        protected override void SetupParameters()
        {
            Add("Skill", PropertyType.Skill, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            var player = GetObject.PlayerCharacter;
            var skill = (string)ValueOf("Skill");

            if (player != null)
            {
                var skillDef = GetObject.PlayerCharacter.SkillHandler.AvailableSkills.FirstOrDefault(s => s.ID == skill);
                if (skillDef != null)
                {
                    return skillDef.Unlocked;
                }
            }

            Debug.LogError("Skill ID [" + skill + "] not found");
            return false;
        }
    }
}