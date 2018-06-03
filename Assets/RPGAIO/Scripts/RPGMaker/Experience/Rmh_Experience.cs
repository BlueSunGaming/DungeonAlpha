using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class Rmh_Experience
    {
        public List<ExpDefinition> CustomExpDefinitions;
        public ExpDefinition PlayerExpDefinition;
        public ExpDefinition TraitExpDefinition;
        public ExpDefinition MonsterExpDefinition;

        [JsonIgnore]
        public List<ExpDefinition> AllExpDefinitions
        {
            get { return (new[] {PlayerExpDefinition, TraitExpDefinition,MonsterExpDefinition}).Concat(CustomExpDefinitions).ToList(); }
        }

        public const string PlayerExpDefinitionID = "Player_Exp_Definition";
        public const string TraitExpDefinitionID = "Trait_Exp_Definition";
        public const string MonsterExpDefinitionID = "Monster_Exp_Definition";

        public int MaxPlayerLevel
        {
            get { return PlayerExpDefinition.MaxLevel; }
        }
        
        public Rmh_Experience()
        {
            PlayerExpDefinition = new ExpDefinition() { ID = PlayerExpDefinitionID, Name = "Player Exp Definition", IsDefault = true, ExpUse = ExpUse.PlayerLevelling };
            TraitExpDefinition = new ExpDefinition() { ID = TraitExpDefinitionID, Name = "Trait Exp Definition", IsDefault = true, ExpUse = ExpUse.TraitLevelling };
            MonsterExpDefinition = new ExpDefinition() { ID = MonsterExpDefinitionID, Name = "Monster Exp Gained Definition", IsDefault = true, ExpUse = ExpUse.ExpGained };
            CustomExpDefinitions = new List<ExpDefinition>();

        }

        public long ExpToNextPlayerLevel(string definitionId, int currentPlayerLevel)
        {

            var expDefinition = Rm_RPGHandler.Instance.Experience.AllExpDefinitions.FirstOrDefault(e => e.ID == definitionId);
            if (expDefinition != null)
            {
                return expDefinition.ExpForLevel(currentPlayerLevel);
            }

            return PlayerExpDefinition.ExpForLevel(currentPlayerLevel);
        }

        public long ExpToNextPlayerTraitLevel(string definitionId, int currentTraitLevel)
        {

            var expDefinition = Rm_RPGHandler.Instance.Experience.AllExpDefinitions.FirstOrDefault(e => e.ID == definitionId);
            if (expDefinition != null)
            {
                return expDefinition.ExpForLevel(currentTraitLevel);
            }

            throw new Exception("Exp Definition not found! ID: " + definitionId);
        }

        public ExpDefinition Get(string expDefinitionID)
        {
            var expDefinition = Rm_RPGHandler.Instance.Experience.AllExpDefinitions.FirstOrDefault(e => e.ID == expDefinitionID);
            return expDefinition;     
        }
    }

    public class ExpDefinition
    {
        public string ID;
        public string Name;
        public int MaxLevel;
        public long XpForFirstLevel; //not for linear OR LINEARRISE
        public long XpForLastLevel;
        public long Constant;
        public ExpType ExpType;
        public ExpUse ExpUse;
        public bool IsDefault;

        public ExpDefinition()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Exp Definition";
            MaxLevel = 25;
            XpForFirstLevel = 100;
            XpForLastLevel = 100000;
            Constant = 100;
            ExpType = ExpType.Logarithmic;
            ExpUse = ExpUse.PlayerLevelling;
            IsDefault = false;
        }

        public long ExpForLevel(int currentLevel)
        {
            Func<int, long> expForLevel;

            switch (ExpType)
            {
                case ExpType.Logarithmic:
                    expForLevel = ExpForLevelLogarithmic;
                    break;
                case ExpType.LinearRise:
                    expForLevel = ExpForLevelLinearRise;
                    break;
                case ExpType.Linear:
                    expForLevel = ExpForLevelLinear;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var x = expForLevel(currentLevel);
            var y = expForLevel(currentLevel - 1);
            var result = x - y;

            return result;
        }

        private long ExpForLevelLogarithmic(int currentLevel)
        {
            double B = Math.Log((double)XpForLastLevel / XpForFirstLevel) / (MaxLevel - 1);
            double A = (double)XpForFirstLevel / (Math.Exp(B) - 1.0);

            var x = (long)(A * Math.Exp((B) * currentLevel));
            var y = Math.Pow(10, (long)(Math.Log(x) / Math.Log(10) - 2.2));
            return (long)((long)(x / y) * y);
        }

        private long ExpForLevelLinearRise(int currentLevel)
        {
            var xpForFirst = XpForFirstLevel - Constant;

            currentLevel++;
            var experience = (long)((Math.Pow(currentLevel, 2) + currentLevel) / 2 * Constant - (currentLevel * Constant)) + (currentLevel * xpForFirst);
            return experience;
        }

        private long ExpForLevelLinear(int currentLevel)
        {
            long? s = new long();
            return Constant * currentLevel;
        }
    }

    public enum ExpType
    {
        Logarithmic,
        LinearRise,
        Linear
    }
    public enum ExpUse
    {
        PlayerLevelling,
        TraitLevelling,
        ExpGained
    }
}