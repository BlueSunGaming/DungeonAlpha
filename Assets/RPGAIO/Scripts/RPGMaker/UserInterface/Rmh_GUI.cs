namespace LogicSpawn.RPGMaker
{
    public class Rmh_GUI
    {
        //todo: GUI

        //Npc Dialog
        public bool ShowNPCIcon;
        //Tooltips
        public bool DisplayTTForNPC;
        public bool DisplayTTForItems;
        public bool DisplayTTForEnemies;
        public bool DisplayTTForInteractables;
        public bool DisplayTTForSkills;
        public bool ShowMonsterType;

        public Rmh_GUI()
        {
            ShowMonsterType = true;
            ShowNPCIcon = true;
            DisplayTTForNPC = true;
            DisplayTTForItems = true;
            DisplayTTForEnemies = true;
            DisplayTTForInteractables = true;
            DisplayTTForSkills = true;
        }
    }
}