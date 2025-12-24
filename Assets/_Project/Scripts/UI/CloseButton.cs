using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CloseButton : BasicButtonVisual
{
    public GameObject panel;

    public event Action<bool> OnIsClicked;
    public bool IsOn
    {
        get => isOn;
        set
        {
            if (isOn == value) return;
            isOn = value;
            OnIsClicked?.Invoke(isOn);
        }
    }
    void Start()
    {
        base.Init();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        Highlight();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        Selected(); //사운드 플레이 / 색상 애니메이션 / 백그라운드 이미지 활성화.
    }
    public override void Refresh()
    {
        base.Refresh();
    }

    public override void Highlight()
    {
        base.Highlight();//사운드 플레이 / 크기 애니메이션
    }
    public override void Selected()
    {
        UISoundManager.Instance.ClickSoundPlay();
        btnBackground.enabled = true;
        currentTween = DOTween.Sequence().Append(transform.DOScale(1.05f, 0.2f))
            .Join(mainImage.DOColor(targetColor, 0.2f))
            .Append(transform.DOScale(originalScale, 0.2f))
            .OnComplete(() => {
                IsOn = !IsOn;
                Refresh();
                }); ;
    }
}
