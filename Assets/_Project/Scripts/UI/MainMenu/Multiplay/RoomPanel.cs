using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RoomPanel : MonoBehaviour
{
    public CloseButton leaveButton;
    public TextMeshProUGUI roomStateText;
    public TextMeshProUGUI roomCodeText;
    public TextMeshProUGUI[] playerListText;

    StringBuilder sb;

    public void Init()
    {
        leaveButton.OnIsClicked += value => { if (value) NetworkManager.LeftRoom(); };

        roomCodeText.text = NetworkManager.GetRoomCode();

        sb = new StringBuilder();
    }

    public void ChangeRoomState(bool isMaster, int current, int max)
    {
        sb.Clear();

        sb.Append(isMaster?"Master":"Member");
        sb.Append(" : ");
        sb.Append(current);
        sb.Append("/");
        sb.Append(max);

        roomStateText.text = sb.ToString();

        roomCodeText.text = NetworkManager.GetRoomCode();

        foreach (var player in playerListText)
        {
            player.text = "";
        }

        IEnumerator<Photon.Realtime.Player> playDict = PhotonNetwork.CurrentRoom.Players.Values.GetEnumerator();
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            playDict.MoveNext();
            playerListText[i].text = playDict.Current.NickName;
        }
    }
}
