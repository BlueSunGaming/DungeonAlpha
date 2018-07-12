using UnityEngine;
using System.Collections;
using DAShooter;
using DungeonArchitect;


public class SetupDungeon : MonoBehaviour
{
    private static SetupDungeon instance;
    public Dungeon dungeon;

    private LevelNpcSpawner enemySpawner;
    private PlayerPlacementSpawner ppSpawner;

    /// <summary>
    /// If we have static geometry already in the level created during design time, then the pooled scene
    /// provider cannot re-use it because the editor would have performed optimizations on it and might not be able to move it
    /// This flag clears out any design time static geometry before rebuilding to avoid movement issues of static objects
    /// </summary>
    bool performCleanRebuild = true;

    public static SetupDungeon Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        instance = this;
        enemySpawner = GetComponent<LevelNpcSpawner>();
        ppSpawner = GetComponent<PlayerPlacementSpawner>();


        CreateNewLevel();
    }

    public void CreateNewLevel()
    {
        // Assing a different seed to create a new layout
        int seed = Mathf.FloorToInt(Random.value * int.MaxValue);
        if (dungeon != null)
        {
            dungeon.Config.Seed = (uint) seed;
        }
        else
        {
            Debug.Log("dungeon was null during the call to create new level");
        }

        // Rebuild a new dungeon
        StartCoroutine(RebuildLevel());
    }
    
    IEnumerator RebuildLevel()
    {
        if (dungeon != null)
        {
            if (performCleanRebuild)
            {
                // We want to remove design time data with a clean destroy since editor would allow modification of optimized static game objects
                // We want to do this only for the first time
                dungeon.DestroyDungeon();
                performCleanRebuild = false;

                // Wait for 1 frame to make sure our design time objects were destroyed
                yield return 0;
            }

            // Build the dungeon
            var config = dungeon.Config;
            if (config != null)
            {
                config.Seed = (uint)(Random.value * uint.MaxValue);
                dungeon.Build();

                yield return 0;

                ppSpawner.OnPostDungeonBuild(dungeon, dungeon.ActiveModel);

                GameObject player = GameObject.FindGameObjectWithTag("Player");
                GameObject spawnPosition = GameObject.FindGameObjectWithTag("SpawnPosition");

                //if (player != null && spawnPosition != null)
                //{
                //    player.transform.position = spawnPosition.transform.position;
                //}
            }
        }
    }
}
