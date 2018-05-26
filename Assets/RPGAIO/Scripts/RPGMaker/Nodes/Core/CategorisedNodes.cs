using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;

namespace Assets.Scripts.RPGMaker.Nodes.Core
{
    public class OrganisedNodeTree
    {
        public List<CategorisedNodes> MainCategories;

        public OrganisedNodeTree(List<Node> filteredNodes, NodeTreeType type )
        {
            MainCategories = new List<CategorisedNodes>();
            var contextualCat = new CategorisedNodes() {CatergoryName = type.ToString()};
            var refilteredNodes = filteredNodes;

            for (int index = 0; index < refilteredNodes.Count; index++)
            {
                var node = refilteredNodes[index];
                if (!string.IsNullOrEmpty(node.TreeType) && node.TreeType == type.ToString())
                {
                    refilteredNodes.RemoveAt(index);
                    contextualCat.Nodes.Add(node);
                    index--;
                }
            }



            foreach(var node in refilteredNodes)
            {
                var nodeMainCat = node.MainCat;
                var nodeSubCat = node.SubCat;

                var mainCat = MainCategories.FirstOrDefault(f => f.CatergoryName == nodeMainCat);
	            if( mainCat == null)
	            {
	                mainCat = new CategorisedNodes() {CatergoryName = nodeMainCat};
		            MainCategories.Add(mainCat);
	            }

                var subCat = mainCat.SubCategories.FirstOrDefault(s => s.SubCatergoryName == nodeSubCat);
	            if(string.IsNullOrEmpty(nodeSubCat))
                {
                    mainCat.Nodes.Add(node);
                }
                else if( subCat == null)
	            {
		            subCat = new SubCategorisedNodes(){SubCatergoryName = nodeSubCat};
                    mainCat.SubCategories.Add(subCat);
                }

                if (subCat != null)
                {
                    subCat.Nodes.Add(node);
                }
            }

            MainCategories = MainCategories.OrderBy(m => m.CatergoryName).ToList();

            if (contextualCat.Nodes.Any())
            {
                MainCategories.Insert(0,contextualCat);
            }

            var uncategorised = MainCategories.FirstOrDefault(c => c.CatergoryName == "Uncategorised");
            if(uncategorised != null)
            {
                MainCategories.Remove(uncategorised);
                MainCategories.Add(uncategorised);
            }

            foreach(var c in MainCategories)
            {
                c.Nodes = c.Nodes.OrderBy(n => n.Tag).ToList();
                c.SubCategories = c.SubCategories.OrderBy(v => v.SubCatergoryName).ToList();
                foreach (var x in c.SubCategories)
                {
                    x.Nodes = x.Nodes.OrderBy(n => n.Tag).ToList();
                }
            }

        }

        public CategorisedNodes GetMainCat(string catergoryName)
        {
            return MainCategories.FirstOrDefault(m => m.CatergoryName == catergoryName);
        }

        public SubCategorisedNodes GetSubCat(string catergoryName, string subCatergoryName)
        {
            return GetMainCat(catergoryName).SubCategories.FirstOrDefault(s => s.SubCatergoryName == subCatergoryName);
        }
    }

    public class CategorisedNodes
    {
        public string CatergoryName;
        public List<SubCategorisedNodes> SubCategories;
        public List<Node> Nodes;
        public bool Show;

        public CategorisedNodes()
        {
            CatergoryName = "";
            SubCategories = new List<SubCategorisedNodes>();
            Nodes = new List<Node>();
            Show = true;
        }
    }

    public class SubCategorisedNodes
    {        
        public string SubCatergoryName;
        public List<Node> Nodes;
        public bool Show;

        public SubCategorisedNodes()
        {
            SubCatergoryName = "";
            Nodes = new List<Node>();
            Show = true;
        } 
    }
}