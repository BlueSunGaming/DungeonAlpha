using System;
using System.Linq;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Node Chains", "")]
    public class NodeChainValueNode : PropertyNode
    {
        public override string Name
        {
            get { return "Get Node Chain Value"; }
        }

        public override string Description
        {
            get { return "Gets the node chain's value of specified type."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override PropertyFamily PropertyFamily
        {
            get
            {
                var isList = (bool)ValueOf("Is List?");
                if(isList) return PropertyFamily.List;

                switch (PropertyNodeType)
                {
                    case PropertyType.String:
                    case PropertyType.Float:
                    case PropertyType.Int:
                    case PropertyType.Bool:
                        return PropertyFamily.Primitive;
                    default:
                        return PropertyFamily.Object;
                }
            }
        }

        protected override PropertyType PropertyNodeType
        {
            get { return (PropertyType)ValueOf("Return Type"); }
        }

        protected override bool InheritsPropertyType
        {
            get { return false; }
        }

        protected override void SetupParameters()
        {
            Add("Node Chain", PropertyType.NodeChain, null, "", PropertySource.EnteredOnly, PropertyFamily.Object);
            Add("Return Type", PropertyType.Enum, typeof(PropertyType), PropertyType.Int, PropertySource.EnteredOnly, PropertyFamily.Object);
            //Add("Is List?", PropertyType.Bool, null, false);
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object,object> func)
        {
            var idOfChain = (string) ValueOf("Node Chain");
            var returnType = (PropertyType) ValueOf("Return Type");
            var foundChain = Rm_RPGHandler.Instance.Nodes.EventNodeChains.FirstOrDefault(n => n.CurrentNode.ID == idOfChain);
            if(foundChain != null)
            {
                var hasRoutine = foundChain.Nodes.Any(n => n.IsRoutine);
                if(hasRoutine)
                {
                    throw new Exception("Cannot get value from node chain which has a routine node inside it. All nodes must be non-routine.");
                }

                while (!foundChain.Done)
                {
                    foundChain.Evaluate();
                }

                

                object returnVal = null;
                switch (returnType)
                {
                    case PropertyType.String:
                        returnVal = foundChain.StringValue;
                        break;
                    case PropertyType.Float:
                        returnVal = foundChain.FloatValue;
                        break;
                    case PropertyType.Int:
                        returnVal = foundChain.IntValue;
                        break;
                    case PropertyType.Bool:
                        returnVal = foundChain.BoolValue;
                        break;
                    default:
                        returnVal = foundChain.ObjectValue;
                        break;
                }

                if(func != null)
                {
                    func(returnVal);
                }

                return returnVal;
            }
            
            throw new Exception("Node chain not found.");
        }
    }
}