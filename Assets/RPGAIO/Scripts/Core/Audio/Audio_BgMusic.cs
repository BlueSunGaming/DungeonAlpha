using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;

    public class Audio_BgMusic : AudioBase
    {
        private bool Fading;
        private float _volume;
        protected override float Volume
        {
            get
            {
                return Fading ? _volume : (Rm_GameConfig.Instance.Audio.MusicVolume / 100.0F) *
                  (Rm_GameConfig.Instance.Audio.MasterVolume / 100.0F);
            }
        }

        protected override void Update()    
        {
            base.Update();

            if (!AudioSource.loop && Rm_RPGHandler.Instance.Audio.FadeOutMusic && (AudioSource.clip.length - AudioSource.time) <= Rm_RPGHandler.Instance.Audio.FadeOutTime)
            {
                if(!Fading)
                {
                    Debug.Log("ey");
                    _volume = Volume;
                    Fading = true;
                }
            }

            if(Fading)
            {
                _volume -= 1 * (Time.deltaTime / Rm_RPGHandler.Instance.Audio.FadeOutTime);
            }
        }
    }