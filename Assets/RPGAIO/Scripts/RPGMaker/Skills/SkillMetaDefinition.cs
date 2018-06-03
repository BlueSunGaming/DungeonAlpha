using System;

namespace LogicSpawn.RPGMaker
{
    public class SkillMetaDefinition
    {
        public string ID;
        public string Name;

        public SkillMetaDefinition()
        {

            ID = Guid.NewGuid().ToString();
            Name = "New Skill Meta";
        }
    }
}