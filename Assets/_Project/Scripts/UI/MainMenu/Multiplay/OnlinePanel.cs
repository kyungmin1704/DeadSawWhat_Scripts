using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnlinePanel : MonoBehaviour
{
    public CloseButton createButton;
    public CloseButton joinButton;
    public TMP_InputField roomCodeInput;

    public void Init()
    {
        createButton.OnIsClicked += value => { if (value) NetworkManager.CreateRoom(); };
        joinButton.OnIsClicked += value => { if (value) NetworkManager.JoinRoom(); };
        roomCodeInput.onEndEdit.AddListener(value => NetworkManager.SetRoomCode(value));
        roomCodeInput.text = NetworkManager.GetRoomCode();
    }
}
