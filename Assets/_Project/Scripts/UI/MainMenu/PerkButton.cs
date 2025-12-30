using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PerkButton : BasicButtonVisual
{
    public PerkData data;
    PerkPanel panel;

    public Image icon;
    public TextMeshProUGUI pkName;

    void Start()
    {
        Init();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (isOn) { return; }
        Highlight();
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        Selected();
    }

    public override void Highlight()
    {
        UISoundManager.Instance.HoverSoundPlay();
        currentTween?.Kill();
        currentTween = mainImage.DOColor(targetColor, 0.2f);
    }
    public override void Selected()
    {
        UISoundManager.Instance.ClickSoundPlay();
        currentTween?.Kill();
        bgTween?.Kill();

        currentTween = mainImage.DOColor(originalColor, 0.2f);
        btnBackground.enabled = true;
        isOn = true;

        panel.SelectedButton(this);

    }
    public void Init(PerkPanel panel, PerkData data)
    {
        base.Init();
        targetColor = Color.white;
        this.panel = panel;

        this.data = data;
        pkName.text = data.perkName;
        this.icon.sprite = data.perkIcon;


    }

}
