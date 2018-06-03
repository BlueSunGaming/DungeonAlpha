namespace LogicSpawn.RPGMaker
{
    public class Rmh_Preferences
    {
        public bool EnableAutoSave;
        public bool EnableBackupOnSave;
        public bool OneBackupPerDay;
        public float AutoSaveFrequency;

        public Rmh_Preferences()
        {
            EnableAutoSave = false;
            EnableBackupOnSave = true;
            OneBackupPerDay = true;
            AutoSaveFrequency = 300;
        }
    }
}