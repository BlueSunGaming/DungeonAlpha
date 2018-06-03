using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class UserSave
    {
        public string ID;
        public Inventory Stash;
        public Dictionary<string, string> CustomData;

        public UserSave()
        {
            ID = Guid.NewGuid().ToString();
            Stash = new Inventory()
                        {
                            MaxItems = Rm_RPGHandler.Instance.Items.GlobalStashMaxItems,
                            MaxWeight = Rm_RPGHandler.Instance.Items.GlobalStashMaxWeight,
                            InventoryType = InventoryType.GlobalStash
                        };
            CustomData = new Dictionary<string, string>();
        }
    }

    public class PlayerSave
    {
        public string SaveID ;
        public string SaveName ;
        public string CurrentScene ;
        public PlayerCharacter Character ;
        public string Difficulty ;
        public PetData CurrentPet;
        public Inventory Stash ;
        public TimeSpan TimePlayed ;

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime CreationDate ;

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime LastSaved ;

        public GeneralLog GeneralLog ;
        public AchivementsLog AchivementsLog ;
        public QuestLog QuestLog ;

        public List<VisualCustomisation> VisualCustomisations;


        public GenericStats GenericStats ;
        public GamePersistence GamePersistence ;
        public WorldMap WorldMap ;
        public string SavePath;
        public Dictionary<string, string> CustomData;


        public PlayerSave(string saveName, string classNameId, string genderId, string raceId, string subraceId, string characterId, string charName, string startingScene)
        {
#if UNITY_WEBPLAYER
            SaveID = PlayerSaveLoadManager.Instance.NextID;
#else
            SaveID = Guid.NewGuid().ToString();
#endif
            LastSaved = CreationDate = DateTime.Now;

            SaveName = saveName;
            Character = new PlayerCharacter(genderId, classNameId, raceId, subraceId, characterId, charName);
            CurrentScene = startingScene;
            CurrentPet = null;
            //Initialize();
        }


        public void Initialize()
        {
            //Debug.Log("New Player");
            var classDef = Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == Character.PlayerCharacterID);
            Character.ExpDefinitionID = classDef.ExpDefinitionID;
            Character.AttackStyle = classDef.AttackStyle;
            Character.AnimationType = classDef.AnimationType;

            Difficulty = Rm_RPGHandler.Instance.Player.DefaultDifficultyID;
            GeneralLog = new GeneralLog();
            AchivementsLog = new AchivementsLog();
            QuestLog = new QuestLog();
            GenericStats = new GenericStats();
            TimePlayed = new TimeSpan();
            GamePersistence = new GamePersistence();
            WorldMap = new WorldMap();
            Stash = new Inventory()
            {
                MaxItems = Rm_RPGHandler.Instance.Items.CharacterStashMaxItems,
                MaxWeight = Rm_RPGHandler.Instance.Items.CharacterStashMaxWeight,
                InventoryType = InventoryType.CharacterStash
            };


            CustomData = new Dictionary<string, string>();

            


            Character.TalentHandler.Init(Character);
            Character.TalentHandler.LoadTalents();
            Character.SkillHandler.Init();
            QuestLog.Init();
            if (classDef.HasStartingQuest && !string.IsNullOrEmpty(classDef.StartingQuestID))
            {
                Debug.Log(classDef.HasStartingQuest);
                var startingQuest = QuestLog.AllObjectives.First(o => o.ID == classDef.StartingQuestID);
                BeginStartingQuest(startingQuest);
            }

            AchivementsLog.Init();

            Character.Init();


            Character.InitStatsFromNewSave();
            foreach (var vital in Character.Vitals)
            {
                vital.CurrentValue = vital.MaxValue;
                if(vital.AlwaysStartsAtZero)
                {
                    vital.CurrentValue = 0;
                }
            }
        }

        private void BeginStartingQuest(Quest quest)
        {
            quest.IsAccepted = true;
            foreach (var q in quest.AllConditions)
            {
                if (q == null) continue;

                var deliverCondition = q as DeliverCondition;
                if (deliverCondition != null)
                {
                    var item = Rm_RPGHandler.Instance.Repositories.QuestItems.Get(deliverCondition.ItemToDeliverID);
                    if (item != null)
                    {
                        Character.Inventory.AddItem(item);
                    }
                }
            }

            if (quest.HasTimeLimit)
            {
                quest.CurrentTimeLimit = quest.TimeLimit;
            }

            if (quest.HasBonusCondition && quest.BonusHasTimeLimit)
            {
                quest.BonusCurrentTimeLimit = quest.BonusTimeLimit;
            }
        }

        //TODO: add in API, along with OnPlayerCreation event where we can initialise stuff
        public T GetCustomData<T>(string key) where T : class
        {
            string jsonString;
            if (CustomData.TryGetValue(key, out jsonString))
            {
                var obj = (T)JsonConvert.DeserializeObject(jsonString, typeof (T));
                return obj;
            }

            return null;
        }
        
        public void SetCustomData(string key, object value)
        {
            var jsonString = JsonConvert.SerializeObject(value);
            CustomData.Add(key, jsonString);
        }

        public override string ToString()
        {
            var details = "";
            var classes = Rm_RPGHandler.Instance.Player.ClassNameDefinitions;
            var className = classes.FirstOrDefault(c => c.ID == Character.PlayerClassNameID);

            if(className == null)
            {
                details += Character.Name + ", Level " + Character.Level + " INVALID CLASSTYPE [" + CurrentScene + " - " + LastSaved.ToString("0:MM/dd/yy H:mm:ss") + "]";
            }
            else
            {
                details += Character.Name + ", Level " + Character.Level + " " + className.Name + " [" + CurrentScene + " - " + LastSaved.ToString("0:MM/dd/yy H:mm:ss") + "]";
            }
            //e.g. Looks like: Dextero Level 85 Warrior [Newbie Island - 06/10/11 15:24:16]
            return details;
        }

        public Rmh_CustomVariable GetCustomVariable(string variableID)
        {
            return GenericStats.CustomVariables.FirstOrDefault(v => v.ID == variableID);
        }
    }
}