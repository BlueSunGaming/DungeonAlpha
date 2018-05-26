using System.Linq;

namespace LogicSpawn.RPGMaker.API
{
    public static partial class RPG
    {
        public class Combat
        {
            public static Rm_UnityColors GetElementalColorById(string id)
            {
                var elementalDamageDefinition = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.FirstOrDefault(s => s.ID == id);
                return elementalDamageDefinition != null ? elementalDamageDefinition.Color : Rm_UnityColors.None;
            }
            public static string GetElementalNameById(string id)
            {
                var elementalDamageDefinition = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.FirstOrDefault(s => s.ID == id);
                return elementalDamageDefinition != null ? elementalDamageDefinition.Name : null;
            }

            public static string GetElementalIdByName(string name)
            {
                var elementalDamageDefinition = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.FirstOrDefault(s => s.Name == name);
                return elementalDamageDefinition != null ? elementalDamageDefinition.ID : null;
            }

            public static string GetSkillName(string skillId)
            {
                var skillName = Rm_RPGHandler.Instance.Repositories.Skills.Get(skillId);
                return skillName != null ? skillName.Name : null;
            }
        }
    }
}