using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthPanel : MonoBehaviour
{
    public TextMeshProUGUI nickNameText;
    public Slider healthSlider;

    public int ActorID { get; private set; }

    public PlayerHealthPanel Init(string nickName, int actorID)
    {
        ActorID = actorID;
        nickNameText.text = nickName;
        healthSlider.maxValue = 100f;
        healthSlider.value = 100f;
        return this;
    }

    public void RefreshPanel(float current, float max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = current;
    }
}
