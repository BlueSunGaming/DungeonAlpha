using System;
using System.Collections.Generic;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker
{
    public static class CustomVariableHandler
    {
        public static void HandleCvarSetters(List<Rm_CustomVariableGetSet> customVariableSetters)
        {
            foreach (var cvarOnEnable in customVariableSetters)
            {
                var cvar = GetObject.PlayerSave.GetCustomVariable(cvarOnEnable.VariableID);
                if (cvar != null)
                {
                    switch (cvar.VariableType)
                    {
                        case Rmh_CustomVariableType.Float:
                            cvar.FloatValue = cvarOnEnable.FloatValue;
                            break;
                        case Rmh_CustomVariableType.Int:
                            cvar.IntValue = cvarOnEnable.IntValue;
                            break;
                        case Rmh_CustomVariableType.Bool:
                            cvar.BoolValue = cvarOnEnable.BoolValue;
                            break;
                        case Rmh_CustomVariableType.String:
                            cvar.StringValue = cvarOnEnable.StringValue;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

    }
}