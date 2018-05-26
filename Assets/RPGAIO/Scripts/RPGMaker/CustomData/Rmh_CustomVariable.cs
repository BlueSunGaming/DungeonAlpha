using System;

namespace LogicSpawn.RPGMaker
{
    public class Rmh_CustomVariable
    {
        public string ID;
        public string Name;
        public float FloatValue;
        public string StringValue;
        public int IntValue;
        public bool BoolValue;
        public Rmh_CustomVariableType VariableType;

        public Rmh_CustomVariable()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Custom Variable";
            VariableType = Rmh_CustomVariableType.String;
            StringValue = "";
        }

        public override string ToString()
        {
            return Name;
        }
    }
}