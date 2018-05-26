using System;
using Assets.Scripts.Beta.NewImplementation;
using Assets.Scripts.Core.Interaction;
using Assets.Scripts.Testing;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_PrefabGenerator
    {
        public static string GeneratePrefab(Rmh_PrefabType prefabType, SkillType? skillType = null, string param = "")
        {
            var prefab = new GameObject(prefabType + "_"  + "Prefab_" +  GeneralMethods.NextID);
            
            var identifier = prefab.AddComponent<Rm_PrefabIdentifier>();
            identifier.SearchName = prefab.name;
            identifier.PrefabType = prefabType;

            switch(prefabType)
            {
                case Rmh_PrefabType.Casting:
                case Rmh_PrefabType.Cast:
                case Rmh_PrefabType.Impact:
                case Rmh_PrefabType.Effect_Active:
                case Rmh_PrefabType.Effect_Activated:
                case Rmh_PrefabType.Effect_Expired:
                case Rmh_PrefabType.DoT_Damage_Tick:
                case Rmh_PrefabType.Moving_To_Effect:
                case Rmh_PrefabType.Target_Reached_Effect:
                case Rmh_PrefabType.Melee_Effect:
                    prefab.AddComponent<DestroyHelper>();
                    prefab.AddComponent<IgnoredByRaycast>();
                    break;
                case Rmh_PrefabType.Interactable:
                    var cc1 = prefab.AddComponent<CapsuleCollider>();
                    cc1.isTrigger = true;
                    var io = prefab.AddComponent<InteractiveObjectMono>();
                    io.ObjectID = param;
                    AddCameraPivot(prefab);

                    var minimapIcon2 = Resources.Load("RPGMakerAssets/PrefabGen/MinimapIconUI");
                    var minimapUI2 = (GameObject)PrefabUtility.InstantiatePrefab(minimapIcon2);
                    minimapUI2.transform.SetParent(prefab.transform, false);
                    minimapUI2.transform.localPosition = new Vector3(0, -0.5f, 0);

                    var minimapIconModel2 = minimapUI2.GetComponent<MinimapIconModel>();
                    minimapIconModel2.Type = MinimapIconType.Interactable;

                    break;
                case Rmh_PrefabType.Harvest:
                    var cc = prefab.AddComponent<CapsuleCollider>();
                    cc.isTrigger = true;
                    var ih = prefab.AddComponent<InteractableHarvestable>();
                    ih.ObjectID = param;
                    prefab.tag = "Harvestable";

                    var minimapIcon1 = Resources.Load("RPGMakerAssets/PrefabGen/MinimapIconUI");
                    var minimapUI1 = (GameObject)PrefabUtility.InstantiatePrefab(minimapIcon1);
                    minimapUI1.transform.SetParent(prefab.transform, false);
                    minimapUI1.transform.localPosition = new Vector3(0, -0.5f, 0);

                    var minimapIconModel1 = minimapUI1.GetComponent<MinimapIconModel>();
                    minimapIconModel1.Type = MinimapIconType.Harvestable;

                    break;
                case Rmh_PrefabType.Skill:
                    prefab.AddComponent<DestroyHelper>();
                    if(skillType != SkillType.Spawn)
                    {
                        prefab.AddComponent<IgnoredByRaycast>();    
                    }
                    break;
                case Rmh_PrefabType.SpawnPoint:
                    prefab.tag = "SpawnPosition";
                    prefab.AddComponent<Cull>();
                    var spawnGraphic = Resources.Load("RPGMakerAssets/PrefabGen/SpawnPos");
                    var graphic = PrefabUtility.InstantiatePrefab(spawnGraphic) as GameObject;
                    graphic.transform.parent = prefab.transform;
                    graphic.transform.localPosition = Vector3.zero;
                    break;
                case Rmh_PrefabType.Target_Selected_Prefab:
                    prefab.AddComponent<IgnoredByRaycast>();
                    prefab.AddComponent<TargetLockPrefab>();
                    break;
                case Rmh_PrefabType.Cast_Area_Prefab:
                    break;
                case Rmh_PrefabType.Auto_Attack_Projectile:
                    prefab.AddComponent<IgnoredByRaycast>();
                    prefab.AddComponent<DestroyHelper>();
                    prefab.AddComponent<ProjectileAutoAttackHandler>();
                    var boxCollider = prefab.AddComponent<BoxCollider>();
                    boxCollider.isTrigger = false;
                    var rigidbody = prefab.AddComponent<Rigidbody>();
                    rigidbody.isKinematic = false;
                    rigidbody.useGravity = false;
                    rigidbody.freezeRotation = true;
                    break;
                case Rmh_PrefabType.Misc:
                    break;
                case Rmh_PrefabType.Player_Class:
                case Rmh_PrefabType.Enemy:
                case Rmh_PrefabType.NPC:
                    var charC =  prefab.AddComponent<CharacterController>();
                    charC.slopeLimit = 25;
                    charC.height = 2;
                    charC.center = new Vector3(0,1,0);
                    charC.radius = 0.5f;
                    var navMeshAgent = prefab.AddComponent<NavMeshAgent>();
                    navMeshAgent.radius = 0.5f;
                    navMeshAgent.height = 2;
                    navMeshAgent.enabled = false;
                    navMeshAgent.avoidancePriority = 50;
                    var rigidB = prefab.AddComponent<Rigidbody>();
                    rigidB.useGravity = true;
                    rigidB.isKinematic = true;
                    rigidB.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                    var boxC = prefab.AddComponent<BoxCollider>();
                    boxC.isTrigger = false;
                    boxC.center = new Vector3(0,1,0);
                    boxC.size = new Vector3(1,2,1);
                    var rpgC = prefab.AddComponent<RPGController>();
                    rpgC.cameraMode = Rm_RPGHandler.Instance.DefaultSettings.DefaultCameraMode;
                    prefab.AddComponent<RPGAnimation>();
                    prefab.AddComponent<RPGCombat>();

                    var capsuleGraphic = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    var capsuleCol = capsuleGraphic.GetComponent<CapsuleCollider>();
                    UnityEngine.Object.DestroyImmediate(capsuleCol);
                    capsuleGraphic.transform.parent = prefab.transform;
                    capsuleGraphic.transform.localPosition = new Vector3(0,1,0);

                    rpgC.characterModel = capsuleGraphic;

                    var targetLockH = prefab.AddComponent<TargetLockHandler>();

                    var targetLockPrefab = Resources.Load("RPGMakerAssets/PrefabGen/TargetLock");
                    var targetLock = PrefabUtility.InstantiatePrefab(targetLockPrefab) as GameObject;
                    targetLock.transform.parent = prefab.transform;
                    targetLock.transform.localPosition = new Vector3(0, 0.05f, 0);

                    targetLockH.TargetLockObject = targetLock;

                    var cameraPivot = new GameObject("cameraPivot");
                    cameraPivot.transform.parent = prefab.transform;
                    cameraPivot.transform.localPosition = new Vector3(0, 1.7f, 0);

                    var minimapIcon = Resources.Load("RPGMakerAssets/PrefabGen/MinimapIconUI");
                    var minimapUI = (GameObject)PrefabUtility.InstantiatePrefab(minimapIcon);
                    minimapUI.transform.SetParent(prefab.transform, false);
                    minimapUI.transform.localPosition = new Vector3(0, -0.5f, 0);

                    var minimapIconModel = minimapUI.GetComponent<MinimapIconModel>();
                    switch (prefabType)
                    {
                        case Rmh_PrefabType.Enemy:
                            minimapIconModel.Type = MinimapIconType.Enemy;
                            break;
                        case Rmh_PrefabType.NPC:
                            minimapIconModel.Type = MinimapIconType.NPC;
                            break;
                        case Rmh_PrefabType.Player_Class:
                            minimapIconModel.Type = MinimapIconType.Player;
                            break;
                    }

                    break;
                case Rmh_PrefabType.Sound_FX:
                    prefab.AddComponent<Audio_SFX>();
                    break;
                case Rmh_PrefabType.Ambient_Music:
                    prefab.AddComponent<Audio_Ambient>();
                    break;
                case Rmh_PrefabType.Background_Music:
                    prefab.AddComponent<Audio_BgMusic>();
                    break;
                case Rmh_PrefabType.Map_Location_Trigger:
                    //throw new NotImplementedException();
                    break;
                case Rmh_PrefabType.Event_Trigger:
                    //throw new NotImplementedException();
                    break;
                case Rmh_PrefabType.Loot_Item_Prefab:
                    var rigidBody = prefab.AddComponent<Rigidbody>();
                    rigidBody.useGravity = true;
                    rigidBody.isKinematic = false;
                    rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                    var boxCol = prefab.AddComponent<BoxCollider>();
                    boxCol.isTrigger = true;
                    boxCol.size = new Vector3(0.5f, 0.5f, 0.5f);
                    var sphereCol = prefab.AddComponent<SphereCollider>();
                    sphereCol.isTrigger = false;
                    sphereCol.radius = 0.2f;
                    prefab.AddComponent<LootItemMono>();
                    prefab.tag = "LootItem";
                    prefab.layer = LayerMask.NameToLayer("LootItem");
                    var c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    var cubeCol = c.GetComponent<BoxCollider>();
                    UnityEngine.Object.DestroyImmediate(cubeCol);
                    c.transform.localScale = new Vector3(0.35f,0.15f,0.35f);
                    c.transform.parent = prefab.transform;
                    c.transform.localPosition = Vector3.zero;
                    c.layer = LayerMask.NameToLayer("LootItem");
                    break;

                default:
                    Debug.LogError("Prefab creation not defined for this object!");
                    break;
            }

            if(prefabType == Rmh_PrefabType.Player_Class)
            {
                prefab.AddComponent<PlayerMono>();  
                var navMeshObs = prefab.AddComponent<NavMeshObstacle>();
                navMeshObs.shape = NavMeshObstacleShape.Capsule;
                navMeshObs.height = 2;
                navMeshObs.center = new Vector3(0,1,0);
                navMeshObs.radius = 0.5f;
                navMeshObs.carving = false;
                navMeshObs.enabled = false;
                prefab.tag = "Player";
                prefab.layer = LayerMask.NameToLayer("Combatant");

                //todo: rendertex
                //var cameraPrefab = Resources.Load("RPGMakerAssets/PrefabGen/PortraitCamera");
                //var camera = PrefabUtility.InstantiatePrefab(cameraPrefab) as GameObject;
                //camera.transform.SetParent(prefab.transform, false);
                //camera.transform.localPosition = new Vector3(0, 1.25f, 3f);
                //camera.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

            }
            else if(prefabType == Rmh_PrefabType.NPC || prefabType == Rmh_PrefabType.Enemy)
            {
                prefab.AddComponent<RPGPatrol>();
                prefab.AddComponent<RPGFollow>();
                var combatantUi = prefab.AddComponent<CombatantUI>(); 
                var spawnGraphic = Resources.Load("RPGMakerAssets/PrefabGen/CombatantUI");
                var graphic = PrefabUtility.InstantiatePrefab(spawnGraphic) as GameObject;
                graphic.transform.SetParent(prefab.transform,false);
                graphic.transform.localPosition = new Vector3(0,2.5f,0);

                combatantUi.Canvas = graphic;
                combatantUi.HealthBar = graphic.transform.Find("HealthBar").GetComponent<Image>();
                combatantUi.HealthText = graphic.transform.Find("HealthText").GetComponent<Text>();
                
                if (prefabType == Rmh_PrefabType.Enemy)
                {
                    var enemyMono = prefab.AddComponent<EnemyCharacterMono>();
                    enemyMono.EnemyID = param;
                    prefab.tag = "Enemy";
                    prefab.layer = LayerMask.NameToLayer("Combatant");
                }
                if (prefabType == Rmh_PrefabType.NPC)
                {
                    var capCol = prefab.AddComponent<CapsuleCollider>();
                    capCol.isTrigger = true;
                    capCol.center = new Vector3(0,1,0);
                    capCol.radius = 1;
                    capCol.height = 2.5f;
                    capCol.direction = 1;

                    var npcMono = prefab.AddComponent<NpcCharacterMono>();
                    prefab.AddComponent<InteractableNPC>();
                    npcMono.NpcID = param;

                    prefab.tag = "NPC";
                    prefab.layer = LayerMask.NameToLayer("Combatant");

                    var npcQuestStatus = Resources.Load("RPGMakerAssets/PrefabGen/NpcQuestStatus");
                    var npcQuestStatusObj = (GameObject)PrefabUtility.InstantiatePrefab(npcQuestStatus);
                    npcQuestStatusObj.transform.SetParent(prefab.transform, false);
                    npcQuestStatusObj.transform.localPosition = new Vector3(0, 2.6f, 0);

                    npcMono.QuestStatusModel = npcQuestStatusObj.GetComponent<NpcQuestStatusModel>();
                }
            }

            if(prefabType == Rmh_PrefabType.Skill)
            {
                prefab = AddSkillScripts(prefab, skillType.Value);
            }

            string path = "";
            
            if(prefabType == Rmh_PrefabType.Player_Class)
            {
                path = GeneralMethodsEditor.CreatePrefab(prefab, "Prefabs/Classes");
            }
            else
            {
                path = GeneralMethodsEditor.CreatePrefab(prefab);
            }

            Selection.activeObject = SceneView.currentDrawingSceneView;
            if(Selection.activeObject != null)
            {
                var sceneCam = SceneView.currentDrawingSceneView.camera;
                var spawnPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 3f));
                prefab.transform.position = spawnPos;
            }

            return path;
        }

        private static void AddCameraPivot(GameObject prefab)
        {
            var pivot1 = new GameObject("cameraPivot");
            pivot1.transform.parent = prefab.transform;
            pivot1.transform.localPosition = Vector3.zero;
        }

        private static GameObject AddSkillScripts(GameObject prefab, SkillType skillType)
        {
            switch(skillType)
            {
                case SkillType.Area_Of_Effect:
                    {

                        prefab.AddComponent<AreaOfEffectSkillHandler>();
                        var sphereCollider = prefab.AddComponent<SphereCollider>();
                        sphereCollider.isTrigger = true;
                        var boxCollider = prefab.AddComponent<BoxCollider>();
                        boxCollider.isTrigger = true;
                        var rigidbody = prefab.AddComponent<Rigidbody>();
                        rigidbody.isKinematic = false;
                        rigidbody.useGravity = false;
                        rigidbody.freezeRotation = true;
                    }
                    break;
                case SkillType.Projectile:
                    {
                        prefab.AddComponent<ProjectileSkillHandler>();
                        var sphereCollider = prefab.AddComponent<SphereCollider>();
                        sphereCollider.isTrigger = false;
                        var rigidbody = prefab.AddComponent<Rigidbody>();
                        rigidbody.isKinematic = false;
                        rigidbody.useGravity = false;
                        rigidbody.freezeRotation = true;
                    }
                    break;
                case SkillType.Aura:
                    break;
                case SkillType.Spawn:
                    {
                        prefab.AddComponent<SpawnSkillHandler>();
                    }
                    break;
                case SkillType.Restoration:
                    break;
                case SkillType.Ability:
                    break;
                case SkillType.Melee:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("skillType");
            }

            return prefab;
        }
    }
}