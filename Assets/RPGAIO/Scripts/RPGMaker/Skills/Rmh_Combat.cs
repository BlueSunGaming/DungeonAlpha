using System;
using System.Collections.Generic;
using LogicSpawn.RPGMaker.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class Rmh_Combat
    {
        public const string CastAreaWithTexturePath = "RPGMakerAssets/CastPrefab";
        public bool ScaleSkillMoveByCast;
        public bool NPCsCanFight;
        public bool CanAttackNPcs;
        public bool CanAttackUnkillableNPCs;

        public bool EnableTauntSystem;
        public bool EnableFallDamage;

        public float AggroRadius;

        public bool EnableFloatingCombatText;
        public float FloatSpeed;
        public float FloatDuration;
        public float FloatDistBetweenVals;

        public List<ShaderLerpInfo> ShadersToLerp;

        public TargetStyle TargetStyle;

        public bool ShowSelected;
        public bool ShowSelectedWithTexture;
        public ImageContainer SelectedTexture;
        public ImageContainer SelectedCombatTexture;
        public string SelectedPrefabPath;
        public float SelectedYOffSet;

        public bool ShowCastArea;
        public ImageContainer CastAreaTexture;

        public string DefaultProjectilePrefabPath;
        public string DefaultMeleePrefabPath;

        //Skills
        public SkillUpgradeType DefaultUpgradeType;
        public List<SkillTypeName> SkillTypeNames;
        public List<SkillMetaDefinition> SkillMeta;
        public int SkillBarSlots;
        public bool AllowItemsOnBar;

        public bool SkipEvaluatingDmgWithNullAttack;

        public bool MetaAppliesToHealing;
        public float[] _castAreaColorArray;

        [JsonIgnore]
        private Color _castAreaColor = Color.clear;

        public bool SmartCastSkills;
        public bool AutomaticallyScaleAOE;

        [JsonIgnore]
        public Color CastAreaColor
        {
            get
            {
                if(_castAreaColor != Color.clear)
                {
                    return _castAreaColor;
                }
                return  _castAreaColor = new Color(_castAreaColorArray[0], _castAreaColorArray[1], _castAreaColorArray[2], _castAreaColorArray[3]);
            }
            set { 
                _castAreaColor = value;
                _castAreaColorArray[0] = _castAreaColor.r;
                _castAreaColorArray[1] = _castAreaColor.g;
                _castAreaColorArray[2] = _castAreaColor.b;
                _castAreaColorArray[3] = _castAreaColor.a;
            }
        }

        public Rmh_Combat()
        {
            DefaultUpgradeType = SkillUpgradeType.SkillPoints;
            TargetStyle = TargetStyle.ManualTarget;
            ScaleSkillMoveByCast = true;

            FloatSpeed = 2.0f;
            FloatDuration = 1.5f;
            FloatDistBetweenVals = 0.3f;

            AggroRadius = 25.0f;

            SmartCastSkills = false;

            AutomaticallyScaleAOE = false;

            SkipEvaluatingDmgWithNullAttack = true;

            EnableTauntSystem = true;
            NPCsCanFight = true;
            CanAttackNPcs = true;
            CanAttackUnkillableNPCs = true;
            ShadersToLerp = new List<ShaderLerpInfo>();
            SkillTypeNames = new List<SkillTypeName>();
            var skillEnumValues = Enum.GetValues(typeof(SkillType)) as SkillType[];
            for (var i = 0; i < skillEnumValues.Length; i++)
            {
                SkillTypeNames.Add(new SkillTypeName()
                                       {
                                           SkillType = skillEnumValues[i],
                                           Name = skillEnumValues[i].ToString().Replace('_', ' ')
                                       });
            }

            SkillMeta = new List<SkillMetaDefinition>();
            MetaAppliesToHealing = true;
            SelectedTexture = new ImageContainer();
            SelectedCombatTexture = new ImageContainer();
            CastAreaTexture = new ImageContainer();

            DefaultProjectilePrefabPath = "RPGMakerAssets/DefaultProjectilePrefab";
            DefaultMeleePrefabPath = "RPGMakerAssets/DefaultMeleePrefab";

            _castAreaColorArray = new [] { Color.blue.r, Color.blue.g, Color.blue.b, Color.blue.a };
            SkillBarSlots = 9;
            AllowItemsOnBar = true;
        }
    }
}