using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Generic
{
    public class ThreeData
    {
        public string valueA;
        public string valueB;
        public string valueC;
    }

    public class ThreeDataHolder
    {
        public List<ThreeData> Data;

        public ThreeDataHolder()
        {
            Data = new List<ThreeData>();
        }

        public void Add(string a, string b, string c)
        {
            Data.Add(new ThreeData()
                         {
                             valueA = a, valueB = b, valueC = c
                         });
        }
    }
}