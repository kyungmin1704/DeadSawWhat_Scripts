using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class OfflinePanel : MonoBehaviour
{
    public CloseButton connectButton;
    public TMP_InputField nickNameInput;

    public void Init()
    {
        connectButton.OnIsClicked += value => { if (value) NetworkManager.ConnectToPhoton(); };
        nickNameInput.onEndEdit.AddListener(value => NetworkManager.SetNickName(value));
        nickNameInput.text = NetworkManager.GetNickName();
    }
}
