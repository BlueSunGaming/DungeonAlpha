using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class LegacyAnimation
    {
        public AnimationDefinition UnarmedAnim;
        public AnimationDefinition WalkAnim;
        public AnimationDefinition WalkBackAnim;
        public AnimationDefinition RunAnim;
        public AnimationDefinition JumpAnim;
        public AnimationDefinition StrafeRightAnim;
        public AnimationDefinition StrafeLeftAnim;
        public AnimationDefinition TurnRightAnim;
        public AnimationDefinition TurnLeftAnim;
        public AnimationDefinition IdleAnim;
        public AnimationDefinition CombatIdleAnim;
        public AnimationDefinition TakeHitAnim;
        public AnimationDefinition FallAnim;
        public AnimationDefinition DeathAnim;
        public AnimationDefinition KnockBackAnim;
        public AnimationDefinition KnockUpAnim;


        public List<AnimationDefinition> DefaultAttackAnimations;
        public List<AnimationDefinition> Default2HAttackAnimations;
        public List<AnimationDefinition> DefaultDWAttackAnimations;
        public List<WeaponAnimationDefinition> WeaponAnimations;

        public LegacyAnimation()
        {
            UnarmedAnim = new AnimationDefinition() { Name = "Unarmed", RPGAnimationSet = RPGAnimationSet.Core};
            WalkAnim = new AnimationDefinition() { Name = "Walk", RPGAnimationSet = RPGAnimationSet.Core };
            WalkBackAnim = new AnimationDefinition() { Name = "Walk Back", RPGAnimationSet = RPGAnimationSet.Core };
            RunAnim = new AnimationDefinition() { Name = "Run", RPGAnimationSet = RPGAnimationSet.Core };
            JumpAnim = new AnimationDefinition() { Name = "Jump", RPGAnimationSet = RPGAnimationSet.Core };
            TurnRightAnim = new AnimationDefinition() { Name = "Turn Right", RPGAnimationSet = RPGAnimationSet.Core };
            TurnLeftAnim = new AnimationDefinition() { Name = "Turn Left", RPGAnimationSet = RPGAnimationSet.Core };
            StrafeRightAnim = new AnimationDefinition() { Name = "Strafe Right", RPGAnimationSet = RPGAnimationSet.Core };
            StrafeLeftAnim = new AnimationDefinition() { Name = "Strafe Left", RPGAnimationSet = RPGAnimationSet.Core };
            IdleAnim = new AnimationDefinition() { Name = "Idle", RPGAnimationSet = RPGAnimationSet.Core };
            CombatIdleAnim = new AnimationDefinition() { Name = "Combat Idle", RPGAnimationSet = RPGAnimationSet.Core };
            TakeHitAnim = new AnimationDefinition() { Name = "Take Hit", RPGAnimationSet = RPGAnimationSet.Core };
            FallAnim = new AnimationDefinition() { Name = "Falling", RPGAnimationSet = RPGAnimationSet.Core };
            DeathAnim = new AnimationDefinition() { Name = "Death", RPGAnimationSet = RPGAnimationSet.Core, WrapMode = WrapMode.ClampForever };
            KnockBackAnim = new AnimationDefinition() { Name = "Knock Back", RPGAnimationSet = RPGAnimationSet.Core };
            KnockUpAnim = new AnimationDefinition() { Name = "Knock Up", RPGAnimationSet = RPGAnimationSet.Core };

            DefaultAttackAnimations = new List<AnimationDefinition>();
            Default2HAttackAnimations = new List<AnimationDefinition>();
            DefaultDWAttackAnimations = new List<AnimationDefinition>();
            WeaponAnimations = new List<WeaponAnimationDefinition>();
        }
    }

    public class WeaponAnimationDefinition
    {
        public string WeaponTypeID;
        public List<AnimationDefinition> Animations;
        public List<AnimationDefinition> DualWieldAnimations;

        public WeaponAnimationDefinition()
        {
            WeaponTypeID = "";
            Animations = new List<AnimationDefinition>();
            DualWieldAnimations = new List<AnimationDefinition>();
        }
    }

    public class AnimationDefinition
    {
        public string Name;
        public string Animation;
        public WrapMode WrapMode;
        public float Speed;
        public bool Backwards;
        public float ImpactTime;

        public RPGAnimationSet RPGAnimationSet;
        public int WeaponTypeIndex;
        public int MecanimAnimationNumber;
        public int MecanimAnimationGroup;

        public AnimationDefinition()
        {
            SoundPath = "";
            Animation = "";
            Speed = 1.0f;
            ImpactTime = 0.7f;
            WrapMode = WrapMode.Loop;
        }

        public string SoundPath;
        [JsonIgnore]
        public AudioClip _sound ;
        [JsonIgnore]
        public AudioClip Sound
        {
            get { return _sound ?? (_sound = Resources.Load(SoundPath) as AudioClip); }
            set { _sound = value; }
        }
    }
}