using System.Collections.Generic;
using System.Linq;

namespace LogicSpawn.RPGMaker.API
{
    public static partial class RPG
    {
        public class Popup
        {
            public static List<PopupInfo> PopupQueue = new List<PopupInfo>();

            public static void ShowInfo(string info)
            {
                PopupQueue.Add(new PopupInfo(info, 5.0f));
            }

            public static void ShowInfo(string info, float time)
            {
                PopupQueue.Add(new PopupInfo(info, time));
            }
        }

        public class PopupInfo
        {
            public string Text;
            public float ShowTime;
            public bool Shown = false;

            public PopupInfo(string text, float time)
            {
                Text = text;
                ShowTime = time;
            }
        }
    }
}