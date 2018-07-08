using System;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    enum PlayerBuilding
    {
        NONE = 0,
        TENT,
        BUILDING,
        NUM_PLAYER_BUILDINGS
    }

    public class EnumHelperPlayerBuilding
    {
        private static string[] enums = new string[(int)PlayerBuilding.NUM_PLAYER_BUILDINGS]; // string[PlayerBuilding.NUM_PLAYER_BUILDINGS];

        static EnumHelperPlayerBuilding()
        {
            enums[(int)PlayerBuilding.NONE] = "None";
            enums[(int)PlayerBuilding.TENT] = "Player_Tent1";
            enums[(int)PlayerBuilding.BUILDING] = "Player_Building1";
        }

        public static string GetStringFromInt(int enumToCheck)
        {
            string retString = "";
            if (enumToCheck < (int)PlayerBuilding.NUM_PLAYER_BUILDINGS)
            {
                retString = enums[enumToCheck];
            }

            return retString;
        }
    }

    [NodeCategory("GameObject", "Items")]
    public class TranslateEnumToBuildingObject : PropertyNode
    {
        public override string Name
        {
            get { return "Translate Enum To Building Object"; }
        }

        public override string Description
        {
            get { return "Translate Enum To Building Object"; }
        }

        public override string SubText
        {
            get { return ""; }
        }
        public override PropertyFamily PropertyFamily
        {
            get { return PropertyFamily.Primitive; }
        }

        protected override PropertyType PropertyNodeType
        {
            get { return PropertyType.Int; }
        }

        protected override bool InheritsPropertyType
        {
            get { return false; }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return true;
            }
        }

        public override string NextNodeLinkLabel(int index)
        {
            return "Next";
        }

        protected override void SetupParameters()
        {
            Add("Enum", PropertyType.Int, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
        }
        
        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            string retString = "";
            var itemId = (int)ValueOf("Enum");
            
            if (itemId != null)
            {
                retString = EnumHelperPlayerBuilding.GetStringFromInt(itemId);   
            }

            return retString;
        }
    }
}