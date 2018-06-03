using LogicSpawn.RPGMaker.API;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class StartNode : SimpleNode
    {
        public ReturnType ReturnType;

        public override string Name
        {
            get { return "StartNode"; }
        }

        public override string Description
        {
            get { return "Beginning of a node chain used as a starting point for a node iteration."; }
        }

        public override string SubText
        {
            get { return "Entry point"; }
        }

        public override bool CanBeLinkedTo
        {
            get { return false; }
        }

        public override bool IsStartNode
        {
            get { return true; }
        }

        public override bool ShowTarget
        {
            get { return false; }
        }

        public override bool CanBeDeleted
        {
            get { return true; }
        }

        public override string NextNodeLinkLabel(int index)
        {
            return "Next";
        }

        protected override void SetupParameters()
        {
            //Test:
            //Add("Dialog", PropertyType.TextArea, null, "Hello");        

            //Add("Stat", PropertyType.Statistic, null, "").WithSubParams(
            //
            //    SubParam("Stat1", PropertyType.Attribute, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticId("Movement")),
            //    SubParam("Stat2", PropertyType.Statistic, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticId("Movement")),
            //    SubParam("Stat3", PropertyType.Vital, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticId("Movement")),
            //    SubParam("Stat4", PropertyType.Trait, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticId("Movement"))
            //);

            #region mass field test

            //
            //            Add("Stat", PropertyType.Statistic, null, "").WithSubParams(
            //
            //                SubParam("Stat1", PropertyType.Attribute, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat2", PropertyType.Statistic, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat3", PropertyType.Vital, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat4", PropertyType.Trait, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat5", PropertyType.StatusEffect, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat6", PropertyType.Skill, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat7", PropertyType.Talent, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat8", PropertyType.TraitExpDefinition, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat9", PropertyType.ExpGainedDefinition, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat10", PropertyType.PlayerLevellingDefintion, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat11", PropertyType.TalentGroup, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat12", PropertyType.Item, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat13", PropertyType.CraftableItem, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat14", PropertyType.QuestItem, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat15", PropertyType.Rm_ClassDefinition, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat16", PropertyType.Rmh_CustomVariable, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat17", PropertyType.ReputationDefinition, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat18", PropertyType.SkillMetaDefinition, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat19", PropertyType.Quest, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat20", PropertyType.NonPlayerCharacter, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat21", PropertyType.Interactable, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat22", PropertyType.SlotDefinition, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat23", PropertyType.WeaponTypeDefinition, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat24", PropertyType.RarityDefinition, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat25", PropertyType.CombatCharacter, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat26", PropertyType.Rm_LootTable, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat27", PropertyType.Achievement, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat28", PropertyType.MonsterTypeDefinition, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement")),
            //                SubParam("Stat29", PropertyType.Rm_NodeTree, null, "").If(p => (string) p.Value == RPG.Stats.GetStatisticID("Movement"))
            //                );

            #endregion

            //
            //            Add("StringArr2", PropertyType.IntPopup, new []{1,2,3,4,5}, 2);
            //
            //            Add("String1", PropertyType.String, null, "_ default _");
            //
            //            Add("String2", PropertyType.String, null, "_ default _")
            //                .WithSubParams(
            //                    SubParam("String3", PropertyType.String, null, "_ default _").If(p => (string)p.Value == "Show")
            //                        .WithSubParams(
            //                            SubParam("String4", PropertyType.Int, null, 10).Always().If(p => (string)p.Value == "Woah"),
            //                            SubParam("String5", PropertyType.Int, null, 10).Always()
            //                                .WithSubParams(
            //                                    SubParam("EEEEK", PropertyType.Bool, null, true).If(p => (int)p.Value == 5)
            //                                ),
            //                            SubParam("String6", PropertyType.String, null, "_ default _").Always()
            //                        ),
            //                    SubParam("Enum",PropertyType.Enum, typeof(SkillType), SkillType.Area_Of_Effect).Always(),
            //                    SubParam("Attr",PropertyType.Attribute, null , "").Always(),
            //                );
        }

        protected override void Eval(NodeChain nodeChain)
        {
            //            var a1 = (int)Parameter("String2").SubParam("String3").ValueOf("String4");
            //            var a2 = (int)Parameter("String2").SubParam("String3").ValueOf("String5");
            //            Debug.Log(a1 + a2);
        }
    }
}