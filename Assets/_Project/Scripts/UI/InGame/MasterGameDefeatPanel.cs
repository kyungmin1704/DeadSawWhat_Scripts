using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterGameDefeatPanel : GameDefeatPanel
{
    public Button[] buttons;
    public override void Init()
    {

        foreach (var button in buttons)
        {
            switch (button.name)
            {
                case "Main":
                    button.onClick.AddListener(() => LoadSceneManager.Instance.LoadScene(Scenes.Menu));
                    break;
            }
        }
        gameObject.SetActive(false); // 초기에는 비활성화 상태로 시작
    }
}
