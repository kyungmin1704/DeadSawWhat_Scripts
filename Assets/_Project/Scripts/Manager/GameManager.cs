using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : Singleton<GameManager>
{
    public GameObject playerPrefab;
    [HideInInspector] public TestPlayerController player;
    [HideInInspector] public List<TestPlayerController> players;
    public GunData defaultGun;
    public List<AmmoData> ammoList;

    public StageData stageData;
    public List<PerkData> ownPerk = new List<PerkData>();

    public List<OptionButtonData> optionDatas = new List<OptionButtonData>();
    
    public AudioMixer audioMixer;

    public float Sensitivity
    {
        set
        {
            optionDatas[0].ftValue = value;
            if (player != null)
                player.lookSensitivity = value;
        }
    }

    private float moveSpeedMod;
    private float damageMod;
    private float reloadSpeedMod;

    public bool isClear;

    public void ApplyOption()
    {
        if (player != null)
        {
            player.lookSensitivity = optionDatas[0].ftValue;
            player.verticalSensitivityRatio = optionDatas[1].ftValue;
        }
        if (optionDatas[2].ftValue > 0.0001f) audioMixer.SetFloat("Master", Mathf.Log10(optionDatas[2].ftValue / 100) * 20);
        else audioMixer.SetFloat("Master", -80f);
            
    }

    public void OnGameSceneLoaded()
    {
        foreach (AmmoData ammo in ammoList)
        {
            ammo.AmmoAmount = ammo.maxAmmoStack;
        }
        
        // 게임 씬 로드시에 실행될 메서드 입니다. 매니저를 추가했다면 해당 스크립트에서 Init()메서드를 실행해 주세요
        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.InRoom)
            SpawnManager.Instance.Init(stageData.waves);
        InGameUIManager.Instance.Init();
        
        moveSpeedMod = PerkData.GetMoveSpeedMultiplier(ownPerk);
        damageMod = PerkData.GetDamageMultiplier(ownPerk);
        reloadSpeedMod = PerkData.GetReloadSpeedMultiplier(ownPerk);
        if (PhotonNetwork.InRoom)
            player = PhotonNetwork.Instantiate("TestPlayer",Vector3.zero, Quaternion.identity).GetComponent<TestPlayerController>();
        else
            player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<TestPlayerController>();
        player.Init(ammoList);
        player.inventory.AddGunDatas(defaultGun);
        player.SetPerkMod(moveSpeedMod, damageMod, reloadSpeedMod);
        if (!players.Contains(player)) players.Add(player);
        ApplyOption();
        player.cameraAnimator.gameObject.SetActive(true);

        InGameUIManager.Instance.PlayerStatus();
    }

    public Vector3 GetNearestPlayerPosition(Vector3 position)
    {
        if (PhotonNetwork.InRoom)
        {
            List<TestPlayerController> temp = new (players);
            temp = temp.OrderBy(obj => Vector3.Distance(obj.transform.position, position)).ToList();
            return temp.Count < 1 ? Vector3.zero : temp.FirstOrDefault()!.transform.position;
        }

        return players.Count < 1 ? Vector3.zero : players[0].transform.position;
    }
}
