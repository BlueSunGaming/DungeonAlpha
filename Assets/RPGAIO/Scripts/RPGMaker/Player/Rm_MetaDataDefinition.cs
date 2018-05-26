using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker
{
    public class Rm_MetaDataDefinition
    {
        public string ID;
        public string Name;
        public string TooltipDescription;
        public List<MetaDataValue> Values;

        public Rm_MetaDataDefinition()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Metadata";
            TooltipDescription = "";
            Values = new List<MetaDataValue>();
        }

        public override string ToString()
        {
            return Name;
        }

        public string GetValueName(string valueId)
        {
            var metaDataValue = Values.FirstOrDefault(v => v.ID == valueId);
            return metaDataValue != null ? metaDataValue.Name : "";
        }
    }
}