using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker.Objectives
{
    public class Rmh_Questing
    {
        public bool ShowQuestMarkers;

        public AudioContainer QuestComplete;
        public AudioContainer QuestStarted;
        public AudioContainer AchievementUnlocked; //todo: remove 

        public Rmh_Questing()
        {
            ShowQuestMarkers = true;
            QuestComplete = new AudioContainer();
            QuestStarted = new AudioContainer();
            AchievementUnlocked = new AudioContainer();
        }
    }
}