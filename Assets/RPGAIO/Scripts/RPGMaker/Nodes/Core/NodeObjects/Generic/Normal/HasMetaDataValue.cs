using System.Linq;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Comparison", "")]
    public class HasMetaDataValue : BooleanNode
    {
        public override string Name
        {
            get { return "Player MetaData Value Check"; }
        }

        public override string Description
        {
            get { return "Returns true if the player has the right value for a meta-data."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        protected override void SetupParameters()
        {
            Add("MetaData", PropertyType.MetaData, null, "", PropertySource.EnteredOnly, PropertyFamily.Object);
            Add("Value Name", PropertyType.String, null, "", PropertySource.EnteredOrInput, PropertyFamily.Primitive);

        }

        protected override bool Eval(NodeChain nodeChain)
        {
            var combatant = GetObject.PlayerCharacter;
            var metaDataId = (string)ValueOf("MetaData");
            var valueName = (string) ValueOf("Value Name");

            var meta = combatant.GetMetaDataByID(metaDataId);
            
            var metaDef = RPG.Stats.GetMetaDataByID(metaDataId);
            var valueId = metaDef.Values.FirstOrDefault(v => v.Name == valueName);

            if(valueId != null)
            {
                if(meta.ValueID == valueId.ID)
                {
                    return true;
                }
            }

            return false;
        }
    }
}