using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.API
{
    public static partial class RPG
    {
        public static class Player
        {
            /// <summary>
            /// Get the string ID of the vital being used to represent health.
            /// </summary>
            public static string HealthVitalID
            {
                get { return GetObject.PlayerMono.Player.Vitals.First(a => a.IsHealth).ID; }
            }

            /// <summary>
            /// Get a class name by it's classNameId.
            /// </summary>
            /// <param name="classNameId">The ID of the class definition</param>
            /// <returns>Name of class</returns>
            public static string GetClassName(string classNameId)
            {
                var classNameDef = Rm_RPGHandler.Instance.Player.ClassNameDefinitions.FirstOrDefault(s => s.ID == classNameId);
                if (classNameDef != null)
                    return classNameDef.Name;

                return null;
            }

            /// <summary>
            /// Get a class description by it's classNameId.
            /// </summary>
            /// <param name="classNameId">The ID of the class definition</param>
            /// <returns>Name of class</returns>
            public static string GetClassDescription(string classNameId)
            {
                var classNameDef = Rm_RPGHandler.Instance.Player.ClassNameDefinitions.FirstOrDefault(s => s.ID == classNameId);
                if (classNameDef != null)
                    return classNameDef.Description;

                return null;
            }

            /// <summary>
            /// Get a character definition by id.
            /// </summary>
            /// <param name="id">The ID of the character definition</param>
            /// <returns>The character definition</returns>
            public static Rm_ClassDefinition GetCharacterDefinition(string id)
            {
                var classDef = Rm_RPGHandler.Instance.Player.CharacterDefinitions.FirstOrDefault(s => s.ID == id);
                return classDef;
            }

            /// <summary>
            /// Get a character definition by character name.
            /// </summary>
            /// <param name="name">The name of the character</param>
            /// <returns>The class definition</returns>
            public static Rm_ClassDefinition GetCharacterDefinitionByName(string name)
            {
                var classDef = Rm_RPGHandler.Instance.Player.CharacterDefinitions.FirstOrDefault(s => s.Name == name);
                return classDef;
            }

            /// <summary>
            /// Get a race  definition by Id.
            /// </summary>
            /// <param name="id">The ID of the race definition</param>
            /// <returns>The race definition</returns>
            public static Rm_RaceDefinition GetRaceDefinition(string id)
            {
                var def = Rm_RPGHandler.Instance.Player.RaceDefinitions.FirstOrDefault(s => s.ID == id);
                return def;
            }

            /// <summary>
            /// Get a subrace definition by Id.
            /// </summary>
            /// <param name="id">The ID of the subrace definition</param>
            /// <returns>The subrace definition</returns>
            public static Rm_SubRaceDefinition GetSubRaceDefinition(string id)
            {
                var def = Rm_RPGHandler.Instance.Player.SubRaceDefinitions.FirstOrDefault(s => s.ID == id);
                return def;
            }

            /// <summary>
            /// Get a class ID by it's name.
            /// </summary>
            /// <param name="name">The name of the class</param>
            /// <returns>ID of class</returns>
            public static string GetClassId(string name)
            {
                var classNameDef = Rm_RPGHandler.Instance.Player.ClassNameDefinitions.FirstOrDefault(s => s.Name == name);
                if (classNameDef != null)
                    return classNameDef.ID;

                return null;
            }

            /// <summary>
            /// Get's a list of all existing characters definitions in the game data.
            /// </summary>
            public static List<Rm_ClassDefinition> CharacterDefinitions
            {
                get { return Rm_RPGHandler.Instance.Player.CharacterDefinitions; }
            }

            /// <summary>
            /// Get's a list of all existing class name definitions in the game data.
            /// </summary>
            public static List<Rm_ClassNameDefinition> ClassNameDefinitions
            {
                get { return Rm_RPGHandler.Instance.Player.ClassNameDefinitions; }
            }

            /// <summary>
            /// Get's a list of all existing gender definitions in the game data.
            /// </summary>
            public static List<StringDefinition> GenderDefinitions
            {
                get { return Rm_RPGHandler.Instance.Player.GenderDefinitions; }
            }

            /// <summary>
            /// Get's a list of all existing race definitions in the game data.
            /// </summary>
            public static List<Rm_RaceDefinition> RaceDefinitions
            {
                get { return Rm_RPGHandler.Instance.Player.RaceDefinitions; }
            }

            /// <summary>
            /// Get's a list of all existing race definitions in the game data.
            /// </summary>
            public static List<Rm_SubRaceDefinition> SubRaceDefinitions
            {
                get { return Rm_RPGHandler.Instance.Player.SubRaceDefinitions; }
            }
        }
    }
}