using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public class GenericStats
    {
        public int MonstersKilled ;
        public int GoldEarned ;
        public int ItemsLooted ;
        public List<Rmh_CustomVariable> CustomVariables ;
        public List<string> SetOnceNodes ; //todo: remove this

        public GenericStats()
        {
            MonstersKilled = GoldEarned = ItemsLooted = 0;
            CustomVariables = new List<Rmh_CustomVariable>(Rm_RPGHandler.Instance.DefinedVariables.Vars);
            SetOnceNodes = new List<string>();
        }
    }
}