using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class SerializableVector3
    {
        public float X;
        public float Y;
        public float Z;

        public SerializableVector3(Vector3 vector3)
        {
            X = vector3.x;
            Y = vector3.y;
            Z = vector3.z;
        }

        public static implicit operator Vector3(SerializableVector3 s)
        {
            return new Vector3(s.X,s.Y,s.Z);
        }
    }

    public class SerializableQuaternion
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public SerializableQuaternion(Quaternion quaternion)
        {
            X = quaternion.x;
            Y = quaternion.y;
            Z = quaternion.z;
            W = quaternion.w;
        }

        public static implicit operator Quaternion(SerializableQuaternion s)
        {
            return new Quaternion(s.X, s.Y, s.Z,s.W);
        }
    }

    
}