using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    public static AchievementUI Instance;
    public GameObject AchievementsPanel;
    public Text AchievementName;
    public Text AchievementDescription;
    public Image AchievementImage;

    private List<Achievement> _achievementQueue;
    private bool _showingAchievement;

	// Use this for initialization
	void Awake () {
	    AchievementsPanel.SetActive(false);
	    Instance = this;
        _achievementQueue = new List<Achievement>();
	}

    void Update()
    {
        if(_achievementQueue.Any())
        {
            if(!_showingAchievement)
            {
                StartCoroutine(ShowAndHideAchievement(_achievementQueue.First()));
            }
        }
    }

    public void ShowAchievement(Achievement achievement)
    {
        _achievementQueue.Add(achievement);
    }

    public IEnumerator ShowAndHideAchievement(Achievement achievement)
    {
        _showingAchievement = true;
        AchievementName.text = achievement.Name;
        AchievementDescription.text = achievement.Description;
        var image = achievement.ImageContainer.Image;

        if(achievement.ImageContainer.Image != null)
        {
            AchievementImage.gameObject.SetActive(true);
            AchievementImage.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.zero);    
        }
        else
        {
            AchievementImage.gameObject.SetActive(false);
        }
        
        
        AchievementsPanel.SetActive(true);
        var achievementSound = Rm_RPGHandler.Instance.Customise.AchievementUnlockedSound;
        if(achievementSound.Audio != null)
        {
            AudioPlayer.Instance.Play(achievementSound.Audio, AudioType.SoundFX, Vector3.zero);
        }

        yield return new WaitForSeconds(3.0f);
        AchievementsPanel.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        _achievementQueue.Remove(achievement);
        _showingAchievement = false;
    }
}
