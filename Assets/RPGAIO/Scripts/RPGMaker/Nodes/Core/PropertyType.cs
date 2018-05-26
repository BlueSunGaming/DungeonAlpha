namespace LogicSpawn.RPGMaker.Core
{
    public enum PropertyType
    {
        String,
        TextArea,
        Float,
        Int,
        Bool,
        Enum,
        StringArray,
        IntArray,

        //PopupIDs:
        Attribute,
        Statistic,
        Vital,
        Trait,
        StatusEffect,
        Skill,
        Talent,
        TraitExpDefinition,
        ExpGainedDefinition,
        PlayerLevellingDefintion,
        TalentGroup,
        Item,
        CraftableItem,
        QuestItem,
        Rm_ClassDefinition,
        Rmh_CustomVariable,
        ReputationDefinition,
        SkillMetaDefinition,
        Quest,
        NonPlayerCharacter,
        Interactable,
        SlotDefinition,
        WeaponTypeDefinition,
        RarityDefinition,
        CombatCharacter,
        Rm_LootTable,
        Achievement,
        MonsterTypeDefinition,
        Event,
        VendorShop,

        //Objects
        Sound,
        GameObject,
        Texture2D,
        Sprite,

        Vector3,
        Vector2,

        None,
        NodeChain,
        Any,

        PetDefinition,
        MetaData,
        Dialog,
    }

    public enum PropertySource
    {
        InputOnly,
        EnteredOnly,
        EnteredOrInput
    }

    public enum PropertyFamily
    {
        /// <summary>
        /// Singular string, int, bool or float.
        /// </summary>
        Primitive,

        /// <summary>
        /// Singular objects such as: GameObject, Monobehaviours, Attributes, Vitals etc..
        /// </summary>
        Object,

        /// <summary>
        /// Lists or arrays of primitive types or objects.
        /// </summary>
        List,

        /// <summary>
        /// Can be any
        /// </summary>
        Any
    }
}