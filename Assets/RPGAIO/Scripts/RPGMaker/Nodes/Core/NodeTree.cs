using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class NodeTree
    {
        public string Name;
        public List<Node> Nodes;
        public string ID;
        public bool Required;
        public NodeTreeType Type;
        public List<NodeTreeVar> Variables;
        public NodeTree()
        {
            ID = Guid.NewGuid().ToString(); 
            Name = "New Node Tree";
            Nodes = new List<Node>();
            Variables = new List<NodeTreeVar>();
        }

        public void AddNode(Node node, Vector2 pos, int windowId, string id = "")
        {
            node.OnCreate();

            node.ID = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
            node.WindowID = windowId;
            node.Rect = new Rect(pos.x, pos.y, 50, 50);
            Nodes.Add(node); 
        }
    }

    public class NodeTreeVar
    {
        public string ID;
        public string Name;
        public float FloatValue;
        public string StringValue;
        public int IntValue;
        public bool BoolValue;
        public bool IsList;
        public object ObjectValue;
        public object DefaultValue;
        public PropertyType PropType;

        public NodeTreeVar()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New NodeTreee Variable";
            PropType = PropertyType.Int;
            StringValue = "";
            ObjectValue = null;
        }

        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            if (ObjectValue is GameObject || ObjectValue is Vector3 || ObjectValue is List<GameObject>)
            {
                ObjectValue = null;
            }
        }

        public NodeTreeVar(string name, PropertyType type) : base()
        {
            ID = Guid.NewGuid().ToString();
            StringValue = "";
            ObjectValue = null;

            Name = name;
            PropType = type;
            switch (PropType)
            {
                case PropertyType.String:
                    DefaultValue = "";
                    break;
                case PropertyType.Float:
                    DefaultValue = 0.0f;
                    break;
                case PropertyType.Int:
                    DefaultValue = 0;
                    break;
                case PropertyType.Bool:
                    DefaultValue = false;
                    break;
                case PropertyType.Vector3:
                    DefaultValue = RPGVector3.Zero;
                    break;
                case PropertyType.GameObject:
                    DefaultValue = null;
                    break;
                default:
                    DefaultValue = null;
                    break;
            }
        }

        [JsonIgnore]
        public object Value
        {
            get
            {
                if(IsList)
                {
                    return ObjectValue;
                }

                switch(PropType)
                {
                    case PropertyType.String:
                        return StringValue;
                    case PropertyType.Float:
                        return FloatValue;
                    case PropertyType.Int:
                        return IntValue;
                    case PropertyType.Bool:
                        return BoolValue;
                    default:
                        return ObjectValue;
                }
            }

            set
            {
                if(IsList)
                {
                    ObjectValue = value;
                }

                switch (PropType)
                {
                    case PropertyType.String:
                        StringValue = (string)value;
                        break;
                    case PropertyType.Float:
                        FloatValue = (float)value;
                        break;
                    case PropertyType.Int:
                        IntValue = (int)value;
                        break;
                    case PropertyType.Bool:
                        BoolValue = (bool)value;
                        break;
                    default:
                        ObjectValue = value;
                        break;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public void ResetValue()
        {
            switch (PropType)
            {
                case PropertyType.String:
                    StringValue = Convert.ToString(DefaultValue);
                    break;
                case PropertyType.Float:
                    FloatValue = Convert.ToSingle(DefaultValue);
                    break;
                case PropertyType.Int:
                    IntValue = Convert.ToInt32(DefaultValue);
                    break;
                case PropertyType.Bool:
                    BoolValue = Convert.ToBoolean(DefaultValue);
                    break;
                default:
                    ObjectValue = DefaultValue;
                    break;
            }
        }

        public NodeTreeVar Clone()
        {
            var n = new NodeTreeVar();
            n.ID = ID;
            n.Name = Name;
            n.FloatValue = FloatValue;
            n.StringValue = StringValue;
            n.IntValue = IntValue;
            n.BoolValue = BoolValue;
            n.IsList = IsList;
            n.ObjectValue = ObjectValue;
            n.DefaultValue = DefaultValue;
            n.PropType = PropType;

            return n;
        }
    }
}