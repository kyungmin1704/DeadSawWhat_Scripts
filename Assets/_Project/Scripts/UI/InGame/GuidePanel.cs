using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidePanel : MonoBehaviour
{
    public RectTransform targetPanel;

    public bool isOn;
    Tween tween;
    public void Init()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            isOn = true;
        }
    }
    void OnEnable()
    {
        isOn = false;
    }
    public void ShowAndHidePanel()
    {
        tween.Kill();
        if (isOn)
        {
            tween = targetPanel.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutExpo);
        }
        else
        {
            tween = targetPanel.DOAnchorPosX(700f, 0.5f).SetEase(Ease.InExpo);
        }
        isOn = !isOn;

    }

}
