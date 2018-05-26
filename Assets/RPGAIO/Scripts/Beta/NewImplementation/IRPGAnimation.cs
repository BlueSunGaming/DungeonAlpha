using LogicSpawn.RPGMaker.Core;
using UnityEngine;

namespace Assets.Scripts.Beta.NewImplementation
{
    public interface IRPGAnimation
    {
        void CrossfadeAnimation(AnimationDefinition animDef);
        void CrossfadeAnimation(string animationname, float speed = 1.0f, WrapMode wrapMode = WrapMode.Loop, bool backwards = false);
        Animation Animation { get; }
    }
}