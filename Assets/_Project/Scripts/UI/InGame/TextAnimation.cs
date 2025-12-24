using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextAnimation : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image icon;
    string fullText;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        fullText = text.text; // 기존 텍스트를 변수에 저장
        text.text = "";        // 애니메이션을 위해 초기화
    }

    public void Play(int count)
    {
        string replacedText = fullText.Replace("00", $"{count}");
        StartCoroutine(ShowText(replacedText));
    }

    private IEnumerator ShowText(string dpText)
    {
        icon.enabled = true;
        text.text = "";        // 애니메이션을 위해 초기화
        foreach (char c in dpText)
        {
            text.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
