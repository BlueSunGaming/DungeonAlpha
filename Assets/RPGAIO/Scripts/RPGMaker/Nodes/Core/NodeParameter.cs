using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LogicSpawn.RPGMaker.Core
{
    public class NodeParameter
    {
        public string Name;
        public PropertyType PropertyType;
        public PropertyFamily PropertyFamily;
        public PropertySource Source;
        public object Property;
        public object Value;
        public StringField InputNodeId;
        public Dictionary<string, SubNodeParameter> SubParams;

        [JsonIgnore]
        public NodeChain NodeChain;

        public NodeParameter()
        {
            Name = "Property";
            PropertyType = PropertyType.String;
            Source = PropertySource.EnteredOnly;
            InputNodeId = new StringField();
            PropertyFamily = PropertyFamily.Primitive;
            Property = null;
            Value = "";
            SubParams = new Dictionary<string, SubNodeParameter>();
        }

        public SubNodeParameter SubParam(string paramName)
        {
            return SubParams.ContainsKey(paramName) ? SubParams[paramName] : null;
        }

        public object ValueOf(string paramName)
        {
            var param = SubParams.ContainsKey(paramName) ? SubParams[paramName].Parameter : null;
            if (param == null) throw new NodeParameterNotFoundException("Node paramter not found. (" + paramName + ")");

            if (param.Source == PropertySource.EnteredOnly)
            {
                return param.Value;
            }
            else if (param.Source == PropertySource.InputOnly)
            {
                //get the first properselector/etc
                if (!string.IsNullOrEmpty(param.InputNodeId.ID))
                {
                    return ValueOfInput(param);
                }
                else
                {
                    throw new NodeParameterMissingException("Node paramater has a required parameter which is missing.");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(param.InputNodeId.ID))
                {
                    return param.Value;
                }
                else
                {
                    return ValueOfInput(param);
                }
            }
        }

        private object ValueOfInput(NodeParameter nodeParameter)
        {
            //get the first property
            var nodeChain = NodeChain;
            var inputNodeId = nodeParameter.InputNodeId.ID;
            var inputNode = nodeChain.Nodes.FirstOrDefault(n => n.ID == inputNodeId) as PropertyNode;
            var input = inputNode.EvaluateInput(nodeChain, o => o);
            return input;
        }

        public void WithSubParams(params SubNodeParameter[] subParams)
        {
            foreach(var subParam in subParams)
            {
                SubNodeParameter exists;
                SubParams.TryGetValue(subParam.Parameter.Name, out exists); 
                if (exists != null)
                {
                    if (exists.Parameter.PropertyType == PropertyType.Enum)
                    {
                        if (!(exists.Parameter.Property is Type))
                        {
                            var paramType = Type.GetType((string)exists.Parameter.Property);
                            exists.Parameter.Property = paramType;
                            if (paramType != null)
                            {
                                exists.Parameter.Value = Enum.ToObject(paramType, Convert.ToInt32(exists.Parameter.Value));
                            }
                        }
                    }
                    else if (exists.Parameter.PropertyType == PropertyType.Int)
                    {
                        exists.Parameter.Value = Convert.ToInt32(exists.Parameter.Value);
                    }
                    else if (exists.Parameter.PropertyType == PropertyType.Float)
                    {
                        exists.Parameter.Value = Convert.ToSingle(exists.Parameter.Value);
                    }
                    else if (exists.Parameter.PropertyType == PropertyType.GameObject)
                    {
                        exists.Parameter.Value = Convert.ToString(exists.Parameter.Value);
                    }
                    else if (exists.Parameter.PropertyType == PropertyType.StringArray)
                    {
                        var s = exists.Parameter.Property as JArray;
                        exists.Parameter.Property = (s == null ? exists.Parameter.Property : s.ToObject<string[]>());

                        exists.Parameter.Value = Convert.ToInt32(exists.Parameter.Value);
                    }
                    else if (exists.Parameter.PropertyType == PropertyType.IntArray)
                    {
                        var s = exists.Parameter.Property as JArray;
                        exists.Parameter.Property = (s == null ? exists.Parameter.Property : s.ToObject<int[]>());

                        exists.Parameter.Value = Convert.ToInt32(exists.Parameter.Value);
                    }
                    exists.Condition = subParam.Condition;
                    exists.Parameter.WithSubParams(subParam.Parameter.SubParams.Values.ToArray());
                }
                else
                {
                    SubParams.Add(subParam.Parameter.Name, subParam);        
                }
            }
        }

        public void SetNodeChain(NodeChain nodeChain)
        {
            NodeChain = nodeChain;
            foreach(var val in SubParams.Values)
            {
                val.SetNodeChain(nodeChain);
            }
        }
    }
}