using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class PlayerMono : BaseCharacterMono
    {
        public PlayerCharacter Player;

        private PlayerSave _playerSave;

        public PlayerSave PlayerSave
        {
            get { return _playerSave; }
            set { _playerSave = value; }
        }

        protected override void DoUpdate()
        {

        }

        protected override void DoStart()
        {
            var playerPos = GetObject.PlayerMonoGameObject.transform;
            PetMono.SpawnPet(GetObject.PlayerSave.CurrentPet, playerPos.position - playerPos.forward);

            ApplyVisualCustomisations();

            foreach(var vital in Player.Vitals)
            {
                if(vital.AlwaysStartsAtZero)
                {
                    vital.CurrentValue = 0;
                }
            }
        }

        private void ApplyVisualCustomisations()
        {
            foreach(var visual in GetObject.PlayerSave.VisualCustomisations)
            {
                switch(visual.CustomisationType)
                {
                    case VisualCustomisationType.BlendShape:
                        if(true)
                        {
                            var target = FindTargetObject(visual.TargetedGameObjectName);
                            var targetSkinnedMeshRendr = target.GetComponent<SkinnedMeshRenderer>();
                            var intRef = targetSkinnedMeshRendr.sharedMesh.GetBlendShapeIndex(visual.StringRef);
                            targetSkinnedMeshRendr.SetBlendShapeWeight(intRef, visual.SavedFloatValue);;
                        }
                        break;
                    case VisualCustomisationType.Scale:
                        if(true)
                        {
                            var target = FindTargetObject(visual.TargetedGameObjectName);
                            var localScale = target.transform.localScale;
                            var scale = new Vector3(visual.ScaleX ? visual.SavedFloatValue : localScale.x,
                                                    visual.ScaleY ? visual.SavedFloatValue : localScale.y,
                                                    visual.ScaleZ ? visual.SavedFloatValue : localScale.z);
                            target.transform.localScale = scale;

                            if (visual.ChildCustomisations.Count > 0)
                            {

                                var minValue = visual.MinFloatValue;
                                var maxValue = visual.MaxFloatValue;

                                var difference = maxValue - minValue;

                                var ratio = (visual.SavedFloatValue - minValue) / difference;

                                foreach (var childCustomisation in visual.ChildCustomisations)
                                {
                                    var minChildValue = childCustomisation.MinFloatValue;
                                    var maxChildValue = childCustomisation.MaxFloatValue;

                                    var childDifference = maxChildValue - minChildValue;

                                    var childValue = minChildValue + (ratio * childDifference);



                                    var localChildGameObject = FindTargetObject(childCustomisation.TargetedGameObjectName);
                                    var localChildScale = localChildGameObject.transform.localScale;
                                    var childScale = new Vector3(childCustomisation.ScaleX ? childValue : localChildScale.x,
                                                            childCustomisation.ScaleY ? childValue : localChildScale.y,
                                                            childCustomisation.ScaleZ ? childValue : localChildScale.z);
                                    localChildGameObject.transform.localScale = childScale;
                                }
                            }
                        }
                        break;
                    case VisualCustomisationType.GameObject:
                        if(true)
                        {
                            var targets = new List<GameObject>();
                            visual.TargetedGameObjectNames.ForEach(t => targets.Add(FindTargetObject(t)));
                            targets.ForEach(m =>
                            {
                                if (m != null)
                                {
                                    m.SetActive(false);
                                }
                            });

                            if (!string.IsNullOrEmpty(visual.SavedStringValue))
                            {
                                targets.First(t => t.name == visual.SavedStringValue).SetActive(true);
                            }
                        }
                        break;
                    case VisualCustomisationType.MaterialChange:
                        if(true)
                        {
                            var material = (UnityEngine.Material)Resources.Load(visual.SavedStringValue);
                            var target = FindTargetObject(visual.TargetedGameObjectName);
                            var targetRendr = target.GetComponent<Renderer>();
                            targetRendr.sharedMaterial = material;


                            UnityEngine.Material[] sharedMaterialsCopy = targetRendr.sharedMaterials;
                            for (int index = 0; index < sharedMaterialsCopy.Length; index++)
                            {
                                sharedMaterialsCopy[index] = material;
                            }
                            targetRendr.sharedMaterials = sharedMaterialsCopy;
                        }
                        break;
                    case VisualCustomisationType.MaterialColor:
                        if(true)
                        {
                            var target = FindTargetObject(visual.TargetedGameObjectName);
                            var targetRendr = target.GetComponent<Renderer>();
                            var sharedMaterials = targetRendr.sharedMaterials;
                            var newMaterials = new List<UnityEngine.Material>();
                            foreach (var mat in sharedMaterials)
                            {
                                newMaterials.Add(new UnityEngine.Material(mat));
                            }

                            var applicableMaterials = newMaterials.Where(m => m.name == visual.StringRef).ToList();
                            applicableMaterials.ForEach(m => m.SetColor(visual.StringRefTwo, visual.SavedColorValue.ToUnityColor()));

                            targetRendr.sharedMaterials = newMaterials.ToArray();
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private GameObject FindTargetObject(string gameObjectName)
        {
            var player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogError("[RPGAIO] Could not find player gameobject.");
                return null;
            }

            var foundChild = player.transform.FindInChildren(gameObjectName);
            return foundChild != null ? foundChild.gameObject : null;
        }

        public void SetPlayerSave(PlayerSave loadedPlayer)
        {
            _playerSave = loadedPlayer;
            Player = _playerSave.Character;
            Player.VitalHandler = new VitalHandler(Player);
            Player.Equipment.Player = Player;
            Player.Inventory.Player = Player;
            Player.CharacterMono = this;
            Character = Player;
            Controller.SetPlayerControl(gameObject);
            Controller.IsPlayerCharacter = true;
            Initialised = true;

            Player.Equipment.UpdateDynamicItems();
            RefreshPrefabs();
        }

        public void OnMouseEnter()
        {
            UIHandler.MouseOnPlayer = true;
        }

        public void OnMouseExit()
        {
            UIHandler.MouseOnPlayer = false;
        }
    }
}