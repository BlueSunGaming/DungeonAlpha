using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleUI : MonoBehaviour
{
    public static SubtitleUI Instance;
    public GameObject SubtitleTextObject;
    public Text SubtitleText;
    public HorizontalLayoutGroup hLayout;

    private float timeShown;
    private float showDuration;

    public List<SubtitleTemplate> Subtitles;

    public bool IsShowingSubtitles
    {
        get { return Subtitles.Count > 0 || timeShown < showDuration; }
    }

    // Update is called once per frame
    void Awake()
    {
        Instance = this;
        timeShown = 0;
        showDuration = 0;
        Subtitles = new List<SubtitleTemplate>();
        hLayout = SubtitleTextObject.GetComponent<HorizontalLayoutGroup>();
    }

    void Update()
    {
        if(Subtitles.Count > 0 && timeShown >= showDuration)
        {
            ShowSubtitle(Subtitles[0].Text, Subtitles[0].Duration, Subtitles[0].AudioPath);
            Subtitles.RemoveAt(0);
        }

        timeShown += Time.deltaTime;
        if(timeShown > showDuration && Subtitles.Count == 0)
        {
            SubtitleTextObject.SetActive(false);
            SubtitleText.text = "";
        }
    }

    public void ShowSubtitles(params SubtitleTemplate[] subtitles)
    {
        if(Subtitles.Count == 0)
        {
            Subtitles = subtitles.ToList();
        }
        else
        {
            Subtitles.AddRange(subtitles);
        }
    }

    public void ShowSubtitle(string text, float duration = 5.0f, string audioPath = "")
    {
        if (Subtitles.Count > 0)
        {
            Debug.Log("[RPGAIO] Skipped showing a single subtitle line as a longer list is already showing.");
        }

        timeShown = 0;
        showDuration = duration + Rm_RPGHandler.Instance.Customise.SubtitleDelayBetweenLines;
        
        SubtitleText.text = text;
        SubtitleTextObject.SetActive(true);


        if(!string.IsNullOrEmpty(audioPath))
        {
            var soundclip = (AudioClip)Resources.Load(audioPath);

            if (soundclip != null)
                AudioPlayer.Instance.Play(soundclip, AudioType.SoundFX, Vector3.zero);
        }
        
        hLayout.GetComponent<ContentSizeFitter>().enabled = false;
        Canvas.ForceUpdateCanvases();
        hLayout.SetLayoutHorizontal();
        hLayout.GetComponent<ContentSizeFitter>().enabled = true;
    }
}