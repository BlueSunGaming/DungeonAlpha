namespace LogicSpawn.RPGMaker.Core
{
    public class NodeCategoryAttribute : System.Attribute
    {
        public string MainCategory { get; set; }
        public string SubCategory { get; set; }

        public NodeCategoryAttribute(string mainCategory, string subCategory)
        {
            MainCategory = mainCategory;
            SubCategory = subCategory;
        }
    }
    public class NodeCategoryTreeAttribute : System.Attribute
    {
        public string Tree { get; set; }

        public NodeCategoryTreeAttribute(NodeTreeType type)
        {
            Tree = type.ToString();
        }
        public NodeCategoryTreeAttribute(string type)
        {
            Tree = type;
        }
    }
}