using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingOptions : MonoBehaviour
{
    public List<OptionButtonData>optionDatas = new List<OptionButtonData>();
    public List<OptionButtonVisual> options = new List<OptionButtonVisual>();
    public OptionButtonVisual currentButton;

    public TextMeshProUGUI currentOptionName;
    public TextMeshProUGUI currentOptionDescirption;

    bool firstStart = false;
    public void Init()
    {

       // optionDatas = GameManager.Instance.optionDatas;

        for(int i = 0; i < options.Count; i++)
        {
            options[i].SetOption(this , optionDatas[i]);
        }

        currentOptionName.enabled = false;
        currentOptionDescirption.enabled = false;

        firstStart = true;
    }
 
    void OnEnable()
    {
        if (!firstStart) return;

        foreach (var option in options)
        {
            option.Refresh();
        }

        currentOptionName.enabled = false;
        currentOptionDescirption.enabled = false;
    }
    public void SelectedButton(OptionButtonVisual btn)
    {
        currentOptionName.enabled = true;
        currentOptionDescirption.enabled = true;
        foreach (var option in options)
        {
            if (!options.Contains(btn)) { print($"{option.name} / {btn.name}");continue; }

            if (option == btn)
            {
                currentButton = btn;
                currentOptionName.text = currentButton.data.optionName;
                currentOptionDescirption.text = currentButton.data.description;
            }
            else
            {
                option.Refresh();
            }
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.ApplyOption();
    }
}
