using System.Collections.Generic;
using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker
{
    public class Rmh_Audio
    {
        public bool Enable2DAudio;
        public bool LoadAllAudioOnLoad;
        public bool PlayThroughSceneSwitch = true;
        public bool FadeOutMusic;
        public float FadeOutTime;
        public bool FadeInMusic;
        public float FadeInTime;
        public bool PlayUniqueMusicForBattles;
        public bool PlayUniqueMusicForScenes;
        public bool PlayUniqueMusicForDeath;

        public List<AudioContainer> GlobalPlaylist;
        public List<SceneMusic> ScenePlaylists;
        public List<AudioContainer> BattlePlaylist;
        public AudioContainer DeathMusic;
        public bool ShufflePlaylist;


        public Rmh_Audio()
        {
            ScenePlaylists = new List<SceneMusic>();
            GlobalPlaylist = new List<AudioContainer>();
            BattlePlaylist = new List<AudioContainer>();
            DeathMusic = new AudioContainer();
            LoadAllAudioOnLoad = false;
            FadeOutTime = 2.0f;
            FadeInTime = 1.0f;
            FadeInMusic = true;
            FadeOutMusic = true;
            PlayUniqueMusicForBattles = false;
            PlayUniqueMusicForDeath = false;
            PlayUniqueMusicForScenes = false;
            ShufflePlaylist = true;
            Enable2DAudio = false;
        }

        
    }

    public class SceneMusic
    {
        public string SceneName;
        public AudioContainer Music;

        public SceneMusic()
        {
            SceneName = "";
            Music = new AudioContainer();
        }
    }
}