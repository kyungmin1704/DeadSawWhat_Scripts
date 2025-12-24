using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentWaveUI : MonoBehaviour
{
    public Image defaultIcon;
    public Image normalWave;
    public Image BossWave;

    public List<Image> waves = new List<Image>();

    Color32 currentWaveColor = Color.white;

    public void Init()
    {
        foreach(var icon in waves)
        {
            icon.sprite = defaultIcon.sprite;
            icon.color = defaultIcon.color;
        }
    }

    public void WaveIconRefresh(int index)
    {
        for(int i = 0; i < waves.Count; i++)
        {
            if (i == index && SpawnManager.Instance.CurrentStageWavesData.waveList[index].waveType != WaveType.Boss)
            {
                waves[i].sprite = normalWave.sprite;
                waves[i].color = currentWaveColor;
            }else if (i == index && SpawnManager.Instance.CurrentStageWavesData.waveList[index].waveType == WaveType.Boss)
            {
                waves[index].sprite = BossWave.sprite;
                waves[index].color = currentWaveColor;
            }
            else
            {
                waves[i].color = defaultIcon.color;
            }
        }
    }

}
