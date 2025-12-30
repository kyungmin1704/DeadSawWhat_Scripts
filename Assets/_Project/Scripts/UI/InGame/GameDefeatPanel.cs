using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameDefeatPanel : MonoBehaviour
{

    Tween mainTween;
    Tween etcTween;

    public TextMeshProUGUI mainText;

    public GameObject etcPanel;

    public float fadeInDuration = 0.8f;
    public float fadeOutDuration = 0.5f;

    public Vector3 endvalue = Vector3.one;
    public Vector3 originValue = Vector3.zero;

    public void GameEnd()
    {
        gameObject.SetActive(true); // 게임 클리어 패널을 활성화
    }
    public virtual void Init()
    {
        gameObject.SetActive(false); // 초기에는 비활성화 상태로 시작
    }

    void OnDisable()
    {
        mainTween?.Kill(); // 트윈을 중지하고 제거
        etcTween?.Kill();
    }

    public void Highlight()
    {
        mainTween?.Kill(); // 이전 트윈을 중지하고 제거
        etcTween?.Kill();

        mainTween = DOTween.Sequence()
            .Append(mainText.transform.DOScale(endvalue, fadeInDuration).SetEase(Ease.OutQuart))
            .Append(mainText.transform.DOScale(originValue, fadeInDuration))
            .Append(mainText.transform.DOScale(endvalue, fadeInDuration).SetEase(Ease.OutQuart))
            .SetLoops(-1);
        etcTween = DOTween.Sequence()
            .Append(etcPanel.transform.DOScale(endvalue, fadeOutDuration).SetEase(Ease.OutExpo))
            .Append(etcPanel.transform.DOScale(originValue, fadeOutDuration))
            .Append(etcPanel.transform.DOScale(endvalue, fadeOutDuration).SetEase(Ease.OutExpo))
            .SetLoops(-1);
    }
}
