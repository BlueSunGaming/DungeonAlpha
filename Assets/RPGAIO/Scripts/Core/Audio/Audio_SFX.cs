using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;


    public class Audio_SFX : AudioBase
    {
        protected override void Awake()
        {
            base.Awake();

            AudioSource.dopplerLevel = 1;   
        }

        protected override float Volume
        {
            get
            {
                return (Rm_GameConfig.Instance.Audio.SoundEffectVolume / 100.0F) *
                    (Rm_GameConfig.Instance.Audio.MasterVolume / 100.0F);
            }
        }
    }