using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

public class TriggerEventAlt : MonoBehaviour {

	// Use this for initialization
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var eventId = Rm_RPGHandler.Instance.Nodes.EventNodeChains.First(c => c.CurrentNode.NodeChainName == "TestChain").CurrentNode.ID;
        GetObject.EventHandler.RunEvent(eventId);
        Destroy(gameObject);
    }
}
