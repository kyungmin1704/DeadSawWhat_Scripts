using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoom : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (!PhotonNetwork.IsConnected) NetworkManager.ConnectToPhoton();
            else NetworkManager.JoinRoom();
        });
    }
}
