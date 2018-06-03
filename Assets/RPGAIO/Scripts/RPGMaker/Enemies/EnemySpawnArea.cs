using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.RPGAIO.Scripts.RPGMaker.Enemies
{
    public class EnemySpawnArea : MonoBehaviour
    {
        public enum SpawnMode
        {
            KeepAtMax,
            RespawnWhenZero
        }

        public bool SpawnOnStart;
        public SpawnMode Mode = SpawnMode.KeepAtMax;
        public int MaxNumberOfEnemies;
        
        public float TimeBetweenSpawns;
        private float curTimeBetweenSpawn;

        public Collider SpawnAreaCollider;
        public List<GameObject> EnemyPrefabs = new List<GameObject>();
        public List<GameObject> SpawnedEnemies = new List<GameObject>();
 
        void Awake(){
            if(SpawnOnStart)
                SpawnEnemies();
        }
 
        void Update()
        {
            //remove any null objects
            for (int index = 0; index < SpawnedEnemies.Count; index++)
            {
                var go = SpawnedEnemies[index];
                if(go == null)
                {
                    SpawnedEnemies.RemoveAt(index);
                    index--;
                }
            }

            //spawn new if we need to
            if(Mode == SpawnMode.KeepAtMax)
            {
                if(SpawnedEnemies.Count < MaxNumberOfEnemies)
                {
                    curTimeBetweenSpawn += Time.deltaTime;
                    if(curTimeBetweenSpawn >= TimeBetweenSpawns)
                    {
                        SpawnEnemy();
                        curTimeBetweenSpawn = 0;
                    }
                }
            }
            else if(Mode == SpawnMode.RespawnWhenZero)
            {
                if(SpawnedEnemies.Count == 0)
                {
                    curTimeBetweenSpawn += Time.deltaTime;
                    if (curTimeBetweenSpawn >= TimeBetweenSpawns)
                    {
                        SpawnEnemies();
                        curTimeBetweenSpawn = 0;
                    }
                }
            }

        }

        void SpawnEnemy(){
            var sizex = SpawnAreaCollider.bounds.size.x;
            var sizey = SpawnAreaCollider.bounds.size.y;
            var sizez = SpawnAreaCollider.bounds.size.z;
 
            var currentPos = transform.position;

            var position = new Vector3(Random.Range(currentPos.x - (sizex/2),currentPos.x + (sizex/2)),
                                    Random.Range(currentPos.y - (sizey/2),currentPos.y + (sizey/2)),
                                    Random.Range(currentPos.z - (sizez/2),currentPos.z + (sizez/2)));

            var newEnemy = Instantiate(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count)], position, transform.rotation) as GameObject;
            newEnemy.transform.parent = this.transform;
            SpawnedEnemies.Add(newEnemy);
        }
        
        void SpawnEnemies(){
            var sizex = SpawnAreaCollider.bounds.size.x;
            var sizey = SpawnAreaCollider.bounds.size.y;
            var sizez = SpawnAreaCollider.bounds.size.z;
 
            var currentPos = transform.position;

            for (var i = 0; i <= MaxNumberOfEnemies; ++i)
            {
                var position = new Vector3(Random.Range(currentPos.x - (sizex/2),currentPos.x + (sizex/2)),
                                        Random.Range(currentPos.y - (sizey/2),currentPos.y + (sizey/2)),
                                        Random.Range(currentPos.z - (sizez/2),currentPos.z + (sizez/2)));

                var newEnemy = Instantiate(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count)], position, transform.rotation) as GameObject;
                newEnemy.transform.parent = this.transform;
                SpawnedEnemies.Add(newEnemy);
            }
        }
    }
}
