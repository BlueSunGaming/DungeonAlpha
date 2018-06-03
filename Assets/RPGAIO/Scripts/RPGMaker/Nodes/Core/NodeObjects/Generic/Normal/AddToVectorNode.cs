using System;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Calculations", "Vectors")]
    public class AddToVectorNode : PropertyNode
    {
        public override string Name
        {
            get { return "Add To Vector"; }
        }

        public override string Description
        {
            get { return "Adds a value to a Vector3."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return true;
            }
        }


        public override PropertyFamily PropertyFamily
        {
            get { return PropertyFamily.Object; }
        }

        protected override bool InheritsPropertyType
        {
            get { return false; }
        }

        protected override PropertyType PropertyNodeType
        {
            get { return PropertyType.Vector3; }
        }

        public override NodeType NodeType
        {
            get
            {
                return NodeType.PropertyAndSimple;
            }
        }

        protected override void SetupParameters()
        {
            Add("Vector3", PropertyType.Vector3, null, RPGVector3.Zero, PropertySource.EnteredOrInput, PropertyFamily.Object);
            Add("Value Added", PropertyType.Vector3, null, RPGVector3.Zero, PropertySource.EnteredOrInput, PropertyFamily.Object);
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            var result =  func(Eval(nodeChain));
            return ApplyFunctionTo("Vector3", o =>
                                                  {
                                                      o = result;
                                                      return o;
                                                  });
        }

        protected override object Eval(NodeChain nodeChain)
        {
            var addValue = (RPGVector3)ValueOf("Value Added");

            var result = ApplyFunctionTo("Vector3", o =>
                                                        {
                                                            var val = new RPGVector3((Vector3)o);
                                                            val += addValue;
                                                            return val;
                                                        });

            return result;
        }
    }
}