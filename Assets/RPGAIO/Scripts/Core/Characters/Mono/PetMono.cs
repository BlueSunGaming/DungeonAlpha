using System;
using Assets.Scripts.Beta.NewImplementation;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LogicSpawn.RPGMaker.Core
{
    public class PetMono : MonoBehaviour
    {
        public RPGFollow Follow;
        public RPGController Controller;
        public PetData PetData;

        void Update()
        {
            if (Follow == null)
            {
                Follow = GetComponent<RPGFollow>();
            }

            if (Controller == null)
            {
                Controller = GetComponent<RPGController>();
            }

            if (Follow.TargetToFollow != GetObject.PlayerMono)
            {
                Follow.TargetToFollow = GetObject.PlayerMono;    
            }
            Follow.FollowTarget = true;

            switch(PetData.CurrentBehaviour)
            {
                case PetBehaviour.Aggresive:
                    ((CombatCharacter) Controller.Character).IsAggressive = true;
                    break;
                case PetBehaviour.Assist:
                    ((CombatCharacter)Controller.Character).IsAggressive = false;
                    break;
                case PetBehaviour.PetOnly:
                    ((CombatCharacter)Controller.Character).IsAggressive = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static GameObject SpawnPet(PetData data, Vector3 position)
        {
            
            var pet = data;
            if (pet != null)
            {
                CombatCharacter petChar = null;
                if (pet.IsNpc)
                {
                    var npc = Rm_RPGHandler.Instance.Repositories.Interactable.GetNPC(pet.CharacterID);
                    petChar = GeneralMethods.CopyObject((CombatCharacter)npc);
                }
                else
                {
                    var enemy = Rm_RPGHandler.Instance.Repositories.Enemies.Get(pet.CharacterID);
                    petChar = GeneralMethods.CopyObject(enemy);
                }

                if(GetObject.PlayerCharacter.CurrentPet != null)
                {
                    GetObject.PlayerCharacter.CurrentPet.Remove();
                }

                var petGo = (GameObject)Instantiate(Resources.Load(petChar.CharPrefabPath), position + Vector3.back, Quaternion.identity);
                var petMono = petGo.AddComponent<PetMono>();
                var follow = petGo.GetComponent<RPGFollow>();
                follow.FollowTarget = true;
                follow.TargetToFollow = GetObject.PlayerMono;
                petMono.PetData = data;

                //Update current pet
                GetObject.PlayerCharacter.CurrentPet = petMono;
                GetObject.PlayerSave.CurrentPet = data;
                return petGo;
            }

            return null;
        }

        public void Remove()
        {
            GetObject.PlayerSave.CurrentPet = null;
            GetObject.PlayerCharacter.CurrentPet = null;
            Destroy(gameObject);
        }
    }
}