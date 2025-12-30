using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeMenu : MonoBehaviour
{
    public List<StageBar> stages; //모든 스테이지바 (전체의 스테이지)
    StageBar currentStage; //현재 선택 된 스테이지


    public StageBar stagePrefab; //스테이지바의 프리팹
    public Transform stagePanel; //부모로 받을 패널

    public StageData[] datas;

    public StagePanel panel;

    public CloseButton Btn;

    void OnEnable()
    {
        Btn.OnIsClicked += TestClick;
    }
    void OnDisable()
    {
        Btn.OnIsClicked -= TestClick;
    }

    void TestClick(bool ison)
    {
        if (ison)
        {
            StartButtonOnClick();
        }
    }

    public void Init()
    {
        foreach (Transform child in stagePanel)
        {
            Destroy(child.gameObject);
        }
        stages.Clear();

        foreach (StageData data in datas)
        {
            StageBar newStage = Instantiate(stagePrefab, stagePanel);
            newStage.Init(data, this);
            stages.Add(newStage);
        }

        for (int i = 0; i < stages.Count; i++)
        {
            if(i == 0)
                stages[0].stageOpen = true;
            else
            {
                stages[i].stageOpen = true;
            }
        }
        stages[0].Selected();
    }


    public void StageSelected(StageBar stage)
    {
        foreach (var stageBar in stages)
        {
            if (stageBar == stage)
            {
                stage.isOn = true;
                currentStage = stage;
                panel.gameObject.SetActive(true);
                panel.StagePanelRefresh(currentStage.data);
            }
            else
            {
                stageBar.isOn = false;
                stageBar.Refresh();
            }
        }
    }
    public void StartButtonOnClick()
    {
        GameManager.Instance.stageData = currentStage.data;
        LoadSceneManager.Instance.LoadScene((Scenes)currentStage.data.sceneIndex);
    }
}
