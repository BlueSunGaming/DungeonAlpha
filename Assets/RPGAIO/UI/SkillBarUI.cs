using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillBarUI : MonoBehaviour
{
    public static SkillBarUI Instance;
    public bool Show;
    public GameObject SkillButtonContainer;
    public GameObject SkillBarButtonPrefab;
    public List<SkillBarButtonModel> SkillButtons;
    private float timePassed;

    private EventSystem EventSystem
    {
        get { return UIHandler.Instance.EventSystem; }
    }

    private SkillBarSlot GetSlot(int i)
    {
        return RPG.GetPlayerCharacter.SkillHandler.Slots[i];
    }

	// Use this for initialization
	void Awake () {
	    Instance = this;
	}

    public void Init()
    {
        if(GameMaster.isMobile)
        {
            SetupButtonsMobile();
        }
        else
        {
            SetupButtonsPC();
        }
        
    }

    void SetupButtonsMobile()
    {
        var skillSlots = RPG.GetPlayerCharacter.SkillHandler.Slots.Length;
        if (SkillButtonContainer != null && SkillButtonContainer.transform != null)
        {
            SkillButtons = new List<SkillBarButtonModel>();
            for (int i = 0; i < SkillButtonContainer.transform.childCount; i++)
            {
                if(i > skillSlots - 1)
                {
                    Destroy(SkillButtonContainer.transform.GetChild(i).gameObject);
                    continue;
                }

                SkillButtons.Add(SkillButtonContainer.transform.GetChild(i).GetComponent<SkillBarButtonModel>());
            }



            for (int i = 0; i < skillSlots; i++)
            {
                if (i > SkillButtonContainer.transform.childCount - 1) break; //Don't apply if skill slot was greater than available mobile slots

                var slot = GetSlot(i);
                var itemModel = SkillButtons[i];
                itemModel.SkillImage.sprite = slot.Image != null ? GeneralMethods.CreateSprite(slot.Image) : null;
                itemModel.SkillImage.color = itemModel.SkillImage.sprite == null ? Color.clear : Color.white;

                var labelNum = (i + 1);
                var label = labelNum.ToString();
                if (labelNum == 10)
                {
                    label = "0";
                }
                else if (labelNum == 11)
                {
                    label = "-";
                }
                else if (labelNum == 12)
                {
                    label = "+";
                }

                itemModel.SkillText.text = string.Format("{0}", label);
                itemModel.SkillSlot = i;
            }
        }
    }

    void SetupButtonsPC()
    {
        var skillSlots = RPG.GetPlayerCharacter.SkillHandler.Slots.Length;
        if (SkillButtonContainer != null && SkillButtonContainer.transform != null)
        {
            SkillButtonContainer.transform.DestroyChildren();
            SkillButtons = new List<SkillBarButtonModel>();
            for (int i = 0; i < skillSlots; i++)
            {
                var slot = GetSlot(i);
                var go = Instantiate(SkillBarButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go.transform.SetParent(SkillButtonContainer.transform, false);
                var itemModel = go.GetComponent<SkillBarButtonModel>();
                itemModel.SkillImage.sprite = slot.Image != null ? GeneralMethods.CreateSprite(slot.Image) : null;
                itemModel.SkillImage.color = itemModel.SkillImage.sprite == null ? Color.clear : Color.white;

                var labelNum = (i + 1);
                var label = labelNum.ToString();
                if (labelNum == 10)
                {
                    label = "0";
                }
                else if (labelNum == 11)
                {
                    label = "-";
                }
                else if (labelNum == 12)
                {
                    label = "+";
                }

                itemModel.SkillText.text = string.Format("{0}", label);
                itemModel.SkillSlot = i;

                SkillButtons.Add(itemModel);
            }
        }
    }

    void Update()
    {
        timePassed += Time.deltaTime;
        if(SkillButtons == null)
        {
            Init();
        }
        if(timePassed > 0.2f)
        {
            for (int i = 0; i < SkillButtons.Count; i++)
            {
                var slot = GetSlot(i);
                var itemModel = SkillButtons[i];
                //itemModel.SkillImage.sprite = slot.Image != null ? GeneralMethods.CreateSprite(slot.Image) : null;
                //itemModel.SkillImage.color = itemModel.SkillImage.sprite == null ? Color.clear : Color.white;

                var labelNum = (i + 1);
                var label = labelNum.ToString();
                if (labelNum == 10)
                {
                    label = "0";
                }
                else if (labelNum == 11)
                {
                    label = "-";
                }
                else if (labelNum == 12)
                {
                    label = "+";
                }

                itemModel.SkillText.text = string.Format("[{0}]", label);
                itemModel.SkillSlot = i;
            }
            timePassed = 0;
        }
        
    }
}
