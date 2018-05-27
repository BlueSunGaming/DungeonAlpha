using System;
using System.Collections;
using System.Collections.Generic;
using DungeonRPG.Character;
using UnityEngine;
using UnityEngine.SceneManagement;




public class SceneChangeToDungeonEntrance : MonoBehaviour
{
    

    
    private void Start()
    {

    }

    private void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(1);
    }
}


