using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameUIManager : Singleton<InGameUIManager>
{
    public GameObject InGameUICanvasPrefab;
    public GameObject GameClearCanvasPrefab;
    InGameUICanvas obj;
    GameClearCanvas clearCanvas;
    public TextMeshProUGUI CurrentEquipmentName { get; private set; }
    public TextMeshProUGUI CurrentAndMaxAmmo { get; private set; }
    public TextMeshProUGUI CurrentPlayerHp { get; private set; }
    public TextMeshProUGUI CurrentEnemyCount { get; private set; }
    public TextMeshProUGUI CurrentStageName { get; private set; }

    private int killCount = 0;
    public int KillCount
    {
        get => killCount;
        set
        {
            killCount = value;
        }
    }
    private int waveCount = 0;
    public int WaveCount
    {
        get => waveCount;
        set
        {
            waveCount = value;
        }
    }
    //#region 테스트용 무시
    //private void Start()
    //{
    //    Init();
    //}
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Y))
    //    {
    //        GameClear();
    //    }
    //}
    //#endregion
    public void Init()
    {
        obj = Instantiate(InGameUICanvasPrefab, transform).GetComponent<InGameUICanvas>();
        clearCanvas = Instantiate(GameClearCanvasPrefab, transform).GetComponent<GameClearCanvas>();
        clearCanvas.Init();
        clearCanvas.gameObject.SetActive(false);
        CurrentEquipmentName = obj.currentEquipmentName;
        CurrentAndMaxAmmo = obj.currentAndMaxAmmo;
        CurrentPlayerHp = obj.currentPlayerHp;
        CurrentEnemyCount = obj.currentEnemyCount;
        CurrentEnemyCount.text = "0";
        CurrentStageName = obj.currentStageName;

        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.InRoom)
        {
            CurrentStageName.text = GameManager.Instance.stageData.stageName;
            obj.settingOption.optionDatas = GameManager.Instance.optionDatas;
        }

        obj.menuButtons.gameObject.SetActive(false);
        obj.settingOption.gameObject.SetActive(false);
        obj.playerHitEffect.gameObject.SetActive(false);

        obj.settingOption.Init();
        obj.playerHitEffect.Init();

        //InGameUICanvas 패널들의 Text, Icon 따로 패널끼리 분리해둘 것.
        obj.playerInfoPanel.GetComponent<PlayerStatusPanel>().Init();

        obj.currentWaveUI.Init();
        obj.guidePanel.Init();
        if (PhotonNetwork.InRoom)
            obj.partyHealthPanel.Init(PhotonNetwork.CurrentRoom.Players.Values.ToArray());
        else
            obj.partyHealthPanel.Init(null);
    }


    public void GameClear()
    //메서드 내에서 게임 클리어, 게임 오버 패널 구분 필요
    {
        GameManager.Instance.player.PlayerInput.SwitchCurrentActionMap("UI");
        obj.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        
        clearCanvas.gameObject.SetActive(true);
        clearCanvas.PopupGameEnd(!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient);
    }

    public void GameOver()
    {
        GameManager.Instance.player.PlayerInput.SwitchCurrentActionMap("UI");
        obj.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        
        clearCanvas.gameObject.SetActive(true);
        clearCanvas.PopupDefeat(!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient);
    }

    public void RefreshPlayerHealth(float current, float max, int actorID)
    {
        obj.partyHealthPanel.playerHealthPanels?.Find(obj => obj.ActorID == actorID)?.RefreshPanel(current, max);
    }

    public void ShowInGameMenu(InputAction.CallbackContext ob)
    {
        Escape(ob);
        this.obj.menuButtons.gameObject.SetActive(true);
    }
    public void Escape(InputAction.CallbackContext obj)
    {
        Cursor.lockState = CursorLockMode.None;

    }
    #region 웨이브에 따른 웨이브 아이콘 수정 메서드
    public void RefreshWaveIcon(int index)
    {
        obj.currentWaveUI.WaveIconRefresh(index);
    }
    #endregion
    #region 현재 생성 되어있는 에너미의 수 텍스트
    public void RefreshCurrentEnemyCount(int count)
    {
        CurrentEnemyCount.text = count.ToString();
    }
    #endregion

    public void ToggleKeybinding(InputAction.CallbackContext obj)
    {
        this.obj.guidePanel.ShowAndHidePanel();
    }

    #region 플레이어의 스테미나 초기 설정과 스크롤바 변경 호출 메서드
    public void PlayerStatus()
    {
        obj.playerStaminaUI.Init();
    }

    public void PlayerStaminaRefresh()
    {
        obj.playerStaminaUI.RefreshStaminaBar();
    }
    #endregion
    #region 플레이어가 피격 시 나오는 이펙트UI 호출 메서드
    public void HitFX()
    {
        obj.playerHitEffect.TakeDamage();
    }
    #endregion


    #region 플레이어의 현재 총알 / 소지하고 있는 전체 총알의 수 텍스트 UI
    public void AmmoRefresh(int curAmmo, int MaxAmmo)
    {
        CurrentAndMaxAmmo.text = $"{curAmmo} / {MaxAmmo}";
    }
    #endregion
    #region 플레이어의 현재 착용중인 무기 이름
    public void SetEquipmentName(string value)
    {
        CurrentEquipmentName.text = value;
    }
    #endregion
}
