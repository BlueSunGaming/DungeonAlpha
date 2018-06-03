using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class SkillRepository
    {
        public List<Skill> AllSkills;

        public SkillRepository()
        {
            AllSkills = new List<Skill>();
        }

        public Skill Get(string skillID)
        {
            return GeneralMethods.CopyObject(AllSkills.First(i => i.ID == skillID));
        }

        public List<Skill> GetSkills(string classID)
        {
            return AllSkills.Where(s => s.AllClasses || s.ClassIDs.Contains(classID)).ToList();
        }
    }
}