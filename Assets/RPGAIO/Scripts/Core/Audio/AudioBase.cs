using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class AudioBase : MonoBehaviour
{
    public string SoundID = "";
    public AudioSource AudioSource;
    protected abstract float Volume { get; }


    protected virtual void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
        AudioSource.volume = Volume;

        AudioSource.spatialBlend = Rm_RPGHandler.Instance.Audio.Enable2DAudio ? 0 : 1;
        AudioSource.rolloffMode = AudioRolloffMode.Linear;
        AudioSource.minDistance = 1;
        AudioSource.maxDistance = 25;
        AudioSource.dopplerLevel = 0;
    }

    protected virtual void Update()
    {
        AudioSource.volume = Volume;
        if (!AudioSource.isPlaying && !AudioSource.loop)
        {
            AudioPlayer.Instance.PlayOneAtATime.Remove(SoundID);
            Destroy(gameObject);
        }

        if(GameMaster.GamePaused)
        {
            AudioSource.Pause();
        }
        else
        {
            AudioSource.UnPause();
        }
    }
}