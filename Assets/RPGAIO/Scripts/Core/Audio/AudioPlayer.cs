using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//todo: take hit and death sounds not implemented.
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        private bool Playing;

        public List<AudioContainer> GlobalPlaylist
        {
            get { return Rm_RPGHandler.Instance.Audio.GlobalPlaylist; }
        }
        public List<AudioContainer> BattlePlaylist
        {
            get { return Rm_RPGHandler.Instance.Audio.BattlePlaylist; }
        }
        public AudioContainer DeathMusic
        {
            get { return Rm_RPGHandler.Instance.Audio.DeathMusic; }
        }
        public List<SceneMusic> SceneLists
        {
            get { return Rm_RPGHandler.Instance.Audio.ScenePlaylists; }
        }
        
        private float MusicVolume
        {
            get
            {
                return (Rm_GameConfig.Instance.Audio.MusicVolume/100.0F)*
                       (Rm_GameConfig.Instance.Audio.MasterVolume / 100.0F);
            }
        }

        public List<AudioContainer> CurrentPlaylist;

        public int CurrentSong = 0;
        public bool FadeMusic;
        public bool FadeMusicIn;
        public AudioSource Speaker;

        public static AudioPlayer _instance;
        public static AudioPlayer Instance { get { return _instance; } }

        //
        public List<string> PlayOneAtATime;

        public List<AudioContainer> PrevSongPlaylist;
        public int PrevSongIndex;
        public int PrevSongTimeSamples;
        private bool SwitchedToBattle;
        private bool PlayedDeathMusic;
        public PlaylistMode CurrentPlayListMode;
        public PlaylistMode PrevSongPlaylistMode;

        void Awake()
        {
            _instance = this;
            PlayOneAtATime = new List<string>();

            Speaker = GetComponent<AudioSource>();
            Speaker.spatialBlend = 1;
            Speaker.rolloffMode = AudioRolloffMode.Linear;
            Speaker.minDistance = 1;
            Speaker.maxDistance = 25;
            Speaker.dopplerLevel = 0;
            Speaker.volume = (Rm_GameConfig.Instance.Audio.MusicVolume / 100.0F) *
                  (Rm_GameConfig.Instance.Audio.MasterVolume / 100.0F);

            if (Rm_RPGHandler.Instance.Audio.LoadAllAudioOnLoad)
            {
                foreach (var audioF in GlobalPlaylist)
                {
                    audioF.Audio = Resources.Load(audioF.AudioPath) as AudioClip;
                }
                foreach (var audioF in BattlePlaylist)
                {
                    audioF.Audio = Resources.Load(audioF.AudioPath) as AudioClip;
                }

                DeathMusic.Audio = Resources.Load(DeathMusic.AudioPath) as AudioClip;

                foreach (var audioF in SceneLists)
                {
                    audioF.Music.Audio = Resources.Load(audioF.Music.AudioPath) as AudioClip;
                }
            }

            //note: hack: for some reason the first audio doesn't loop
            var n = new AudioContainer {AudioPath = "Audio/gun"};
            Destroy(Play(n.Audio, AudioType.SoundFX, Vector3.zero,null, "", 10));

        }

        void Update()
        {
            if (HaveBattleMusic() && GetObject.PlayerMono != null && GetObject.PlayerMono.Controller.InCombat && !SwitchedToBattle) //switched to battle
            {
                SwitchedToBattle = true;
                PrevSongTimeSamples = Speaker.timeSamples;
                PrevSongIndex = CurrentSong;
                PrevSongPlaylist = CurrentPlaylist;
                PrevSongPlaylistMode = CurrentPlayListMode;

                StopMusic();

                CurrentPlaylist = Rm_RPGHandler.Instance.Audio.BattlePlaylist;
                CurrentPlayListMode = PlaylistMode.Battle;
                CurrentSong = 0;

                StartCoroutine("PlayMusic", false);
            }            

            //todo: cinematics
            if (HaveBattleMusic() && GetObject.PlayerMono != null && !GetObject.PlayerMono.Controller.InCombat && SwitchedToBattle)
            {
                SwitchedToBattle = false;
                CurrentPlaylist = PrevSongPlaylist;
                CurrentPlayListMode = PrevSongPlaylistMode;
                CurrentSong = PrevSongIndex;
                StopMusic();
                StartCoroutine("PlayMusic", true);
            }

            if (HaveDeathMusic() && GetObject.PlayerMono != null && !GetObject.PlayerMono.Character.Alive && !PlayedDeathMusic) //switched to battle
            {
                if(SwitchedToBattle)
                {
                    SwitchedToBattle = false;
                    CurrentPlaylist = PrevSongPlaylist;
                    CurrentPlayListMode = PrevSongPlaylistMode;
                    CurrentSong = PrevSongIndex;
                    StopMusic();
                }

                PlayedDeathMusic = true;
                PrevSongTimeSamples = Speaker.timeSamples;
                PrevSongIndex = CurrentSong;
                PrevSongPlaylist = CurrentPlaylist;
                PrevSongPlaylistMode = CurrentPlayListMode;

                StopMusic();

                CurrentPlaylist = new List<AudioContainer>() { DeathMusic };
                CurrentPlayListMode = PlaylistMode.Death;
                CurrentSong = 0;

                StartCoroutine("PlayMusic", false);
            }

            if (HaveDeathMusic() && GetObject.PlayerMono != null && GetObject.PlayerMono.Character.Alive && PlayedDeathMusic) //switched to battle
            {
                PlayedDeathMusic = false;
                StopMusic();
                FadeMusic = false;
                FadeMusicIn = true;
                StartAudio();
            }

            if(FadeMusic)
            {
                Speaker.volume -= (MusicVolume / Rm_RPGHandler.Instance.Audio.FadeOutTime) * Time.deltaTime;
            }
            else if(FadeMusicIn)
            {
                Speaker.volume += (MusicVolume / Rm_RPGHandler.Instance.Audio.FadeInTime) * Time.deltaTime;
            }
            else
            {
                Speaker.volume = MusicVolume;
            }

            if (GameMaster.CutsceneActive)
            {
                Speaker.Pause();
            }
            else
            {
                Speaker.UnPause();
            }
        }

        private bool HaveDeathMusic()
        {
            return Rm_RPGHandler.Instance.Audio.PlayUniqueMusicForDeath && DeathMusic != null;
        }

        private bool HaveBattleMusic()
        {
            return Rm_RPGHandler.Instance.Audio.PlayUniqueMusicForBattles && BattlePlaylist.Count > 0;
        }

        private void StopMusic()
        {
            StopCoroutine("PlayMusic");
            Playing = false;
        }

        //Todo: Called from GameMaster
        public void StartAudio()
        {
            Speaker = GetComponent<AudioSource>();
            Speaker.spatialBlend = 1;
            Speaker.rolloffMode = AudioRolloffMode.Linear;
            Speaker.minDistance = 1;
            Speaker.maxDistance = 25;
            Speaker.dopplerLevel = 0;
            Speaker.volume = (Rm_GameConfig.Instance.Audio.MusicVolume / 100.0F) *
                  (Rm_GameConfig.Instance.Audio.MasterVolume / 100.0F);

            CurrentPlaylist = GlobalPlaylist;
            CurrentPlayListMode = PlaylistMode.Global;

            bool playSceneMusic = false;
            //figure out playlist
            if(Rm_RPGHandler.Instance.Audio.PlayUniqueMusicForScenes)
            {
                var loadedScene = SceneManager.GetActiveScene().name;
                var allSceneMusic = SceneLists.Where(s => s.SceneName == loadedScene).ToList();
                if(allSceneMusic.Count > 0)
                {
                    playSceneMusic = true;
                    CurrentPlaylist = allSceneMusic.Select(a => a.Music).ToList();
                    CurrentPlayListMode = PlaylistMode.Scene;
                    
                }
            }

            if(CurrentPlaylist.Count > 0 && (!Playing || playSceneMusic))
            {
                if (Rm_RPGHandler.Instance.Audio.ShufflePlaylist)
                {
                    CurrentSong = Random.Range(0, CurrentPlaylist.Count);
                }
                else
                {
                    CurrentSong = 0;
                }

                StartCoroutine("PlayMusic", false);
            }
        }

        IEnumerator PlayMusic(bool resumingPrevSong)
        {
            Playing = true;
            Speaker.clip = null;
            //Use Global/Scene and then use loaded or resource.load(string)

            if (CurrentPlaylist.Count == 0 || CurrentPlaylist[CurrentSong].Audio == null)
            {
                StopMusic();
                Debug.Log("No music");
            }
            else
            {
                Speaker.clip = CurrentPlaylist[CurrentSong].Audio;

                //todo:test
                if (resumingPrevSong)
                {
                    Speaker.timeSamples = PrevSongTimeSamples;
                }
                else
                {
                    Speaker.timeSamples = 0;
                    Speaker.time = 0;
                }

                var songLength = resumingPrevSong ? Speaker.clip.length - Speaker.time : Speaker.clip.length;
                var fadeInTime = 0f;


                if (Rm_RPGHandler.Instance.Audio.FadeInMusic && CurrentPlayListMode == PlaylistMode.Global || CurrentPlayListMode == PlaylistMode.Scene)
                {
                    fadeInTime = Rm_RPGHandler.Instance.Audio.FadeInTime;
                    Speaker.volume = 0;
                    Speaker.Play();
                    FadeMusicIn = true;
                    yield return new WaitForSeconds(fadeInTime);
                    FadeMusicIn = false;
                }
                else
                {
                    Speaker.Play();
                }

                if(Rm_RPGHandler.Instance.Audio.FadeOutMusic)
                {
                    var fadeOutTime = Rm_RPGHandler.Instance.Audio.FadeOutTime;
                    yield return new WaitForSeconds(songLength - fadeOutTime - fadeInTime);
                    FadeMusic = true;
                    yield return new WaitForSeconds(fadeOutTime);
                    FadeMusic = false;
                }
                else
                {
                    yield return new WaitForSeconds(songLength);
                }

                if(Rm_RPGHandler.Instance.Audio.ShufflePlaylist && CurrentPlaylist.Count > 1)
                {
                    var oldSongNo = CurrentSong;
                    while(CurrentSong == oldSongNo)
                    {
                        CurrentSong = Random.Range(0, CurrentPlaylist.Count);
                    }
                }
                else
                {
                    CurrentSong++;
                    if (CurrentSong > CurrentPlaylist.Count - 1)
                    {
                        CurrentSong = 0;
                    }
                }
                
                StartCoroutine("PlayMusic", false);
            }
            
        }

        public void StopSoundById(string soundId)
        {
            var destroyed = false;
            var audioObjects = GameObject.FindGameObjectsWithTag("RPGAudioObject");
            for (int i = 0; i < audioObjects.Length; i++)
            {
                var child = audioObjects[i];
                var comp = child.GetComponent<AudioBase>();
                if(comp != null && comp.SoundID == soundId)
                {
                    Destroy(comp.gameObject);
                    PlayOneAtATime.Remove(soundId);
                    destroyed = true;
                }
            }

            if(!destroyed)
            {
                //Debug.LogWarning("Could not find sound id to destroy! Already destroyed or possible error");
            }
        }

        public AudioBase GetSoundObjectByID(string soundId)
        {
            AudioBase foundSound = null;
            var audioObjects = GameObject.FindGameObjectsWithTag("RPGAudioObject");
            for (int i = 0; i < audioObjects.Length; i++)
            {
                var child = audioObjects[i];
                var comp = child.GetComponent<AudioBase>();
                if(comp != null && comp.SoundID == soundId)
                {
                    foundSound = comp;
                    break;
                }
            }

            return foundSound;
        }

        //Overridable by other script to allow play through other
        public virtual GameObject Play(AudioClip audioClip, AudioType audioType, Vector3 position, Transform parent = null, string soundId = "", float duration = -1)
        {
            if(audioClip != null)
            {
                if (PlayOneAtATime.Contains(soundId))
                {
                    return null;
                }

                if (!string.IsNullOrEmpty(soundId))
                {
                    PlayOneAtATime.Add(soundId);
                }

                var g = new GameObject(audioType.ToString());
                if(position == Vector3.zero)
                {
                   g.transform.parent = transform;
                   g.transform.localPosition = Vector3.zero; 
                }
                else
                {
                    g.transform.position = position;
                    if (parent != null)
                    {
                        g.transform.parent = parent;
                    }
                    else
                    {
                        g.transform.parent = transform;
                    }
                }

                var audiosource = g.AddComponent<AudioSource>();
                g.transform.tag = "RPGAudioObject";
                audiosource.clip = audioClip;
                audiosource.spatialBlend = 1;
                if(audioType == AudioType.Music)
                {
                    g.AddComponent<Audio_BgMusic>().SoundID = soundId;
                }
                else if(audioType == AudioType.Ambient)
                {
                    g.AddComponent<Audio_Ambient>().SoundID = soundId;
                }
                else if(audioType == AudioType.SoundFX)
                {
                    g.AddComponent<Audio_SFX>().SoundID = soundId;
                }
                else if(audioType == AudioType.Voice)
                {
                    g.AddComponent<Audio_Voice>().SoundID = soundId;
                }

                if(duration != -1 && duration > audioClip.length)
                {
                    audiosource.loop = true;
                    g.AddComponent<DestroyHelper>().Init(DestroyCondition.Time, duration);
                }
                    
                g.GetComponent<AudioSource>().Play();

                return g;
            }
            else
            {
                //Debug.Log("Audioclip is null.");
                return null;
            }

            
        }

        public GameObject PlayForever(AudioClip audioClip, AudioType audioType, Vector3 position, string soundId, Transform parent = null)
        {
            var audioObject =  Play(audioClip, audioType, position, parent, soundId);
            if(audioObject != null)
            {
                audioObject.GetComponent<AudioSource>().loop = true;    
                audioObject.GetComponent<AudioSource>().Play();    
            }
            return audioObject;
        }
    }

    public enum AudioType
    {
        Music,
        Ambient,
        SoundFX,
        Voice
    }

    public enum PlaylistMode
    {
        Global,
        Scene,
        Battle,
        Death
    }