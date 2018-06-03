using System.Collections.Generic;

namespace LogicSpawn.RPGMaker
{
    public class Rmh_ASVT
    {
        //Attributes, Vitals, Traits

        public string AttributeNameTitle = "Attributes";
        public List<Rm_AttributeDefintion> AttributesDefinitions;

        public string VitalNameTitle = "Vitals";
        public List<Rm_VitalDefinition> VitalDefinitions;

        public string StatisticNameTitle = "Statistics";
        public List<Rm_StatisticDefintion> StatisticDefinitions;

        public string TraitNameTitle = "Traits";
        public List<Rm_TraitDefintion> TraitDefinitions;

        //Stats
        public bool EnableElementalDamage; //remove this, not needed
        public List<ElementalDamageDefinition> ElementalDamageDefinitions;

        //Movement stat
        public float BaseNpcMovementSpeed;
        public float BaseMovementSpeed;
        public bool UseStatForMovementSpeed;
        public string StatForMovementID;
        public float RegenInterval;

        public float JumpHeight;

        public bool AllowExpToOverflow;

        public Rmh_ASVT()
        {
            BaseNpcMovementSpeed = 2.0f;
            RegenInterval = 0.9f;
            BaseMovementSpeed = 4.0f;
            UseStatForMovementSpeed = false;
            StatForMovementID = "";

            AllowExpToOverflow = false;

            AttributesDefinitions = new List<Rm_AttributeDefintion>();
            VitalDefinitions = new List<Rm_VitalDefinition>()
                                   {
                                       new Rm_VitalDefinition()
                                           {
                                               Name = "Health",
                                               DefaultValue = 100,
                                               UpperLimit = 99999,
                                               IsHealth = true,
                                               HasUpperLimit = false
                                           }
                                   };
            StatisticDefinitions = new List<Rm_StatisticDefintion>();
            TraitDefinitions = new List<Rm_TraitDefintion>();
            ElementalDamageDefinitions = new List<ElementalDamageDefinition>();
            EnableElementalDamage = true;
            JumpHeight = 5.0f;
        }

    }
}