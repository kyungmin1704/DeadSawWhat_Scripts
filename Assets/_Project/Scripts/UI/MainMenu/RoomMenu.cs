using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMenu : MonoBehaviour
{
   public GameObject playerPrefab;
    PlayerInfo_Room playerInfoBar;

    public void Init()
    {
        playerInfoBar = Instantiate(playerPrefab, transform.Find("Viewport").Find("PlayerInfoArea")).GetComponent<PlayerInfo_Room>();
    }


    //플레이어가 룸 입장시 호출 할 메서드 생성 할 것.
    //메서드 안에 Init()과 playerInfoBar.Init() 넣어서 동기화 시키기.

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.P))
    //    {
    //        Init();
    //    }
    //}
}
