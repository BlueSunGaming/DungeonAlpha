using LogicSpawn.RPGMaker.API;
using UnityEngine;
using System.Collections;

public class IntroHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    //Run coroutine for movies/slides

        RPG.LoadLevel("MainMenu",false);
	}
}
