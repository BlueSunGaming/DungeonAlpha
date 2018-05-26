using System;

namespace LogicSpawn.RPGMaker
{
    public class DifficultyDefinition
    {
        public string ID;
        public string Name;
        public float DamageMultiplier;

        public DifficultyDefinition()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Difficulty";
            DamageMultiplier = 1.0f;
        }
        public DifficultyDefinition(string name, float multiplier)
        {
            ID = Guid.NewGuid().ToString();
            Name = name;
            DamageMultiplier = multiplier;
        }
    }
}