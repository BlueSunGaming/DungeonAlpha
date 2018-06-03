namespace LogicSpawn.RPGMaker
{
    public enum GenericValue
    {
        /*
         *  //ALL: popup of: raw (int, float, string, bool, Vector3), nodesWithReturnType, Cvars, Random
            //COMBAT:
                //core_damage_taken: physical, elemental [n] , totaldamage
                //default: attacker/defender (attr, statistic, trait, vital, level, enemytype, position)
            //DIALOG:
                //default: dialog/npc (attr, statistic, trait, vital, level, enemytype, position)
            //EVENT:
                //default: combatants.where(). (attr, statistic, trait, vital, level, enemytype, position)
         * 
         */
        Whole_Number,
        Number,
        Boolean,
        Text,
        Position,
        Physical_Damage,
        Elemental_Damage,
        Total_Elemental_Damage,
        Total_Damage,
        Node_Chain_Result,
        Attribute_Value,
        Statistic_Value,
        Trait_Level,
        Vital_Value,
        Reputation,
        Level,
        Enemy_Type,
        Custom_Variable,
        Random,
        Auto
    }

    public enum GenericNodeTarget
    {
        Physical_Damage,
        All_Elemental_Damage,
        Elemental_Damage,
        Total_Damage,

        Attacker, //sub target? e.g. attacker.attribute.base
        Defender,
        DialogNPC,
        Player,
        Combatants,

        Attribute_Value,
        Statistic_Value,
        Trait_Level,
        Vital_Value,
        Reputation,
        Exp,
        Gold,
        SkillPoints,
        TalentPoints,
        Level,
        Enemy_Type,
        Position,
        Custom_Variable,
        Random
    }
}