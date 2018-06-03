using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;

namespace Assets.Scripts.RPGMaker.Nodes.Core
{
    public static class NodeHelper
    {
        public static List<Node> GetChainAsNodes(Node node, NodeTree tree)
        {
            var list = new List<Node>(){node};
            GetAllLinkedNodes(node, tree, ref list);
            return list;
        }
        public static List<Node> GetChainAsNodes(Node node, List<Node> nodes)
        {
            var list = new List<Node>() { node }; 
            GetAllLinkedNodes(node, new NodeTree() { Nodes = nodes }, ref list);
            return list;
        }

        public static void GetAllLinkedNodes(Node node, NodeTree tree, ref List<Node> list)
        {
            if (node == null) return;
            var l = list;
            var nodes = GetLinkedNodes(node, tree);
            nodes = nodes.Where(n => l.FirstOrDefault(x => x.ID == n.ID) == null).ToList();
            list.AddRange(nodes);
            foreach (var x in nodes.Where(n => n != null))
            {
                GetAllLinkedNodes(x, tree, ref list);
            }
        }

        public static List<Node> GetLinkedNodes(Node node, NodeTree tree)
        {
            var list = node.NextNodeLinks.Where(x => x != null).Select(x => tree.Nodes.FirstOrDefault(n => n.ID == x.ID)).Where(x => x != null).ToList();
            if(node.Parameters.Any())
            {
                var paramNodes = node.Parameters.Values.Where(p => !string.IsNullOrEmpty(p.InputNodeId.ID)).Select(x => tree.Nodes.FirstOrDefault(n => n.ID == x.InputNodeId.ID)).Where(y => y != null);
                list.AddRange(paramNodes);

                foreach(var param in node.Parameters)
                {
                    var subParams = param.Value.SubParams.Values.ToList();
                    if(subParams.Any())
                    {
                        AddSubParams(subParams, tree, ref list);    
                    }
                }
                
            }
            return list;
        }

        private static void AddSubParams(List<SubNodeParameter> parameters, NodeTree tree, ref List<Node> list)
        {
            foreach(var subParam in parameters)
            {
                var parameter = subParam.Parameter;
                if(parameter.InputNodeId != null && !string.IsNullOrEmpty(parameter.InputNodeId.ID))
                {
                    var inputNode = tree.Nodes.FirstOrDefault(n => n.ID == parameter.InputNodeId.ID);
                    if(!list.Contains(inputNode))
                    {
                        list.Add(inputNode);
                    }
                }

                var subParams = parameter.SubParams.Values.ToList();
                if(subParams.Any())
                {
                    AddSubParams(subParams, tree, ref list);
                }
            }
        }
    }
}