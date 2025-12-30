using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHitEffectUI : MonoBehaviour
{

    public Image background;
    public Image leftHitEffect;
    public Image rightHitEffect;

    public Color32 bgOriginalColor = new Color(255, 0, 0, 0);
    public Color32 hitEffectOriginColor = new Color(255, 255, 255, 0);

    public float fadeInDuration;
    public float fadeOutDuration;

    public Tween rightHitTween;
    public Tween leftHitTween;

    public Ease ease;
    public Ease ease2;

    public void Init()
    {
        leftHitEffect.color = hitEffectOriginColor;
        rightHitEffect.color = hitEffectOriginColor;

        fadeInDuration = 0.2f;
        fadeOutDuration = 0.5f;

        ease = Ease.Linear;
        ease2 = Ease.OutBounce;
    }

    public void TakeDamage()
    {
        gameObject.SetActive(true);
        rightHitTween?.Complete();
        leftHitTween?.Complete();

        background.color = new Color32(255,0,0,10);

        rightHitTween = DOTween.Sequence()
            .Append(rightHitEffect.DOColor(Color.white, fadeInDuration).SetEase(ease))
            .Append(rightHitEffect.DOColor(hitEffectOriginColor, fadeOutDuration));

        leftHitTween = DOTween.Sequence()
           .Append(leftHitEffect.DOColor(Color.white, fadeInDuration).SetEase(ease2))
           .Append(leftHitEffect.DOColor(hitEffectOriginColor, fadeOutDuration));

        Tween seq = DOTween.Sequence()
            .Append(rightHitTween)
            .Join(leftHitTween)
            .Join(background.DOColor(bgOriginalColor,fadeOutDuration))
            .OnComplete(()=>gameObject.SetActive(false));

    }

}
