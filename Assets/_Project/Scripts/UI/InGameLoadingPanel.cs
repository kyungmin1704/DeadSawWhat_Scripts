using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameLoadingPanel : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    string[] patterns = { "..", "...", "....","..."};
    public float interval = 0.4f;

    int index;
    Tween tw;

    void OnEnable()
    {
        tw = DOTween.To(() => 0, x => { }, 1, interval).SetLoops(-1).OnKill(() =>
        {
            loadingText.text = "Loading" + patterns[index];
            index = (index + 1) % patterns.Length;
        });
    }
    void OnDisable()
    {
        tw.Kill();
    }
}
