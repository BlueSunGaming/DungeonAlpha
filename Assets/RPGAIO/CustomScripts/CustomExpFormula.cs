using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Custom
{
    public static class CustomExpFormula
    {
        public static long ExpToNextPlayerLevel(int currentPlayerLevel)
        {
            //Customise this to your liking. Below is a very simple example
            //Level 1-9 : 100 exp to level
            //Level 10-19 : 1000 exp to level
            //Level 20+ : 2500 exp to level

            if(currentPlayerLevel < 10)
            {
                return 100;
            }

            if(currentPlayerLevel < 20)
            {
                return 1000;
            }

            return 2500;
        }
    }
}
