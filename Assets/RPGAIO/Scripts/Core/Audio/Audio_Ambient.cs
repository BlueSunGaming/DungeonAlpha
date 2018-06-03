using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;


public class Audio_Ambient : AudioBase
{
    protected override float Volume
    {
        get
        {
            return (Rm_GameConfig.Instance.Audio.AmbientVolume / 100.0F) *
              (Rm_GameConfig.Instance.Audio.MasterVolume / 100.0F);
        }
    }
}