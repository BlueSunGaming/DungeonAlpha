using System.Collections;
using Assets.Scripts.Exceptions;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("GameObject", "")]
    public class SpawnGameObject : SimpleNode
    {

        public override string Name
        {
            get { return "Spawn GameObject"; }
        }

        public override string Description
        {
            get { return "Spawns a gameobject to a position folder"; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return true;
            }
        }

        public override string NextNodeLinkLabel(int index)
        {
            return "Next";
        }

        protected override void SetupParameters()
        {
            Add("GameObject", PropertyType.GameObject, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
            Add("Spawn Position", PropertyType.Vector3, null, RPGVector3.Zero, PropertySource.EnteredOrInput, PropertyFamily.Object);
            Add("Set Rotation?", PropertyType.Bool, null, true).WithSubParams(
                SubParam("Euler Rotation", PropertyType.Vector3, null, RPGVector3.Zero, PropertySource.EnteredOrInput, PropertyFamily.Object).IfTrue()
            );
            Add("Store Reference", PropertyType.GameObject, null, null, PropertySource.InputOnly, PropertyFamily.Object);
        }
        
        protected override void Eval(NodeChain nodeChain)
        {
            var gameObjectPath = (string)ValueOf("GameObject"); 
            var gameObject = (GameObject)Resources.Load(gameObjectPath); 
            var spawnPos = (RPGVector3)ValueOf("Spawn Position"); 
            var setRotation = (bool)ValueOf("Set Rotation?"); 
            var rotation = (RPGVector3)Parameter("Set Rotation?").ValueOf("Euler Rotation"); 
            var spawnRotation = setRotation ? Quaternion.Euler(rotation) : Quaternion.identity;
            var spawnedObject = Object.Instantiate(gameObject, spawnPos, spawnRotation);
            var hasReference = !string.IsNullOrEmpty(Parameters["Store Reference"].InputNodeId.ID); 

            if (hasReference)
            {
                ApplyFunctionTo("Store Reference", o =>
                {
                    o = spawnedObject;
                    return o;
                });
            }
        }
    }
}