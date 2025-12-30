using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PartyHealthPanel : MonoBehaviour
{
    public PlayerHealthPanel healthPanelPrefab;
    [HideInInspector] public List<PlayerHealthPanel> playerHealthPanels;

    private Transform pivot;

    public void Init(Photon.Realtime.Player[] players)
    {
        playerHealthPanels = new List<PlayerHealthPanel>();
        if (!PhotonNetwork.InRoom)
        {
            gameObject.SetActive(false);
            return;
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            gameObject.SetActive(false);
            return;
        }

        pivot = transform.GetChild(0);
        foreach (var player in players)
        {
            playerHealthPanels.Add(Instantiate(healthPanelPrefab, pivot).Init(player.NickName, player.ActorNumber));
        }
    }
}
