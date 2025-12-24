using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : Singleton<LoadSceneManager>
{

    public GameObject stageLoadingPanel;
    public Image loadingImage;

    public Color32 originalColor = new Color32(255, 255, 255, 0);
    Color32 targetColor = new Color32(255, 255, 255, 255);
    Tween tw;
    static bool isLoading = false;
    
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        loadingImage.color = originalColor;
        stageLoadingPanel.SetActive(false);
        DOVirtual.DelayedCall(3f, () => LoadScene(Scenes.Menu));
    }

    /// <summary>
    /// 씬 전환 메서드 전환할 씬을 Scenes 열거형 파라미터로 전달 받습니다.
    /// </summary>
    /// <param name="index">전활할 Scene을 가르키는 Scenes 열거형 파라미터</param>
    public void LoadScene(Scenes index)
    {
        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient) return;
        if (isLoading) return;
        isLoading = true;
        switch ((Scenes)SceneManager.GetActiveScene().buildIndex)
        {
            case Scenes.LoadManagers:
                break;
            case Scenes.Menu:
                BgmManager.Instance.FadeOutBgm(BgmType.Menu, true);
                break;
            case Scenes.Stage01:
            case Scenes.Stage02:
                BgmManager.Instance.FadeOutBgm(BgmType.Game, true);
                break;
        }
        switch ((Scenes)index)
        {
            case Scenes.LoadManagers:
                StartCoroutine(LoadSceneCoroutine(index));
                break;
            case Scenes.Stage01:
            case Scenes.Stage02:
                loadingImage.sprite = GameManager.Instance.stageData.stageSprite;
                stageLoadingPanel.SetActive(true);
                tw = loadingImage.DOColor(targetColor, 1.5f);
                GameManager.Instance.players = new List<TestPlayerController>(4);
                StartCoroutine(LoadSceneCoroutine(index));
                break;
            case Scenes.Menu:
                if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    StartCoroutine(LoadSceneCoroutine(index));
                    break;
                }
                stageLoadingPanel.SetActive(true);
                tw = loadingImage.DOColor(targetColor, 1f);
                StartCoroutine(LoadSceneCoroutine(index));
                break;
        }

    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        isLoading = false;
        Time.timeScale = 1f;
        switch ((Scenes)scene.buildIndex)
        {
            case Scenes.LoadManagers:
                LoadScene(Scenes.Menu);
                break;
            case Scenes.Stage01:
            case Scenes.Stage02:
                BgmManager.Instance.FadeInBgm(BgmType.Game, true);
                BgmManager.Instance.FadeOutBgm(BgmType.Menu, true);
                loadingImage.color = originalColor;
                stageLoadingPanel.SetActive(false);
                GameManager.Instance.OnGameSceneLoaded();
                break;
            case Scenes.Menu:
                BgmManager.Instance.FadeInBgm(BgmType.Menu, true);
                BgmManager.Instance.FadeOutBgm(BgmType.Game, true);
                loadingImage.color = originalColor;
                stageLoadingPanel.SetActive(false);
                break;
        }
    }

    private IEnumerator LoadSceneCoroutine(Scenes index)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync((int)index);
        asyncOperation.allowSceneActivation = false;
        while (asyncOperation.progress < 0.9f)
        {
            yield return null;
        }
        DOTween.KillAll(true);
        asyncOperation.allowSceneActivation = true;
    }
}

public enum Scenes
{
    LoadManagers = 0,
    Menu = 1,
    Stage01 = 2,
    Stage02 = 3,
}
