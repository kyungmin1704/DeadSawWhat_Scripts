using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearCanvas : MonoBehaviour
{
    public List<GameClearPanel> gameClearPanels;
    public List<GameDefeatPanel> gameDefeatPanels;

    public void Init()
    {
        foreach (var panel in gameClearPanels)
        {
            panel.Init();
        }
        foreach (var panel in gameDefeatPanels)
        {
            panel.Init();
        }
    }

    public void PopupGameEnd(bool isMaster)
    {
        if (GameManager.Instance.player.CurrentHealth <= 0)
        {
            PopupDefeat(isMaster);
            if (PhotonNetwork.InRoom && isMaster)
            {
                NetworkManager.Pv.RPC("OnMasterDead", RpcTarget.Others);
            }
        }
        else
        {
            gameClearPanels[isMaster?0:1].GameEnd();
            gameClearPanels[isMaster?0:1].Highlight();
        }
    }

    public void PopupDefeat(bool isMaster)
    {
        gameDefeatPanels[isMaster?0:1].GameEnd();
        gameDefeatPanels[isMaster?0:1].Highlight();
    }
}