using System;
using System.Collections.Generic;

namespace LogicSpawn.RPGMaker
{
    public class Rmh_Enemy
    {
        //Enemies
        public List<MonsterTypeDefinition> MonsterTypes;
        public Rmh_Enemy()
        {
            MonsterTypes = new List<MonsterTypeDefinition>()
                               {
                                   new MonsterTypeDefinition { Name = "Normal" }
                               };
        }
    }

    public class MonsterTypeDefinition
    {
        public string ID;
        public string Name;

        public MonsterTypeDefinition()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Monster Type";
        }
    }
}