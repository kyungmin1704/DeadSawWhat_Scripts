using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGame_MenuButtons : MonoBehaviour
{
    public GameObject exitBox;
    public GameObject settingPanel;

    public Button[] buttons;

    void Start()
    {
        foreach (var button in buttons)
        {
            switch (button.gameObject.name)
            {
                case "ResumeButton":
                    button.onClick.AddListener(() =>
                    {
                        this.gameObject.SetActive(false);
                        Cursor.lockState = CursorLockMode.Locked;
                        GameManager.Instance.player.Resume();
                    });
                    break;
                case "MainButton":
                    button.onClick.AddListener(()=>LoadSceneManager.Instance.LoadScene(Scenes.Menu));
                    break;
                case "SettingButton":
                    button.onClick.AddListener(()=> settingPanel.SetActive(true));
                    break;
                case "ResetButton":
                    button.onClick.AddListener(()=>LoadSceneManager.Instance.LoadScene((Scenes)GameManager.Instance.stageData.sceneIndex));
                    break;
                case "ExitButton":
                    button.onClick.AddListener(() => exitBox.SetActive(true));
                    break;
            }
        }
    }
}
