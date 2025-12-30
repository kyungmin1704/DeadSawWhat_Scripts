using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusPanel : MonoBehaviour
{
    public Sprite noneIcon;

    public Image perkIcon_1;
    public Image perkIcon_2;

    public void Init()
    {
        if (GameManager.Instance == null) return;
        perkIcon_1.sprite = GameManager.Instance.ownPerk.Count > 0 ? GameManager.Instance.ownPerk[0].perkIcon : noneIcon;
        perkIcon_2.sprite = GameManager.Instance.ownPerk.Count > 1 ? GameManager.Instance.ownPerk[1].perkIcon : noneIcon;
    }

}
