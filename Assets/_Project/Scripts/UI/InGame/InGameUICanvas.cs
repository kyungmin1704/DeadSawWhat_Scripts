using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameUICanvas : MonoBehaviour
{
    public TextMeshProUGUI currentEquipmentName;
    public TextMeshProUGUI currentAndMaxAmmo;
    public TextMeshProUGUI currentPlayerHp;
    public TextMeshProUGUI currentEnemyCount;
    public TextMeshProUGUI currentStageName;
    public InGame_MenuButtons menuButtons;
    public SettingOptions settingOption;
    public PlayerHitEffectUI playerHitEffect;
    public GameObject playerInfoPanel;
    public PlayerStaminaUI playerStaminaUI;
    public GuidePanel guidePanel;
    public CurrentWaveUI currentWaveUI;
    public PartyHealthPanel partyHealthPanel;
}
