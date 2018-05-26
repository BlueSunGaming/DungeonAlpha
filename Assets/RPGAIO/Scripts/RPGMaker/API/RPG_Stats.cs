using System.Linq;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;

namespace LogicSpawn.RPGMaker.API
{
    public static partial class RPG
    {
        public class Stats
        {
            public static Rm_UnityColors GetAttributeColorById(string id)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.FirstOrDefault(s => s.ID == id);
                return rmStatisticDefintion != null ? rmStatisticDefintion.Color : Rm_UnityColors.None;
            }
            public static Rm_UnityColors GetStatisticColorById(string id)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.FirstOrDefault(s => s.ID == id);
                return rmStatisticDefintion != null ? rmStatisticDefintion.Color : Rm_UnityColors.None;
            }
            public static Rm_UnityColors GetVitalColorById(string id)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.ASVT.VitalDefinitions.FirstOrDefault(s => s.ID == id);
                return rmStatisticDefintion != null ? rmStatisticDefintion.Color : Rm_UnityColors.None;
            }

            public static bool IsStatisticPercentageInUI(string statisticID)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.FirstOrDefault(s => s.ID == statisticID);
                return rmStatisticDefintion != null ? rmStatisticDefintion.IsPercentageInUI : false;
            }

            public static string GetAttributeName(string id)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.FirstOrDefault(s => s.ID == id);
                return rmStatisticDefintion != null ? rmStatisticDefintion.Name : null;
            }

            public static string GetStatisticName(string id)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.FirstOrDefault(s => s.ID == id);
                return rmStatisticDefintion != null ? rmStatisticDefintion.Name : null;
            }

            public static string GetVitalName(string id)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.ASVT.VitalDefinitions.FirstOrDefault(s => s.ID == id);
                return rmStatisticDefintion != null ? rmStatisticDefintion.Name : null;
            }

            public static string GetAttributeDesc(string id)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.FirstOrDefault(s => s.ID == id);
                return rmStatisticDefintion != null ? rmStatisticDefintion.Description : null;
            }

            public static string GetStatisticDesc(string id)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.FirstOrDefault(s => s.ID == id);
                return rmStatisticDefintion != null ? rmStatisticDefintion.Description : null;
            }

            public static string GetVitalDesc(string id)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.ASVT.VitalDefinitions.FirstOrDefault(s => s.ID == id);
                return rmStatisticDefintion != null ? rmStatisticDefintion.Description : null;
            }

            public static string GetTraitName(string id)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.ASVT.TraitDefinitions.FirstOrDefault(s => s.ID == id);
                return rmStatisticDefintion != null ? rmStatisticDefintion.Name : null;
            }
            public static Texture2D GetTraitImage(string id)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.ASVT.TraitDefinitions.FirstOrDefault(s => s.ID == id);
                return rmStatisticDefintion != null ? rmStatisticDefintion.Image : null;
            }

            public static string GetTraitDescription(string id)
            {
                var rmStatisticDefintion = Rm_RPGHandler.Instance.ASVT.TraitDefinitions.FirstOrDefault(s => s.ID == id);
                return rmStatisticDefintion != null ? rmStatisticDefintion.Description : null;
            }

            public static string GetStatisticId(string stat)
            {
                var statFound = Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.FirstOrDefault(s => s.Name == stat);

                return statFound != null ? statFound.ID : null;
            }

            public static string GetVitalId(string vit)
            {
                var vitFound = Rm_RPGHandler.Instance.ASVT.VitalDefinitions.FirstOrDefault(s => s.Name == vit);

                return vitFound != null ? vitFound.ID : null;
            }

            public static string GetAttributeId(string att)
            {
                var attFound = Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.FirstOrDefault(s => s.Name == att);

                return attFound != null ? attFound.ID : null;
            }

            public static string GetTraitId(string att)
            {
                var attFound = Rm_RPGHandler.Instance.ASVT.TraitDefinitions.FirstOrDefault(s => s.Name == att);

                return attFound != null ? attFound.ID : null;
            }

            public static string GetMetaId(string metaName)
            {
                var attFound = Rm_RPGHandler.Instance.Combat.SkillMeta.FirstOrDefault(s => s.Name == metaName);
                return attFound != null ? attFound.ID : null;
            }

            public static string GetMetaName(string id)
            {
                var attFound = Rm_RPGHandler.Instance.Combat.SkillMeta.FirstOrDefault(s => s.ID == id);
                return attFound != null ? attFound.Name : null;
            }

            public static string GetStatusEffectId(string metaName)
            {
                var attFound = Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects.FirstOrDefault(s => s.Name == metaName);
                return attFound != null ? attFound.ID : null;
            }

            public static string GetStatusEffectName(string id)
            {
                var attFound = Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects.FirstOrDefault(s => s.ID == id);
                return attFound != null ? attFound.Name : null;
            }

            public static StatusEffect GetStatusEffectById(string id)
            {
                var attFound = Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects.FirstOrDefault(s => s.ID == id);
                return attFound;
            }

            public static string GetSkillId(string skillName)
            {
                var attFound = Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.FirstOrDefault(s => s.Name == skillName);
                return attFound != null ? attFound.ID : null;
            }

            public static string GetSkillName(string id)
            {
                var attFound = Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.FirstOrDefault(s => s.ID == id);
                return attFound != null ? attFound.Name : null;
            }

            public static string GetReputationId(string repName)
            {
                var attFound = Rm_RPGHandler.Instance.Repositories.Quests.AllReputations.FirstOrDefault(s => s.Name == repName);
                return attFound != null ? attFound.ID : null;
            }

            public static string GetReputationName(string repId)
            {
                var attFound = Rm_RPGHandler.Instance.Repositories.Quests.AllReputations.FirstOrDefault(s => s.ID == repId);
                return attFound != null ? attFound.Name : null;
            }

            public static string GetMetaDataID(string metaDataName)
            {
                var attFound = Rm_RPGHandler.Instance.Player.MetaDataDefinitions.FirstOrDefault(s => s.Name == metaDataName);
                return attFound != null ? attFound.ID : null;
            }
            public static string GetMetaDataName(string metaDataId)
            {
                var attFound = Rm_RPGHandler.Instance.Player.MetaDataDefinitions.FirstOrDefault(s => s.ID == metaDataId);
                return attFound != null ? attFound.Name : null;
            }

            public static Rm_MetaDataDefinition GetMetaDataByID(string metaDataId)
            {
                var attFound = Rm_RPGHandler.Instance.Player.MetaDataDefinitions.FirstOrDefault(s => s.ID == metaDataId);
                return attFound != null ? attFound : null;
            }
        }
    }
}