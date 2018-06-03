using System.Linq;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.API
{
    public static partial class RPG
    {
        public static class Variables
        {
            public static void SetVariable(string variableName, bool value)
            {
                var customVar = GetObject.PlayerSave.GenericStats.CustomVariables.FirstOrDefault(c => c.Name == variableName);
                customVar.BoolValue = value;
            }

        }
    }
}