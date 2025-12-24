using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkPanel : MonoBehaviour
{

    [Serializable]
    public class PerkButtonRow
    {
        public List<PerkButton> buttons = new List<PerkButton>();
        public PerkButton currentButton;
    }

    public List<PerkButtonRow> buttonRow = new List<PerkButtonRow>();

    public PerkData[] perkDatas;
    public Image currentPerkIcon;
    public TextMeshProUGUI currentPerkName;
    public TextMeshProUGUI currentPerkDescription;

    public GameObject perkInfoPanel;

    void Start()
    {
        Init();
    }
    public void Init()
    {
        int count = 0;
        foreach (PerkButtonRow btnRow in buttonRow)
        {
            foreach (PerkButton button in btnRow.buttons)
            {
                if(count > perkDatas.Length) { return; }
                button.Init(this, perkDatas[count]);
                count++;
            }
        }
        perkInfoPanel.SetActive(false);
    }

    public void SelectedButton(PerkButton perkButton)
    {
        foreach (PerkButtonRow btnRow in buttonRow)
        {
            if(!btnRow.buttons.Contains(perkButton))
            { continue; }

            foreach (PerkButton button in btnRow.buttons)
            {
                if (perkButton == button)
                {
                    btnRow.currentButton = button;
                    currentPerkName.text = button.data.perkName;
                    currentPerkIcon.sprite = button.data.perkIcon;
                    currentPerkDescription.text = button.data.perkDescription;

                    if (!GameManager.Instance.ownPerk.Contains(button.data))
                    {
                        GameManager.Instance.ownPerk.Add(button.data);
                    }

                }
                else
                {
                    button.Refresh();
                    if (GameManager.Instance.ownPerk.Contains(button.data))
                    {
                        GameManager.Instance.ownPerk.Remove(button.data);
                    }
                }
            }
        }
        perkInfoPanel.SetActive(true);
    }
}
