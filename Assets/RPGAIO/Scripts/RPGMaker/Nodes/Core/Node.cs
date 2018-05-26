using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Assets.Scripts.Exceptions;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Uncategorised","")]
    [NodeCategoryTree("")]
    public abstract class Node
    {
        public int WindowID { get; set; }
        public string ID;
        public string NodeChainName;
        public string Identifier { get; set; }
        public NodeTarget Target { get; set; } //TODO: REMOVE THIS
        public List<StringField> PrevNodeLinks { get; set; }
        public List<StringField> NextNodeLinks { get; set; }
        public Dictionary<string,NodeParameter> Parameters { get; set; }

        [JsonIgnore]
        public abstract NodeType NodeType { get; }
        [JsonIgnore]
        public abstract string Name { get; }
        [JsonIgnore]
        public abstract string Description { get; }
        [JsonIgnore]
        public abstract string SubText { get; }
        [JsonIgnore]
        public abstract bool HasMaxNextLinks { get; }
        [JsonIgnore]
        public virtual int MaxNextLinks { get { return 25; } }
        [JsonIgnore]
        public virtual bool CanBeDeleted { get { return true; } }
        [JsonIgnore]
        public virtual bool ShowTarget { get { return false; } }
        [JsonIgnore]
        public virtual bool ShowInSearch { get { return true; } }
        [JsonIgnore]
        public abstract bool CanRemoveLinks { get; }
        [JsonIgnore]
        public virtual bool IsStartNode { get { return false; } }

        [JsonIgnore]
        public string MainCat
        {
            get { return GetType().GetAttributeValue((NodeCategoryAttribute n) => n.MainCategory); }
        }
        
        [JsonIgnore]
        public string SubCat
        {
            get { return GetType().GetAttributeValue((NodeCategoryAttribute n) => n.SubCategory); }
        }

        [JsonIgnore]
        public string TreeType
        {
            get { return GetType().GetAttributeValue((NodeCategoryTreeAttribute n) => n.Tree); }
        }

        private bool _started;
        private bool _isDone;
        [JsonIgnore]
        public bool Done { get { return _isDone; } }
        [JsonIgnore]
        public virtual bool IsRoutine { get { return false; } }
        [JsonIgnore]
        public virtual bool CanBeLinkedTo { get { return true; } }


        public SerializableRect SerializableRect;

        [JsonIgnore]
        public NodeChain NodeChain;

        [JsonIgnore]
        public Rect Rect
        {
            get { return SerializableRect.Rect; }
            set { SerializableRect.Rect = value; }
        }

        [JsonIgnore]
        public string Tag
        {
            get
            {
                var n = this;

                if(n.IsRoutine && n is IntComparison)
                {
                    return "<color=orange><b>C/R</b></color> ";
                }

                if (n.IsRoutine)
                {
                    return "<color=brown><b>R</b></color> ";
                }
                if (n is IntComparison)
                {
                    return "<color=lime><b>C</b></color> ";
                }
                if (n.NodeType == NodeType.Property)
                {
                    return "<color=cyan><b>P</b></color> ";
                }
                if (n.NodeType == NodeType.PropertyAndSimple)
                {
                    return "<color=blue><b>S/P</b></color> ";
                }

                return "<color=silver><b>S</b></color> ";
            }
        }

        protected Node()
        {
            ID = Guid.NewGuid().ToString();
            PrevNodeLinks = new List<StringField>();
            SerializableRect = new SerializableRect(0, 0, 100, 100);
            Parameters = new Dictionary<string, NodeParameter>();
            NodeChainName = "NodeChain";
        }

        public virtual string NextNodeLinkLabel(int index)
        {
            if (NodeType == NodeType.Simple || NodeType == NodeType.Options)
            {
                return "";
            }

            if (NodeType == NodeType.TrueOrFalse)
            {
                return index == 0 ? "True" : "False";
            }

            return "";
        }

        public void AddNodeLinkSlot()
        {
            NextNodeLinks.Add(new StringField());
        }

        [OnDeserialized]
        public void LoadParameters(StreamingContext context)
        {
            SetupParameters();
        }

        public string Evaluate(NodeChain nodeChain)
        {
            //If not a routine, or a completed routine:
            if(!IsRoutine || (IsRoutine && _started && Done))
            {
                var result = EvaluateNode(nodeChain);
                string nextNodeId = null;

                if(this is EndNode)
                {
                    return null;
                }

                if (NodeType == NodeType.Simple)
                {
                    nextNodeId = NextNodeLinks[0].ID;
                }
                else if (NodeType == NodeType.TrueOrFalse)
                {
                    var returnedTrue = (bool)result;
                    nextNodeId = returnedTrue ? NextNodeLinks[0].ID : NextNodeLinks[1].ID;
                }
                else if (NodeType == NodeType.Options)
                {
                    if ((int)result != -1)
                    {
                        var index = (int)result;
                        nextNodeId = NextNodeLinks[index].ID;
                    }
                }
                else
                {
                    nextNodeId = NextNodeLinks[0].ID;
                }

                return nextNodeId;
            }
            else
            {
                if (!_started)
                {
                    Start(nodeChain);
                }

                return ID;
            }
        }

        protected NodeParameter Parameter(string paramName)
        {
            return Parameters.ContainsKey(paramName) ? Parameters[paramName] : null;
        }

        public object ValueOf(string paramName)
        {
            var param = Parameters.ContainsKey(paramName) ? Parameters[paramName] : null;
            if (param == null) throw new NodeParameterNotFoundException("Node paramter not found.");

            if(param.Source == PropertySource.EnteredOnly)
            {
                return param.Value;
            }
            else if(param.Source == PropertySource.InputOnly)
            {
                //get the first properselector/etc
                if(!string.IsNullOrEmpty(param.InputNodeId.ID))
                {
                    return ValueOfInput(param);
                }
                else
                {
                    throw new NodeParameterMissingException("Node paramater has a required parameter [" + paramName + "] which is missing or not attached as an input.");
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

        public object ApplyFunctionTo(string paramName, Func<object, object> func )
        {
            var param = Parameters.ContainsKey(paramName) ? Parameters[paramName] : null;
            if (param == null) throw new NodeParameterNotFoundException("Node paramter not found.");

            var nodeChain = NodeChain;
            var inputNodeId = param.InputNodeId.ID;
            var inputNode = nodeChain.Nodes.FirstOrDefault(n => n.ID == inputNodeId) as PropertyNode;
            var input = inputNode.EvaluateInput(nodeChain, func);
            return input;
        }

        protected NodeParameter Add(string name, PropertyType propertyType, object property, object defaultValue, PropertySource source = PropertySource.EnteredOnly, PropertyFamily familyType = PropertyFamily.Primitive)
        {
            NodeParameter exists;
            Parameters.TryGetValue(name, out exists);
            if (exists != null)
            {
                if (exists.PropertyType == PropertyType.Enum)
                {
                    if (!(exists.Property is Type))
                    {
                        var paramType = Type.GetType((string)exists.Property);
                        if (paramType != null)
                        {
                            exists.Value = Enum.ToObject(paramType, Convert.ToInt32(exists.Value));
                        }
                    }
                }
                else if(exists.PropertyType == PropertyType.Int)
                {
                    exists.Value = Convert.ToInt32(exists.Value);
                }
                else if(exists.PropertyType == PropertyType.Float)
                {
                    exists.Value = Convert.ToSingle(exists.Value);
                }
                else if(exists.PropertyType == PropertyType.GameObject)
                {
                    exists.Value = Convert.ToString(exists.Value);
                }
                else if (exists.PropertyType == PropertyType.StringArray)
                {
                    var s = exists.Property as JArray;
                    exists.Property = (s == null ? exists.Property : s.ToObject<string[]>());

                    exists.Value = Convert.ToInt32(exists.Value);
                }
                else if (exists.PropertyType == PropertyType.IntArray)
                {
                    var s = exists.Property as JArray;
                    exists.Property = (s == null ? exists.Property : s.ToObject<int[]>());

                    exists.Value = Convert.ToInt32(exists.Value);
                }
                return exists;
            }
                

            var newParam = new NodeParameter()
            {
                Name = name,
                PropertyType = propertyType,
                Property = property,
                Value = defaultValue,
                SubParams = new Dictionary<string, SubNodeParameter>(),
                Source = source,
                PropertyFamily = familyType
            };


            Parameters.Add(newParam.Name, newParam);
            return newParam;
        }

        protected SubNodeParameter SubParam(string name, PropertyType propertyType, object property, object defaultValue, PropertySource source = PropertySource.EnteredOnly, PropertyFamily familyType = PropertyFamily.Primitive)
        {
            var newParam = new SubNodeParameter()
            {
                Parameter = new NodeParameter()
                {


                    Name = name,
                    PropertyType = propertyType,
                    Property = property,
                    Value = defaultValue,
                    Source = source,
                    PropertyFamily = familyType,
                    SubParams = new Dictionary<string, SubNodeParameter>()
                }
            };
            
            return newParam;
        }

        public void OnCreate()
        {
            SetupNextLinks();
            SetupParameters();
        }

        protected abstract object EvaluateNode(NodeChain nodeChain);
        protected abstract void SetupNextLinks();
        protected abstract void SetupParameters();

        public void Start(NodeChain nodeChain)
        {
            _started = true;

            if(!IsRoutine)
            {
                Debug.LogError("Trying to start a routine when node is not marked as one.");
                return;
            }

            GetObject.EventHandler.StartCoroutine(StartRoutine(nodeChain));
        }

        private IEnumerator StartRoutine(NodeChain nodeChain)
        {
            _isDone = false;
            yield return GetObject.EventHandler.StartCoroutine(Routine(nodeChain));
            _isDone = true;
        }

        public virtual IEnumerator Routine(NodeChain nodeChain)
        {
            yield return null;
        }

        public void SetNodeChain(NodeChain nodeChain)
        {
            NodeChain = nodeChain;
            foreach(var val in Parameters.Values)
            {
                val.SetNodeChain(nodeChain);
            }
        }

        
    }
}