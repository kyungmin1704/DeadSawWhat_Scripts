using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq;

public class GameClearPanel : MonoBehaviour
{

    Tween mainTween;
    Tween etcTween;

    public TextMeshProUGUI mainText;
    public GameObject etcPanel;
    public TextAnimation[] etcTexts;

    public Image waveIcon;
    public Image enemyIcon;


    public float fadeInDuration = 0.3f;
    public float fadeOutDuration = 0.3f;

    public Vector3 endvalue = Vector3.one;
    public Vector3 originValue = Vector3.zero;

    public int killCount;
    public int waveCount;

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

    public virtual void Highlight()
    {
        mainTween?.Kill(); // 이전 트윈을 중지하고 제거
        etcTween?.Kill();

        StartCoroutine(ShowText()); //클리어 한 웨이브 / 처치 한 에너미 킬 카운트 텍스트의 애니메이션


        mainTween = DOTween.Sequence()
            .Append(mainText.transform.DOScale(endvalue, fadeInDuration).SetEase(Ease.OutBack))
            .Append(mainText.transform.DOScale(originValue, fadeInDuration))
            .Append(mainText.transform.DOScale(endvalue, fadeInDuration).SetEase(Ease.OutBack))
            .SetLoops(-1);
        etcTween = DOTween.Sequence()
            .Append(etcPanel.transform.DOScale(endvalue, fadeOutDuration).SetEase(Ease.OutBack))
            .Append(etcPanel.transform.DOScale(originValue, fadeOutDuration))
            .Append(etcPanel.transform.DOScale(endvalue, fadeOutDuration).SetEase(Ease.OutBack))
            .SetLoops(-1);

    }
    protected  IEnumerator ShowText()
    {
        foreach (var text in etcTexts)
        {
            switch (text.name)
            {
                case "WaveClearText":
                    text.Play(InGameUIManager.Instance.WaveCount);
                    break;
                    case "EnemyKillCountText":
                    text.Play(InGameUIManager.Instance.KillCount);
                    break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

}
