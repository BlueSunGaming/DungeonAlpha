using System;
using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public class QuestRequirements
    {
        public List<StringID> QuestCompletedIDs;
        public List<Rm_CustomVariableGetSet> CustomRequirements;

        public bool RequireLevel;
        public int LevelRequired ;

        public bool RequireClass;
        public string RequiredClassID;

        public bool ReqRepAboveValue;
        public bool ReqRepBelowValue;
        public string ReputationFactionID ;
        public int ReputationValue ;

        public bool RequireTraitLevel;
        public string RequiredTraitID;
        public int TraitLevel;




        public bool RequireLearntSkill;
        public string LearntSkillID;

        public QuestRequirements()
        {
            QuestCompletedIDs = new List<StringID>();
            CustomRequirements = new List<Rm_CustomVariableGetSet>();
            LevelRequired = 0;
            ReputationFactionID = "";
            ReputationValue = 0;
        }
    }

    public class StringID
    {
        public string ID;
    }
}