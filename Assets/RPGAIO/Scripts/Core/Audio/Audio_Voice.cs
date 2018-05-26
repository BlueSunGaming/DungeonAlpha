using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;

public class Audio_Voice : AudioBase
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
            return (Rm_GameConfig.Instance.Audio.VoiceVolume / 100.0F) *
                   (Rm_GameConfig.Instance.Audio.MasterVolume / 100.0F);
        }
    }
}