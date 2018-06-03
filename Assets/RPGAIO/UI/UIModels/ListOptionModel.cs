using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ListOptionModel : MonoBehaviour
{
    public Text SelectedOptionText;
    public int SelectedOption;
    public List<string> Options;

    public void Set(int selected, List<string> options)
    {
        SelectedOption = selected;
        Options = options;
        SelectedOptionText.text = Options[selected];
    }

    public void NextOption()
    {
        SelectedOption++;
        if(SelectedOption > Options.Count - 1)
        {
            SelectedOption = 0;
        }

        SelectedOptionText.text = Options[SelectedOption];
    }

    public void PrevOption()
    {
        SelectedOption--;
        if (SelectedOption < 0)
        {
            SelectedOption = Options.Count - 1;
        }

        SelectedOptionText.text = Options[SelectedOption];
    }
}