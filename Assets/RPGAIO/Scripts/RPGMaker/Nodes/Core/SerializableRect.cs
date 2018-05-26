using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class SerializableRect
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        [JsonIgnore]
        public Rect Rect
        {
            get { return new Rect(X, Y, Width, Height); }
            set
            {
                X = value.x;
                Y = value.y;
                Width = value.width;
                Height = value.height;
            }
        }

        public SerializableRect()
        {

        }

        public SerializableRect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public SerializableRect(Rect r)
        {
            X = r.x;
            Y = r.y;
            Width = r.width;
            Height = r.height;
        }
    }
}