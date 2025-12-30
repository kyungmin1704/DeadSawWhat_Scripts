using Photon.Pun;
using UnityEngine;

public class MultiplayPanel : MonoBehaviour
{
    public OfflinePanel offlinePanel;
    public OnlinePanel onlinePanel;
    public RoomPanel roomPanel;

    public void Init()
    {
        offlinePanel.Init();
        onlinePanel.Init();
        roomPanel.Init();

        if (PhotonNetwork.InRoom)
        {
            OnRoom();
            roomPanel.ChangeRoomState(PhotonNetwork.IsMasterClient, PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);
        }
        else if (PhotonNetwork.InLobby)
            OnOnline();
        else
            OnOffline();
    }

    public void OnOffline()
    {
        offlinePanel.gameObject.SetActive(true);
        onlinePanel.gameObject.SetActive(false);
        roomPanel.gameObject.SetActive(false);
    }

    public void OnOnline()
    {
        offlinePanel.gameObject.SetActive(false);
        onlinePanel.gameObject.SetActive(true);
        roomPanel.gameObject.SetActive(false);
    }

    public void OnRoom()
    {
        offlinePanel.gameObject.SetActive(false);
        onlinePanel.gameObject.SetActive(false);
        roomPanel.gameObject.SetActive(true);
    }
}
