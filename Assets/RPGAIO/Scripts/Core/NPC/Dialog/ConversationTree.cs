using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public class ConversationTree
    {
        public List<NPCTopic> Topics ;

        public ConversationTree()
        {
            Topics = new List<NPCTopic>();
        }
    }
}