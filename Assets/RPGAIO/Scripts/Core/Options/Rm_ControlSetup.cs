using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public class Rm_ControlSetup
    {
        public List<ControlDefinition> ControlDefinitions;

        public Rm_ControlSetup()
        {
            ControlDefinitions = new List<ControlDefinition>();
        }
    }
}