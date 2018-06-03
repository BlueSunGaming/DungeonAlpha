using UnityEngine;
using UnityEngine.UI;

public class ToggleOptionModel : MonoBehaviour
{
    public bool SelectedOption;
    public Toggle Toggle;

    public void ToggleOption(bool option)
    {
        SelectedOption = option;
    }

    public void SetToggle(bool option)
    {
        Toggle.isOn = option;
    }
}