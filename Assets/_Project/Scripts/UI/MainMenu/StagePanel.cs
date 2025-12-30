using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StagePanel : MonoBehaviour
{
    public TextMeshProUGUI currentStageInfo;
    public Image currentStageImage;
    public void StagePanelRefresh(StageData data)
    {
        currentStageImage.sprite = data.stageSprite;
        currentStageInfo.text = data.stageName;
    }
}
