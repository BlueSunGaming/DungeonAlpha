 using System;
 using System.Collections.Generic;
 using LogicSpawn.RPGMaker.Generic;
 using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class SkillHandler
    {
        [JsonIgnore] 
        public PlayerCharacter Player;

        public List<Skill> AvailableSkills ;

        public SkillBarSlot[] Slots ;

        public SkillHandler(PlayerCharacter player)
        {
            Player = player;
            AvailableSkills = new List<Skill>();
            Slots = new SkillBarSlot[Rm_RPGHandler.Instance.Combat.SkillBarSlots];  
            for (int i = 0; i < Slots.Length; i++)
            {
                Slots[i] = new SkillBarSlot(this);
            }
        }

        private void LoadSkills()
        {
            var skillsLoaded = Rm_RPGHandler.Instance.Repositories.Skills.GetSkills(Player.PlayerCharacterID);
            var skills = new List<Skill>();
            foreach(var skill in skillsLoaded)
            {
                if (skill.EnemyOnlySkill) continue;

                var skillObj = GeneralMethods.CopyObject(skill);
                skillObj.CurrentRank = 0;
                skills.Add(skillObj);
            }
            AvailableSkills.AddRange(skills);
        }

        public void Init()
        {
            LoadSkills();
        }
    }
}