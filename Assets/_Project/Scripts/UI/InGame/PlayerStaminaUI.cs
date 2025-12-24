using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaUI : MonoBehaviour
{
    public Slider[] playerStaminaBar;

    public void Init()
    {
        foreach (var value in playerStaminaBar)
        {
            value.maxValue = GameManager.Instance.player.MaxStamina;
        }
        playerStaminaBar[0].value = GameManager.Instance.player.CurrentStamina;
        playerStaminaBar[1].value = 3f;
        playerStaminaBar[2].value = 10f;
    }
    public void RefreshStaminaBar()
    {
        foreach (var value in playerStaminaBar)
        {
            value.maxValue = GameManager.Instance.player.MaxStamina;
        }
        playerStaminaBar[0].value = GameManager.Instance.player.CurrentStamina;
        playerStaminaBar[1].value = 3f;
        playerStaminaBar[2].value = 10f;
    }
}
