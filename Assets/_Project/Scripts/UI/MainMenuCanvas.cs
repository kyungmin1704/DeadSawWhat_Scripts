using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCanvas : MonoBehaviour
{
    public TopViewPanel topViewPanel;
    public HomeMenu homeMenu;
    public SettingOptions settingOptions;
    public PerkPanel perkPanel;
    public MultiplayPanel multiplayPanel;
    public GameObject waitingPanel;

    public MainMenuCanvas Init()
    {
        topViewPanel.Init();
        homeMenu.Init();
        settingOptions.Init();
        perkPanel.Init();
        multiplayPanel.Init();
        return this;
    }
}
