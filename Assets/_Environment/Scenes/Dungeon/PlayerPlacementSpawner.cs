using System.Collections;
using System.Collections.Generic;
using DungeonArchitect;
using UnityEngine;

public class PlayerPlacementSpawner : DungeonEventListener
{
    public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
    {
        PlacePlayer();
    }

    public void PlacePlayer()
    {
        GameObject playerSpawnPlaceHolder = GameObject.FindGameObjectWithTag("PlayerSpawnPlaceholder");
        GameObject playerSpawnPosition = GameObject.FindGameObjectWithTag("SpawnPosition");
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && playerSpawnPosition != null && playerSpawnPlaceHolder != null)
        {
            //Vector3 newPosition = ( playerSpawnPlaceHolder.transform.position
            Debug.Log("player.transform.position is " +player.transform.position);
            Debug.Log("playerSpawnPlaceHolder.transform.position is " + playerSpawnPlaceHolder.transform.position);
            player.transform.position = playerSpawnPlaceHolder.transform.position;
        }
        if (playerSpawnPosition == null)
        {
            Debug.Log("playerSpawnPosition was null at the end of post dungeon build event");
        }
        if (playerSpawnPlaceHolder == null)
        {
            Debug.Log("playerSpawnPlaceHolder was null at the end of post dungeon build event");
        }
        if (player == null)
        {
            Debug.Log("player was null at the end of post dungeon build event");
        }
    }

}
