using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class EnemyRepository
    {
        public List<CombatCharacter> AllEnemies;

        public EnemyRepository()
        {
            AllEnemies = new List<CombatCharacter>();
        }

        public CombatCharacter Get(string enemyID)
        {
            return AllEnemies.First(n => n.ID == enemyID);
        }
    }
}