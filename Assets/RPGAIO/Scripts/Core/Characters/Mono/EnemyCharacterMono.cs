using System.Linq;
using Assets.Scripts.Beta.NewImplementation;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class EnemyCharacterMono : BaseCharacterMono
    {
        public string Name;
        public CombatCharacter Player ;
        public string EnemyID;

        //public EnemyAIHandler EnemyAIHandler ;

        protected override void DoStart()
        {
            //AI = GetComponent<AIBrain>();

            Controller.ToggleAI(true);
        }

        protected override void DoUpdate()
        {
            if (!Initialised) return;
        }

        protected void OnEnable()
        {
            if (Initialised) return;

            if (!string.IsNullOrEmpty(EnemyID))
            {
                var enemyChar = Rm_RPGHandler.Instance.Repositories.Enemies.AllEnemies.FirstOrDefault(i => i.ID == EnemyID);
                if (enemyChar != null)
                {
                    SetEnemy(enemyChar);
                }
                else
                {
                    Debug.LogError("Could not find Enemy data for Spawned Enemy: " + EnemyID + ". Destroying.");
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.LogError("Could not find Enemy data for Spawned Enemy: " + EnemyID + ". Destroying.");
                Destroy(gameObject);
            }
            Initialised = true;
        }

        protected void OnDisable()
        {
            Initialised = false;
        }

        public void SetEnemy(CombatCharacter player)
        {
            player = GeneralMethods.CopyObject(player);

            Player = player;
            Character = Player;
            Player.CharacterMono = this;
            Controller.SpawnPosition = transform.position;
            Controller.State = RPGControllerState.Idle;
            Player.VitalHandler = new VitalHandler(Player);
            Player.VitalHandler.Health.CurrentValue = Player.VitalHandler.Health.MaxValue;
            Player.CCInit();
            RefreshPrefabs();
            Name = Character.Name;
            Initialised = true;
        }
    }
}