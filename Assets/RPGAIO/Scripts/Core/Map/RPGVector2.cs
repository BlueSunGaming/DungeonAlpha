using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class RPGVector2
    {
        public float X ;
        public float Y ;

        public RPGVector2(float x, float y)
        {
            X = x;
            Y = y;  
        }

        public static implicit operator Vector2(RPGVector2 m)
        {
            return new Vector2(m.X,m.Y);
        }

    }    
    
    public class RPGVector3
    {
        public float X ;
        public float Y ;
        public float Z ;

        [JsonIgnore]
        public static RPGVector3 Zero
        {
            get { return new RPGVector3(0, 0, 0); }
        }


        public RPGVector3()
        {
            
        }

        public RPGVector3(float x, float y, float z)
        {
            X = x;
            Y = y;  
            Z = z;  
        }

        public RPGVector3(Vector3 vector3)
        {
            X = vector3.x;
            Y = vector3.y;  
            Z = vector3.z;  
        }

        public static RPGVector3 operator +(RPGVector3 c1, RPGVector3 c2)
        {
            return new RPGVector3(c1.X + c2.X, c1.Y + c2.Y, c1.Z + c2.Z);
        }
        public static RPGVector3 operator -(RPGVector3 c1, RPGVector3 c2)
        {
            return new RPGVector3(c1.X - c2.X, c1.Y - c2.Y, c1.Z - c2.Z);
        }

        public static implicit operator Vector3(RPGVector3 m)
        {
            return new Vector3(m.X,m.Y, m.Z);
        }

        public static implicit operator RPGVector3(Vector3 v)
        {
            return new RPGVector3(v.x,v.y,v.z);
        }
    }
}