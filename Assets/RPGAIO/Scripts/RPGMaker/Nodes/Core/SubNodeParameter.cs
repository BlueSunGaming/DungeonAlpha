using System;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class SubNodeParameter
    {
        [JsonIgnore]
        public Predicate<NodeParameter> Condition = parameter => true;
        public NodeParameter Parameter;

        public SubNodeParameter()
        {
            Parameter = null;
        }

        public SubNodeParameter If(Predicate<NodeParameter> condition)
        {
            Condition = condition;
            return this;
        }

        public SubNodeParameter IfTrue()
        {
            Condition = p => (bool)p.Value;
            return this;
        }

        public SubNodeParameter IfFalse()
        {
            Condition = p => !((bool)p.Value);
            return this;
        }

        public SubNodeParameter Always()
        {
            Condition = parameter => true;
            return this;
        }

//        public SubNodeParameter SubParam(string paramName)
//        {
//            return Parameter.SubParams.ContainsKey(paramName) ? Parameter.SubParams[paramName] : null;
//        }
//
//        public object ValueOf(string paramName)
//        {
//            var x = Parameter.SubParams.ContainsKey(paramName);
// 
//            if(x)
//            {
//                var y = Parameter.SubParams[paramName].Parameter.Value;
//                return y;
//            }
//            object m = null;
//            return m;
//        } 

        public SubNodeParameter WithSubParams(params SubNodeParameter[] subParams)
        {
            Parameter.WithSubParams(subParams);
            return this;
        }

        public void SetNodeChain(NodeChain nodeChain)
        {
            Parameter.NodeChain = nodeChain;
        }
    }
}