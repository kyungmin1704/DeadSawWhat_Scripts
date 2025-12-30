using UnityEngine;

public class MenuUIManager : Singleton<MenuUIManager>
{
    MainMenuCanvas mainMenuCanvas;

    public void Init()
    {
        mainMenuCanvas = GameObject.Find("MainMenuCanvas").GetComponent<MainMenuCanvas>().Init();
    }
    void Start()
    {
        Init();
    }

    public void ChangeNetworkState(NetworkState state)
    {
        switch (state)
        {
            case NetworkState.Offline:
                mainMenuCanvas.multiplayPanel.OnOffline();
                break;
            case NetworkState.Online:
                mainMenuCanvas.multiplayPanel.OnOnline();
                break;
            case NetworkState.Room:
                mainMenuCanvas.multiplayPanel.OnRoom();
                break;
        }
    }

    public void ChangeRoomState(bool isMaster, int current, int max)
    {
        mainMenuCanvas.multiplayPanel.roomPanel.ChangeRoomState(isMaster, current, max);
    }

    public void WaitingPanelPopup(bool isPopup)
    {
        mainMenuCanvas.waitingPanel.SetActive(isPopup);
    }

}
public enum NetworkState
{
    Offline,
    Online,
    Room,
}