using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public class NPCTopic
    {
        public int Id ;
        public string TopicDialog ;
        public string TopicDialogSoundPath ;
        public List<CharacterResponse> Responses ;

        public NPCTopic()
        {
            TopicDialog = "";
            Responses = new List<CharacterResponse>();
        }
    }
}