using LogicSpawn.RPGMaker.Core;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class SavableGameObjectData
    {
        public string UniqueID;
        public string SceneName;

        public SerializableVector3 Position;
        public SerializableQuaternion Rotation;
        public SerializableVector3 Scale;
        public bool Active;
        public bool Destroyed;

        public CombatCharacter AdditionalData;

        public SavableGameObjectData()
        {
            SceneName = "";
            Position = new SerializableVector3(Vector3.zero);
            Position = new SerializableVector3(new Vector3(1,1,1));
            Rotation = new SerializableQuaternion(Quaternion.identity);
            Active = true;
            Destroyed = false;
        }
    }
}