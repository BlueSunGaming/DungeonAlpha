using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MetaDataSelectModel : MonoBehaviour
{
    public Rm_MetaDataDefinition MetaDataDefinition;
    public Image Image;
    public Text TitleText;
    public Text Description;
    public Text SelectedOptionLabel;
    public int _selectedOption;

    public void Init(Rm_MetaDataDefinition definition)
    {
        _selectedOption = 0;
        MetaDataDefinition = definition;
        TitleText.text = definition.Name;
        Image.sprite = GeneralMethods.CreateSprite(definition.Values[_selectedOption].Image.Image);
        Description.text = definition.Values[_selectedOption].Description;
        SelectedOptionLabel.text = definition.Values[_selectedOption].Name;
    }

    public void NextOption()
    {
        _selectedOption++;
        if (_selectedOption + 1 > MetaDataDefinition.Values.Count)
        {
            _selectedOption = 0;
        }

        Image.sprite = GeneralMethods.CreateSprite(MetaDataDefinition.Values[_selectedOption].Image.Image);
        Description.text = MetaDataDefinition.Values[_selectedOption].Description;
        SelectedOptionLabel.text = MetaDataDefinition.Values[_selectedOption].Name;
        CharacterCreationMono.Instance.SetMetaData(MetaDataDefinition, _selectedOption);
    }

    public void PrevOption()
    {
        _selectedOption--;
        if (_selectedOption < 0)
        {
            _selectedOption = MetaDataDefinition.Values.Count - 1;
        }

        Image.sprite = GeneralMethods.CreateSprite(MetaDataDefinition.Values[_selectedOption].Image.Image);
        Description.text = MetaDataDefinition.Values[_selectedOption].Description;
        SelectedOptionLabel.text = MetaDataDefinition.Values[_selectedOption].Name;
        CharacterCreationMono.Instance.SetMetaData(MetaDataDefinition, _selectedOption);
    }
}