using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public abstract class PropertyNode : Node
    {
        public PropertyType PropertyType
        {
            get
            {
                return InheritsPropertyType ? InheritedPropertyType : PropertyNodeType;
            }
        }

        public PropertyType InheritedPropertyType;
        public abstract PropertyFamily PropertyFamily { get; }

        protected abstract bool InheritsPropertyType { get; }
        protected abstract PropertyType PropertyNodeType { get; }

        public override NodeType NodeType
        {
            get
            {
                return NodeType.Property;
            }
        }

        public override bool HasMaxNextLinks
        {
            get
            {
                return true;
            }
        }

        public override int MaxNextLinks
        {
            get
            {
                return 1;
            }
        }
        public override bool CanRemoveLinks
        {
            get
            {
                return false;
            }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return false;
            }
        }

        public override string NextNodeLinkLabel(int index)
        {
            return NodeType == NodeType.Property ? "" : "Next / Value";
        }

        protected override object EvaluateNode(NodeChain nodeChain)
        {
            return Eval(nodeChain);
        }

        public abstract object EvaluateInput(NodeChain nodeChain, Func<object, object> func);

        protected virtual object Eval(NodeChain nodeChain)
        {
            throw new NotImplementedException();
        }

        protected override void SetupNextLinks()
        {
            NextNodeLinks = new List<StringField> { new StringField { ID = "" } };
        }

        protected PropertyNode()
        {
            InheritedPropertyType = PropertyType.None;
        }

    }
    public class NodeTreeVarNode : PropertyNode
    {
        public string VariableId;
        public string NewName;
        public PropertyType NodePropertyType;
        public PropertyFamily NodePropertyFamily;

        [JsonIgnore]
        protected override bool InheritsPropertyType { get { return false; } }
        [JsonIgnore]
        protected override PropertyType PropertyNodeType { get { return NodePropertyType; } }
        [JsonIgnore]
        public override PropertyFamily PropertyFamily { get { return NodePropertyFamily; } }
        [JsonIgnore]
        public override NodeType NodeType { get { return NodeType.Property; } }

        public override string Name
        {
            get { return NewName; }
        }

        public override string Description
        {
            get { return "Use this node as a property parameter to initialise a variable."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool ShowInSearch
        {
	        get { return false; }
        }

        public override bool HasMaxNextLinks
        {
            get
            {
                return true;
            }
        }

        public override int MaxNextLinks
        {
            get
            {
                return 1;
            }
        }
        public override bool CanRemoveLinks
        {
            get
            {
                return false;
            }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return false;
            }
        }

        protected override object EvaluateNode(NodeChain nodeChain)
        {
            return true;
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            var variable = nodeChain.GetVariable(VariableId);
            if(variable != null)
            {
                var result = func(variable.Value);
                variable.Value = result;
                return result;
            }

            return null;
        }

        protected override void SetupParameters()
        {
        }

        public NodeTreeVarNode()
        {
            InheritedPropertyType = PropertyType.None;
            NewName = "Var Node";
        }

    }

    public class NodeChainIntNode : PropertyNode
    {
        [JsonIgnore]
        protected override bool InheritsPropertyType { get { return false; } }
        [JsonIgnore]
        protected override PropertyType PropertyNodeType { get { return PropertyType.Int; } }
        [JsonIgnore]
        public override PropertyFamily PropertyFamily { get { return PropertyFamily.Primitive; } }
        [JsonIgnore]
        public override NodeType NodeType { get { return NodeType.Property; } }

        public override string Name
        {
            get { return "Node Chain - Int"; }
        }

        public override string Description
        {
            get { return "Use this node as a property parameter that's tied to the node chain's int."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool HasMaxNextLinks
        {
            get
            {
                return true;
            }
        }

        public override int MaxNextLinks
        {
            get
            {
                return 1;
            }
        }
        public override bool CanRemoveLinks
        {
            get
            {
                return false;
            }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return false;
            }
        }

        protected override object EvaluateNode(NodeChain nodeChain)
        {
            return true;
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            if(func != null)
            {
                var result = func(nodeChain.IntValue);
                nodeChain.IntValue = (int)result;
            }

            return nodeChain.IntValue;
        }

        protected override void SetupNextLinks()
        {
            NextNodeLinks = new List<StringField> { new StringField { ID = "" } };
        }

        protected override void SetupParameters()
        {
        }
        
    }

    public class NodeChainFloatNode : PropertyNode
    {
        [JsonIgnore]
        protected override bool InheritsPropertyType { get { return false; } }
        [JsonIgnore]
        protected override PropertyType PropertyNodeType { get { return PropertyType.Float; } }
        [JsonIgnore]
        public override PropertyFamily PropertyFamily { get { return PropertyFamily.Primitive; } }
        [JsonIgnore]
        public override NodeType NodeType { get { return NodeType.Property; } }

        public override string Name
        {
            get { return "Node Chain - Float"; }
        }

        public override string Description
        {
            get { return "Use this node as a property parameter that's tied to the node chain's int."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool HasMaxNextLinks
        {
            get
            {
                return true;
            }
        }

        public override int MaxNextLinks
        {
            get
            {
                return 1;
            }
        }
        public override bool CanRemoveLinks
        {
            get
            {
                return false;
            }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return false;
            }
        }

        protected override object EvaluateNode(NodeChain nodeChain)
        {
            return true;
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            if(func != null)
            {
                var result = func(nodeChain.FloatValue);
                nodeChain.FloatValue = (float)result;
            }

            return nodeChain.FloatValue;
        }

        protected override void SetupNextLinks()
        {
            NextNodeLinks = new List<StringField> { new StringField { ID = "" } };
        }

        protected override void SetupParameters()
        {
        }
        
    }
    public class TimeScaleNode : PropertyNode
    {
        [JsonIgnore]
        protected override bool InheritsPropertyType { get { return false; } }
        [JsonIgnore]
        protected override PropertyType PropertyNodeType { get { return PropertyType.Float; } }
        [JsonIgnore]
        public override PropertyFamily PropertyFamily { get { return PropertyFamily.Primitive; } }
        [JsonIgnore]
        public override NodeType NodeType { get { return NodeType.Property; } }

        public override string Name
        {
            get { return "Timescale"; }
        }

        public override string Description
        {
            get { return "Tied to Unity3d Time.timeScale."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool HasMaxNextLinks
        {
            get
            {
                return true;
            }
        }

        public override int MaxNextLinks
        {
            get
            {
                return 1;
            }
        }
        public override bool CanRemoveLinks
        {
            get
            {
                return false;
            }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return false;
            }
        }

        protected override object EvaluateNode(NodeChain nodeChain)
        {
            return true;
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            if(func != null)
            {
                var result = func(Time.timeScale);
                Time.timeScale = (float)result;
            }

            return Time.timeScale;
        }

        protected override void SetupNextLinks()
        {
            NextNodeLinks = new List<StringField> { new StringField { ID = "" } };
        }

        protected override void SetupParameters()
        {
        }
        
    }
    public class NodeChainStringNode : PropertyNode
    {
        [JsonIgnore]
        protected override bool InheritsPropertyType { get { return false; } }
        [JsonIgnore]
        protected override PropertyType PropertyNodeType { get { return PropertyType.String; } }
        [JsonIgnore]
        public override PropertyFamily PropertyFamily { get { return PropertyFamily.Primitive; } }
        [JsonIgnore]
        public override NodeType NodeType { get { return NodeType.Property; } }

        public override string Name
        {
            get { return "Node Chain - String"; }
        }

        public override string Description
        {
            get { return "Use this node as a property parameter that's tied to the node chain's int."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool HasMaxNextLinks
        {
            get
            {
                return true;
            }
        }

        public override int MaxNextLinks
        {
            get
            {
                return 1;
            }
        }
        public override bool CanRemoveLinks
        {
            get
            {
                return false;
            }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return false;
            }
        }

        protected override object EvaluateNode(NodeChain nodeChain)
        {
            return true;
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            if(func != null)
            {
                var result = func(nodeChain.StringValue);
                nodeChain.StringValue = (string)result;
            }

            return nodeChain.StringValue;
        }

        protected override void SetupNextLinks()
        {
            NextNodeLinks = new List<StringField> { new StringField { ID = "" } };
        }

        protected override void SetupParameters()
        {
        }
        
    }
    public class NodeChainBoolNode : PropertyNode
    {
        [JsonIgnore]
        protected override bool InheritsPropertyType { get { return false; } }
        [JsonIgnore]
        protected override PropertyType PropertyNodeType { get { return PropertyType.Bool; } }
        [JsonIgnore]
        public override PropertyFamily PropertyFamily { get { return PropertyFamily.Primitive; } }
        [JsonIgnore]
        public override NodeType NodeType { get { return NodeType.Property; } }

        public override string Name
        {
            get { return "Node Chain - Bool"; }
        }

        public override string Description
        {
            get { return "Use this node as a property parameter that's tied to the node chain's boolean value."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool HasMaxNextLinks
        {
            get
            {
                return true;
            }
        }

        public override int MaxNextLinks
        {
            get
            {
                return 1;
            }
        }
        public override bool CanRemoveLinks
        {
            get
            {
                return false;
            }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return false;
            }
        }

        protected override object EvaluateNode(NodeChain nodeChain)
        {
            return true;
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            if(func != null)
            {
                var result = func(nodeChain.BoolValue);
                nodeChain.BoolValue = (bool)result;
            }

            return nodeChain.BoolValue;
        }

        protected override void SetupParameters()
        {
        }
        
    }
    public class NodeChainObjectNode : PropertyNode
    {
        [JsonIgnore]
        protected override bool InheritsPropertyType { get { return false; } }
        [JsonIgnore]
        protected override PropertyType PropertyNodeType { get { return PropertyType.Any; } }
        [JsonIgnore]
        public override PropertyFamily PropertyFamily { get { return PropertyFamily.Any; } }
        [JsonIgnore]
        public override NodeType NodeType { get { return NodeType.Property; } }

        public override string Name
        {
            get { return "Node Chain - Object"; }
        }

        public override string Description
        {
            get { return "Use this node as a property parameter that's tied to the node chain's boolean value."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool HasMaxNextLinks
        {
            get
            {
                return true;
            }
        }

        public override int MaxNextLinks
        {
            get
            {
                return 1;
            }
        }
        public override bool CanRemoveLinks
        {
            get
            {
                return false;
            }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return false;
            }
        }

        protected override object EvaluateNode(NodeChain nodeChain)
        {
            return true;
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            if(func != null)
            {
                object result;

                try
                {
                    result = func(nodeChain.BoolValue);
                }
                catch
                {
                    Debug.LogError("[RPGAIO] Error accessing/modifying node chain object value. You may be attempting to access a different type, or a list instead of a singular object.");
                    throw;
                }
                
                nodeChain.BoolValue = (bool)result;
            }

            return nodeChain.BoolValue;
        }

        protected override void SetupParameters()
        {
        }
    }
}