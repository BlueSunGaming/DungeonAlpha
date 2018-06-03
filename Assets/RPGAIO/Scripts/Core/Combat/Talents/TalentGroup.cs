using System;

namespace LogicSpawn.RPGMaker.Core
{
    public class TalentGroup
    {
        public string ID;
        public string Name;
        public int NumberAllowedActive;

        public TalentGroup()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Talent Group";
            NumberAllowedActive = 1;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}