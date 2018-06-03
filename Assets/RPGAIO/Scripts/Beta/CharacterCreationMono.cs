using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Beta.NewImplementation;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LogicSpawn.RPGMaker.Core
{
    public class CharacterCreationMono : MonoBehaviour
    {
        //Instance
        public static CharacterCreationMono Instance;

        private bool SkipCreation;
        private bool SkipRace;
        private bool SkipSubRace;
        private bool CustomisationEnabled;

        //UI References
        public GameObject RaceSelectUI;
        public GameObject MainCreationUI;
        public GameObject CustomisationUI;
        public GameObject MainCreationAndCustomisationUI;
        public GameObject SubRacePanel;
        public GameObject SubRaceInfoPanel;
        public InputField CharacterName;
        public Button CreateButton;
        public Text CreateButtonText;
        public Text ReturnButtonText;

        //Race Select
        public GameObject RaceInfo;
        public Text RaceName;
        public Text RaceDescription;
        public GameObject RaceContainer;
        public GameObject RaceSelectPrefab;

        //Sub Race Select
        public GameObject SubRaceContainer;
        public GameObject SubRaceSelectPrefab;
        public Text SubRaceTitle;
        public Text SubRaceDescription;

        //Metadata Select
        public GameObject NameSubRaceGenderPanel;
        public GameObject MetaDataPrefab;
        
        //Character Select
        public GameObject CharacterContainer;
        public GameObject CharacterSelectPrefab;
        public Text ClassNameLabel;
        public string ClassNameLabelPrefix;
        public Text CharacterDescription;
        public GameObject AttributeValueContainer;
        public GameObject AttributeValuePrefab;
        
        //Selected Class Genders
        private List<Rm_ClassDefinition> _genderCharactersAvailable;
        private int _selectedGenderCharacter;
        public Text GenderLabel;
        public Button NextGenderButton;
        public Button PrevGenderButton;

        private Dictionary<string, Rm_ClassDefinition> CachedRaceCharacters;

        //Customisation
        public Text SummaryCharacterName;
        public Text SummaryCharacterRaces;
        public Text SummaryCharacterClass;
        public GameObject VisualCustCategoryPrefab;
        public GameObject VisualCustomisationContainer;
        public GameObject VisualCustSliderPrefab;
        public GameObject VisualCustColorPrefab;
        public GameObject VisualCustTextPrefab;
        public GameObject VisualCustImagePrefab;
        public GameObject VisualCustTextListPrefab;

        //State
        private bool InRaceSelect = true;
        private bool InCustomisation = false;

        //Camera
        public Transform Camera;
        public Vector3 RaceSelectCameraPosition;
        public Vector3 MainCreationCameraPosition;
        public Vector3 ZoomOnePosition;
        public Vector3 ZoomTwoPosition;
        private int _currentZoom;
        private bool _rotateRightOn;
        private bool _rotateLeftOn;

        //Spawned Character
        public GameObject MainCreationSpawnPosition;
        public GameObject SpawnedCharacter;

        //Save Info
        public string SaveName;
        public string SaveRaceID;
        public string SaveSubRaceID;
        public string SaveCharacterID;
        public string SaveClassID;
        public string SaveGenderID;
        public Dictionary<string, string> MetaDataValues;


        //public GameObject ClassContainer;
        //public GameObject ClassSelectPrefab;
        //public GameObject SpawnPosition;
        //public GameObject SpawnedClass;
        //public InputField CharacterNameTextField;
        //public Text SelectedClassName;
        //public PlayerSave PlayerToCreate;
        //public string ClassID;
        //public string RaceID;
        //public string SubraceID;
        //public string GenderID;
        //public string ClassNameID;
        //public string charName;

        void Awake()
        {
            Time.timeScale = 1;

            SkipCreation = Rm_RPGHandler.Instance.Player.SkipCharacterCreation;
            SkipRace = Rm_RPGHandler.Instance.Player.SkipRaceSelection;
            SkipSubRace = Rm_RPGHandler.Instance.Player.SkipSubRaceSelection;
            CustomisationEnabled = Rm_RPGHandler.Instance.Player.CustomisationsEnabled;

            Instance = this;
            SpawnedCharacter = null;
            CachedRaceCharacters = new Dictionary<string, Rm_ClassDefinition>();
            SaveName = "Player_Save" + Random.Range(0, 999);

            _genderCharactersAvailable = new List<Rm_ClassDefinition>();

            RaceSelectUI.SetActive(false);
            MainCreationUI.SetActive(false);
            MainCreationAndCustomisationUI.SetActive(false);
            CustomisationUI.SetActive(false);

            MetaDataValues = new Dictionary<string, string>();

            Init();
        }

        private void Init()
        {
            //Initialise Some Save Data
            SaveGenderID = RPG.Player.GenderDefinitions.First().ID;

            foreach (var metaDataDef in Rm_RPGHandler.Instance.Player.MetaDataDefinitions)
            {
                var go = Instantiate(MetaDataPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go.transform.SetParent(NameSubRaceGenderPanel.transform, false);
                var metaDataSelectModel = go.GetComponent<MetaDataSelectModel>();
                metaDataSelectModel.Init(metaDataDef);

                MetaDataValues.Add(metaDataDef.ID, metaDataDef.Values[0].ID);
                SetMetaData(metaDataDef, 0);
            }

            //If Skipping Creation

            if (SkipCreation)
            {

                SaveRaceID = RPG.Player.RaceDefinitions[0].ID;
                SaveSubRaceID = RPG.Player.SubRaceDefinitions[0].ID;
                SaveCharacterID = RPG.Player.CharacterDefinitions[0].ID;
                SaveClassID = RPG.Player.ClassNameDefinitions[0].ID;
                SaveName = "";
                CreateAndSaveCharacter();
                return;
            }

            //Initialise the rest

            if(!SkipRace)
            {
                //Spawn UI buttons
                RaceContainer.transform.DestroyChildren();
                foreach (var raceDefinition in RPG.Player.RaceDefinitions)
                {
                    var go = Instantiate(RaceSelectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.SetParent(RaceContainer.transform, false);
                    var raceSelect = go.GetComponent<RaceSelectModel>();
                    raceSelect.RaceID = raceDefinition.ID;
                                
                    if(raceSelect.ButtonText != null)
                        raceSelect.ButtonText.text = raceDefinition.Name;
                
                    if(raceSelect.ButtonImage != null)
                        raceSelect.ButtonImage.sprite = GeneralMethods.CreateSprite(raceDefinition.Image.Image);
                }

                //Show first model then hide text
                ShowRace(RPG.Player.RaceDefinitions[0].ID);
                ShowRace(null);

                //Set State
                InRaceSelect = true;

                //Show UI
                RaceSelectUI.SetActive(true);
            }
            else
            {
                //Handle Skip Race (Show MainCreationUI)
                SaveRaceID = RPG.Player.RaceDefinitions[0].ID;
                InRaceSelect = false;
                SwitchToMainCreation();
            }


            
        }

        void Update()
        {
            if(InRaceSelect)
            {
                Camera.transform.position = RaceSelectCameraPosition;
            }
            else
            {
                switch(_currentZoom)
                {
                    case 0:
                        Camera.transform.position = MainCreationCameraPosition;
                        break;
                    case 1:
                        Camera.transform.position = ZoomOnePosition;
                        break;
                    case 2:
                        Camera.transform.position = ZoomTwoPosition;
                        break;
                    default:
                        _currentZoom = 0;
                        break;
                }
            }


            CreateButton.interactable = !string.IsNullOrEmpty(CharacterName.text);

            if(_rotateRightOn)
            {
                //Debug.Log("Rotating");
                SpawnedCharacter.transform.Rotate(0, -360 * Time.deltaTime, 0);
            }
            if(_rotateLeftOn)
            {
                SpawnedCharacter.transform.Rotate(0, 360 * Time.deltaTime, 0);
            }
        }

        public void ContinueOrCreate()
        {
            if(CustomisationEnabled && !InCustomisation)
            {
                SwitchToCustomisation();
            }
            else
            {
                CreateAndSaveCharacter();
            }
        }

        private void CreateAndSaveCharacter()
        {
            var classDef = Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == SaveCharacterID);
            
            var startingScene = classDef.StartingScene;
            var startingWorldArea = classDef.StartingWorldArea;
            var startingLocation = classDef.StartingLocation;

            var playerToCreate = new PlayerSave(SaveName, SaveClassID, SaveGenderID, SaveRaceID, SaveSubRaceID, SaveCharacterID, CharacterName.text, startingScene);
            playerToCreate.Initialize();

            if(classDef.HasStartingPet)
            {
                var petDef = Rm_RPGHandler.Instance.Player.PetDefinitions.FirstOrDefault(p => p.ID == classDef.StartingPet);
                playerToCreate.CurrentPet = new PetData(petDef);
            }
            playerToCreate.WorldMap.CurrentWorldAreaID = startingWorldArea;
            playerToCreate.WorldMap.CurrentLocationID = startingLocation;
            
            if (classDef.StartAtWorldLocation)
            {
                var locationScene =
                    Rm_RPGHandler.Instance.Customise.WorldMapLocations.First(w => w.ID == startingWorldArea).Locations.
                        First(l => l.ID == startingLocation).SceneName;
                playerToCreate.CurrentScene = locationScene;
            }
            
            //Save MetaData
            foreach(var metaData in MetaDataValues)
            {
                var newMetaData = new MetaDataInfo();
                newMetaData.ID = metaData.Key;
                newMetaData.ValueID = metaData.Value;

                playerToCreate.Character.MetaData.Add(newMetaData);
            }

            //Save visual customisations
            var allCustomisations = FindObjectsOfType<VisualCustomiser>();
            var customisations = allCustomisations.Select(e => e.VisualCustomisation);
            playerToCreate.VisualCustomisations = GeneralMethods.CopyObject(customisations).ToList();

            //Save and Load the player
            PlayerSaveLoadManager.Instance.Save(playerToCreate,true);
            RPG.LoadLevel(playerToCreate.CurrentScene,false,false);
        }

        public void ReturnOrGoBack()
        {
            if(InCustomisation)
            {
                SwitchToMainCreation();
            }
            else if(!InRaceSelect && !Rm_RPGHandler.Instance.Player.SkipRaceSelection)
            {
                SwitchToRaceSelect();
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        private void SwitchToCustomisation()
        {
            var character = RPG.Player.GetCharacterDefinition(SaveCharacterID);

            CustomisationUI.SetActive(true);

            CustomisationUI.transform.GetChild(0).gameObject.SetActive(character.VisualCustomisations.Count > 0);

            RaceSelectUI.SetActive(false);
            MainCreationUI.SetActive(false);
            MainCreationAndCustomisationUI.SetActive(true);

            //Bug workaround
            var containerParent = VisualCustomisationContainer.transform.parent;
            var imageParent = containerParent.GetComponent<Image>();
            if(imageParent != null)
            {
                imageParent.enabled = false;
                imageParent.enabled = true;
            }

            //Set summary labels
            if(SkipRace && SkipSubRace)
            {
                if(SummaryCharacterRaces != null)
                {
                    Destroy(SummaryCharacterRaces);
                    SummaryCharacterRaces = null;
                }
            }

            SummaryCharacterName.text = CharacterName.text;
            SummaryCharacterClass.text = RPG.Player.GetClassName(SaveClassID);

            if(!SkipRace)
            {
                SummaryCharacterRaces.text = RPG.Player.GetRaceDefinition(SaveRaceID).Name + " ";
            }

            if(!SkipSubRace)
            {
                SummaryCharacterRaces.text += RPG.Player.GetSubRaceDefinition(SaveSubRaceID).Name;
            }

            //Spawn in all customisation groups
            StartCoroutine(InitiateCustom(SpawnedCharacter, character));

            //End of spawn in

            CreateButtonText.text = "Create";
            ReturnButtonText.text = "Back";

            InRaceSelect = false;
            InCustomisation = true;
        }

        private void InitiateCustomisations(Rm_ClassDefinition character)
        {
            if (!Rm_RPGHandler.Instance.Player.CustomisationsEnabled) return;

            VisualCustomisationContainer.transform.DestroyChildren();
            foreach (var visual in character.VisualCustomisations)
            {
                if (visual.CustomisationType == VisualCustomisationType.Category)
                {
                    var go = Instantiate(VisualCustCategoryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.SetParent(VisualCustomisationContainer.transform, false);
                    go.GetComponentInChildren<Text>().text = visual.Identifier;
                }
                else if (visual.DisplayType == VisualDisplayType.Slider)
                {
                    var go = Instantiate(VisualCustSliderPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.SetParent(VisualCustomisationContainer.transform, false);
                    var subRaceSelect = go.GetComponent<VisualCustomisationSliderModel>();
                    subRaceSelect.Init(visual);
                }
                else if (visual.DisplayType == VisualDisplayType.Color)
                {
                    var go = Instantiate(VisualCustColorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.SetParent(VisualCustomisationContainer.transform, false);
                    var subRaceSelect = go.GetComponent<VisualCustomisationColorsModel>();
                    subRaceSelect.Init(visual);
                }
                else if (visual.DisplayType == VisualDisplayType.TextOptions)
                {
                    var go = Instantiate(VisualCustTextPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.SetParent(VisualCustomisationContainer.transform, false);
                    var subRaceSelect = go.GetComponent<VisualCustomisationTextsModel>();
                    subRaceSelect.Init(visual);
                }
                else if (visual.DisplayType == VisualDisplayType.ImageOptions)
                {
                    var go = Instantiate(VisualCustImagePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.SetParent(VisualCustomisationContainer.transform, false);
                    var subRaceSelect = go.GetComponent<VisualCustomisationImagesModel>();
                    subRaceSelect.Init(visual);
                }
                else if (visual.DisplayType == VisualDisplayType.TextList)
                {
                    var go = Instantiate(VisualCustTextListPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.SetParent(VisualCustomisationContainer.transform, false);
                    var subRaceSelect = go.GetComponent<VisualCustomisationTextListModel>();
                    subRaceSelect.Init(visual);
                }
            }
        }

        private void SwitchToMainCreation()
        {
            RaceSelectUI.SetActive(false);
            CustomisationUI.SetActive(false);
            MainCreationUI.SetActive(true);
            MainCreationAndCustomisationUI.SetActive(true);

            //Spawn a character if null and move to correct position
            if (SpawnedCharacter == null)
            {
                SpawnCharacter(RPG.Player.CharacterDefinitions[0]);
            }

            SpawnedCharacter.transform.position = MainCreationSpawnPosition.transform.position -
                                                      new Vector3(0, 1, 0);
            SpawnedCharacter.transform.rotation = MainCreationSpawnPosition.transform.rotation;

            //Remove the subrace panel if we are ignoring it
            if(SkipSubRace)
            {
                if(SubRacePanel != null)
                {
                    Destroy(SubRacePanel);
                    if(Rm_RPGHandler.Instance.Player.RemoveSubRaceDescription)
                    {
                        Destroy(SubRaceInfoPanel);
                    }
                    SubRacePanel = null;
                    SubRaceInfoPanel = null;
                }
            }


            var applicableSubRaces = RPG.Player.SubRaceDefinitions.Where(s => s.ApplicableRaceID == SaveRaceID).ToList();

            //Spawn subraceicons and select first if we are not skipping subrace
            if(!SkipSubRace)
            {
                SubRaceContainer.transform.DestroyChildren();
                foreach (var subRaceDefinition in applicableSubRaces)
                {
                    var applicableCharacters = RPG.Player.CharacterDefinitions.Where(
                                s =>
                                s.ApplicableRaceID == SaveRaceID && s.ApplicableSubRaceID == subRaceDefinition.ID).ToList();

                    if (applicableCharacters.Count == 0)
                    {
                        Debug.LogWarning("[RPGAIO] Cannot find any characters for the sub-race [" + subRaceDefinition.Name + "]. Will not add an icon for to select it.");
                        continue;
                    }

                    var go = Instantiate(SubRaceSelectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.SetParent(SubRaceContainer.transform, false);
                    var subRaceSelect = go.GetComponent<SubRaceSelectModel>();
                    subRaceSelect.SubRaceID = subRaceDefinition.ID;

                    if (subRaceSelect.ButtonImage != null)
                        subRaceSelect.ButtonImage.sprite = GeneralMethods.CreateSprite(subRaceDefinition.Image.Image);
                }
            }
            
            //Set subrace to first option to populate class select container
            SetSubRace(applicableSubRaces.First().ID);

            CreateButtonText.text = CustomisationEnabled ? "Continue" : "Create";
            ReturnButtonText.text = SkipRace ? "Return To Main Menu" : "Back";

            InRaceSelect = false;
            InCustomisation = false;
        }

        public void SwitchToRaceSelect()
        {
            CustomisationUI.SetActive(false);
            RaceSelectUI.SetActive(true);
            MainCreationUI.SetActive(false);
            MainCreationAndCustomisationUI.SetActive(false);

            if (SpawnedCharacter == null)
            {
                SpawnCharacter(RPG.Player.CharacterDefinitions[0]);
            }

            SpawnedCharacter.transform.position = MainCreationSpawnPosition.transform.position -
                                                      new Vector3(0, 1, 0);
            SpawnedCharacter.transform.rotation = MainCreationSpawnPosition.transform.rotation;

            _currentZoom = 0;

            ReturnButtonText.text = "Return To Main Menu";

            InRaceSelect = true;
            InCustomisation = false;
        }

        public void SetRace(string raceId)
        {
            SaveRaceID = raceId;
            SwitchToMainCreation();
        }

        public void SetSubRace(string subRaceId)
        {
            SaveSubRaceID = subRaceId;
            
            //Spawn available characters and select first
            CharacterContainer.transform.DestroyChildren();
            var applicableCharacters = RPG.Player.CharacterDefinitions.Where(
                s =>
                s.ApplicableRaceID == SaveRaceID && s.ApplicableSubRaceID == SaveSubRaceID).ToList();

            bool firstApplicableCharacterSet = false;
            Rm_ClassDefinition firstCharacter = null;
            Rm_ClassNameDefinition firstClass = null;
            //For each class that is within applicable characters, spawn class icon with details of the applicable character
            //Note: We are using both character and class definition details
            foreach(var classDef in RPG.Player.ClassNameDefinitions)
            {
                var applicableCharacter =
                    applicableCharacters.FirstOrDefault(c => c.ApplicableClassIDs.Any(a => a.ID == classDef.ID));
                if(applicableCharacter != null)
                {

                    if(!firstApplicableCharacterSet)
                    {
                        firstCharacter = applicableCharacter;
                        firstClass = classDef;
                        firstApplicableCharacterSet = true;
                    }

                    var go = Instantiate(CharacterSelectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.SetParent(CharacterContainer.transform, false);
                    var classSelectModel = go.GetComponent<CharacterSelectModel>();
                    classSelectModel.ClassNameID = classDef.ID;
                    classSelectModel.CharacterID = applicableCharacter.ID;

                    if (classSelectModel.ButtonImage != null)
                        classSelectModel.ButtonImage.sprite = GeneralMethods.CreateSprite(classDef.Image.Image);
                }
            }

            if(firstCharacter == null)
            {
                throw new Exception("[RPGAIO] Could not find any character's for this subrace. Please check at least one character falls into this category.");
            }

            //Set descriptions and titles
            if(SubRaceInfoPanel != null)
            {
                var subRaceDefinition = RPG.Player.GetSubRaceDefinition(subRaceId);
                SubRaceTitle.text = subRaceDefinition.Name;
                SubRaceDescription.text = subRaceDefinition.Description;
            }

            SetCharacter(firstClass.ID, firstCharacter.ID);
        }

        public void SetCharacter(string classNameId, string characterId)
        {
            Destroy(SpawnedCharacter);
            SpawnCharacter(RPG.Player.GetCharacterDefinition(characterId));

            SaveCharacterID = characterId;
            SaveClassID = classNameId;

            var charDefinition = RPG.Player.GetCharacterDefinition(characterId);
            //Load up to 8 starting  attributes
            AttributeValueContainer.transform.DestroyChildren();
            foreach (var attr in charDefinition.StartingAttributes)
            {
                var go = Instantiate(AttributeValuePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go.transform.SetParent(AttributeValueContainer.transform, false);
                var attrValue = go.GetComponent<AttributeValueModel>();
                attrValue.ValueText.text = RPG.Stats.GetAttributeName(attr.AsvtID) + " " + attr.Amount;
            }

            //Set gender
            SaveGenderID = charDefinition.ApplicableGenderID;
            GenderLabel.text = RPG.Player.GenderDefinitions.First(g => g.ID == SaveGenderID).Name;

            //Check if same class exists with different genders (add to list)
            var sameClassCharacters = RPG.Player.CharacterDefinitions.FindAll(
                    c => c.ApplicableRaceID == SaveRaceID && c.ApplicableSubRaceID == SaveSubRaceID
                         && c.ApplicableClassIDs.FirstOrDefault(a => a.ID == classNameId) != null);

            _genderCharactersAvailable = sameClassCharacters;

            _selectedGenderCharacter = _genderCharactersAvailable.IndexOf(charDefinition);

            //Enable buttons if more than one gender is available.
            NextGenderButton.interactable = _genderCharactersAvailable.Count > 1;
            PrevGenderButton.interactable = _genderCharactersAvailable.Count > 1;

            //Set descriptions and titles
            ClassNameLabel.text = ClassNameLabelPrefix + " " + RPG.Player.GetClassName(classNameId);
            
            switch (Rm_RPGHandler.Instance.Player.ShownDescription)
            {
                case DescriptionShown.ClassDescription:
                    CharacterDescription.text = RPG.Player.GetClassDescription(classNameId);    
                    break;
                case DescriptionShown.CharacterDescription:
                    CharacterDescription.text = charDefinition.Description;
                    break;
                case DescriptionShown.Both:
                    CharacterDescription.text = charDefinition.Description + "\n\n" +
                                                RPG.Player.GetClassDescription(classNameId);
                    break;
                case DescriptionShown.None:
                    CharacterDescription.text = "";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void NextGender()
        {
            _selectedGenderCharacter++;
            if (_selectedGenderCharacter + 1 > _genderCharactersAvailable.Count)
            {
                _selectedGenderCharacter = 0;
            }
            
            //The class is the same as the current one
            SetCharacter(SaveClassID, _genderCharactersAvailable[_selectedGenderCharacter].ID);
        }

        public void PrevGender()
        {
            _selectedGenderCharacter--;
            if (_selectedGenderCharacter < 0)
            {
                _selectedGenderCharacter = _genderCharactersAvailable.Count - 1;
            }

            //The class is the same as the current one
            SetCharacter(SaveClassID, _genderCharactersAvailable[_selectedGenderCharacter].ID);
        }

        public void ShowRace(string raceId)
        {
            if(string.IsNullOrEmpty(raceId))
            {
                RaceInfo.SetActive(false);
                return;
            }

            RaceInfo.SetActive(true);
            var race = RPG.Player.GetRaceDefinition(raceId);
            RaceName.text = race.Name;
            RaceDescription.text = race.Description;

            //Cache that races first available character
            if(!CachedRaceCharacters.ContainsKey(raceId))
            {
                var firstChar = RPG.Player.CharacterDefinitions.First(c => c.ApplicableRaceID == raceId);
                CachedRaceCharacters.Add(raceId, firstChar);
            }

            var character = CachedRaceCharacters[raceId];

            Destroy(SpawnedCharacter);

            SpawnCharacter(character);
        }

        private void SpawnCharacter(Rm_ClassDefinition character)
        {
            SpawnedCharacter = (GameObject)Instantiate(Resources.Load(character.ClassPrefabPath), MainCreationSpawnPosition.transform.position - new Vector3(0, 1, 0), MainCreationSpawnPosition.transform.rotation);
            var comp = SpawnedCharacter.GetComponents(typeof(MonoBehaviour));
            var model = SpawnedCharacter.GetComponent<RPGController>().CharacterModel;
            foreach (var com in comp)
            {
                Destroy(com);
            }
            Destroy(SpawnedCharacter.GetComponent<NavMeshAgent>());
            Destroy(SpawnedCharacter.GetComponent<NavMeshObstacle>());
            Destroy(SpawnedCharacter.GetComponent<CapsuleCollider>());
            Destroy(SpawnedCharacter.GetComponent<CharacterController>());
            Destroy(SpawnedCharacter.transform.GetChild("TargetLock"));

            //Wait for end of frame and apply default customisations
            StartCoroutine(InitiateCustom(SpawnedCharacter, character));

            //Play Idle Animation
            var modelAnim = model.GetComponent<Animation>();
            var animator = model.GetComponent<Animator>();

            if (modelAnim != null)
            {
                var idleAnimation = string.IsNullOrEmpty(character.LegacyAnimations.CombatIdleAnim.Animation)
                    ? character.LegacyAnimations.CombatIdleAnim
                    : character.LegacyAnimations.IdleAnim;

                if (modelAnim.GetClip(idleAnimation.Animation) != null)
                {
                    modelAnim[idleAnimation.Animation].wrapMode = WrapMode.Loop;
                    modelAnim.CrossFade(idleAnimation.Animation);
                }
            }
            else if (animator != null)
            {
                animator.SetFloat("speed", 0);
                animator.SetBool("moving", false);
                animator.SetBool("strafing", false);
                animator.SetBool("falling", false);
                animator.SetBool("idle", true);
                animator.SetBool("combatIdle", false);
                animator.SetBool("jumping", false);
            }
        }

        private IEnumerator InitiateCustom(GameObject spawnedCharacter, Rm_ClassDefinition character)
        {
            yield return new WaitForEndOfFrame();
            //Initiate customisations
            if(spawnedCharacter != null)
                InitiateCustomisations(character);
        }

        public void ZoomIn()
        {
            _currentZoom++;
            if(_currentZoom > 2)
            {
                _currentZoom = 2;
            }
        }

        public void ZoomOut()
        {
            _currentZoom--;
            if (_currentZoom < 0)
            {
                _currentZoom = 0;
            }
        }

        public void RotateLeft()
        {
            _rotateLeftOn = !_rotateLeftOn;
            _rotateRightOn= false;
        }

        public void RotateRight()
        {
            _rotateRightOn = !_rotateRightOn;
            _rotateLeftOn = false;
        }

        public void SetMetaData(Rm_MetaDataDefinition metaDataDefinition, int selectedOption)
        {
            MetaDataValues[metaDataDefinition.ID] = metaDataDefinition.Values[selectedOption].ID;
        }
    }

//    public class CharacterCreationMono : MonoBehaviour
//    {
//        public static CharacterCreationMono Instance;
//        public GameObject ClassContainer;
//        public GameObject ClassSelectPrefab;
//        public GameObject SpawnPosition;
//        public GameObject SpawnedClass;
//        public Button CreateButton;
//        public InputField CharacterNameTextField;
//        public Text SelectedClassName;
//        public PlayerSave PlayerToCreate;
//        public string SaveName;
//        public string ClassID;
//        public string RaceID;
//        public string SubraceID;
//        public string GenderID;
//        public string ClassNameID;
//        public string charName;
//
//        void Awake()
//        {
//            Instance = this;
//            SaveName = "Test_Save_" + Random.Range(0, 999);
//            ClassID = "";
//            charName = "Character";
//
//            CreateClassButtons();
//        }
//
//        void Start()
//        {
//            SetClass(RPG.Player.CharacterDefinitions[0].ID);
//
//            //Set theme sound level
//            var themeObj = GameObject.Find("CharacterCreateTheme");
//            if(themeObj != null)
//            {
//                themeObj.AddComponent<Audio_BgMusic>();
//            }
//        }
//
//        private void CreateClassButtons()
//        {
//            ClassContainer.transform.DestroyChildren();
//            foreach (var playerClass in RPG.Player.CharacterDefinitions)
//            {
//                var go = Instantiate(ClassSelectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
//                go.transform.SetParent(ClassContainer.transform, false);
//                var classSelect = go.GetComponent<ClassSelectModel>();
//                classSelect.ClassID = playerClass.ID;
//                
//                if(classSelect.ButtonText != null)
//                    classSelect.ButtonText.text = playerClass.Name;
//
//                if(classSelect.ButtonImage != null)
//                    classSelect.ButtonImage.sprite = GeneralMethods.CreateSprite(playerClass.Image);
//            }
//        }
//
//        public void ReturnToMainMenu()
//        {
//            SceneManager.LoadScene("MainMenu");
//        }
//
//        public void CreatePlayer()
//        {
//            var classDef = Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == ClassID);
//
//            var startingScene = classDef.StartingScene;
//            var startingWorldArea = classDef.StartingWorldArea;
//            var startingLocation = classDef.StartingLocation;
//
//            PlayerToCreate = new PlayerSave(SaveName,ClassNameID, GenderID, RaceID, SubraceID, ClassID, charName, startingScene);
//            PlayerToCreate.Initialize();
//            if(classDef.HasStartingPet)
//            {
//                var petDef = Rm_RPGHandler.Instance.Player.PetDefinitions.FirstOrDefault(p => p.ID == classDef.StartingPet);
//                PlayerToCreate.CurrentPet = new PetData(petDef);
//            }
//            PlayerToCreate.WorldMap.CurrentWorldAreaID = startingWorldArea;
//            PlayerToCreate.WorldMap.CurrentLocationID = startingLocation;
//
//            if (classDef.StartAtWorldLocation)
//            {
//                var locationScene =
//                    Rm_RPGHandler.Instance.Customise.WorldMapLocations.First(w => w.ID == startingWorldArea).Locations.
//                        First(l => l.ID == startingLocation).SceneName;
//                PlayerToCreate.CurrentScene = locationScene;
//            }
//
//            PlayerSaveLoadManager.Instance.Save(PlayerToCreate,true);
//            RPG.LoadLevel(PlayerToCreate.CurrentScene,false,false);
//        }
//        
//        void Update()
//        {
//            CreateButton.interactable = !string.IsNullOrEmpty(charName) && !string.IsNullOrEmpty(ClassID);
//            charName = CharacterNameTextField.text;
//        }
//
//        public void SetClass(string classId)
//        {
//            Destroy(SpawnedClass);
//            var classes = RPG.Player.CharacterDefinitions;
//            var newClass = classes.First(c => c.ID == classId);
//
//            SelectedClassName.text = newClass.Name;
//            ClassID = classId;
//            SpawnedClass = null;
//            SpawnedClass = (GameObject)Instantiate(Resources.Load(newClass.ClassPrefabPath), SpawnPosition.transform.position - new Vector3(0,1,0), SpawnPosition.transform.rotation);
//            var comp = SpawnedClass.GetComponents(typeof (MonoBehaviour));
//            var model = SpawnedClass.GetComponent<RPGController>().CharacterModel;
//            var idleAnimation = newClass.LegacyAnimations.CombatIdleAnim;
//            foreach(var com in comp)
//            {
//                Destroy(com);
//            }
//            Destroy(SpawnedClass.GetComponent<NavMeshAgent>());
//            Destroy(SpawnedClass.GetComponent<NavMeshObstacle>());
//            Destroy(SpawnedClass.GetComponent<CapsuleCollider>());
//            Destroy(SpawnedClass.GetComponent<CharacterController>());
//
//            var modelAnim = model.GetComponent<Animation>();
//            if(modelAnim != null)
//            {
//                modelAnim[idleAnimation.Animation].wrapMode = WrapMode.Loop;
//                modelAnim.CrossFade(idleAnimation.Animation);
//            }
//        }
//    }
}