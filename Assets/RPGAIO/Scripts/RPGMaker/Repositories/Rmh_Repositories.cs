using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker
{
    public class Rmh_Repositories
    {
        public AchievementRepository Achievements; //todo: remove this, as we use nodetrees instead
        public CraftableItemRepository CraftableItems;
        public CraftListRepository CraftLists;
        public ItemRepository Items;
        public DismantleRepository Dismantle;
        public QuestItemRepository QuestItems;
        public InteractableRepository Interactable;
        public EnemyRepository Enemies;
        public NPCVendorRepository Vendor;
        public QuestRepository Quests;
        public SkillRepository Skills;
        public StatusEffectRepository StatusEffects;
        public TalentRepository Talents;
        public WorldMapRepository WorldMap;
        public LootTableRepository LootTables;

        public Rmh_Repositories()
        {
            Achievements = new AchievementRepository();
            CraftableItems =new CraftableItemRepository();
            CraftLists = new CraftListRepository();
            Items = new ItemRepository();
            Dismantle = new DismantleRepository();
            QuestItems = new QuestItemRepository();
            Interactable = new InteractableRepository();
            Enemies = new EnemyRepository();
            Vendor = new NPCVendorRepository();
            Quests = new QuestRepository();
            Skills = new SkillRepository();
            StatusEffects = new StatusEffectRepository();
            Talents = new TalentRepository();
            WorldMap = new WorldMapRepository();
            LootTables = new LootTableRepository();
        }
    }
}