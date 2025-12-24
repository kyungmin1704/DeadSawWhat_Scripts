using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;

public class NetworkManager : PunSingleton<NetworkManager>
{
    private static string nickName;
    private static string roomCode;

    public static PhotonView Pv { get; private set; }

    private void OnDestroy()
    {
        if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
        if (PhotonNetwork.InLobby) PhotonNetwork.LeaveLobby();
        if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = Random.Range(0, 1000).ToString();
        roomCode = "room code";
        SetNickName("default");
        
        Pv = GetComponent<PhotonView>();
    }

    public void SceneChange(Scenes scene)
    {
        PhotonNetwork.LoadLevel((int)scene);
    }

    public static void SetRoomCode(string roomCode) => NetworkManager.roomCode = roomCode;
    public static string GetRoomCode() => roomCode;
    public static void SetNickName(string nickName)
    {
        NetworkManager.nickName = nickName;
        PhotonNetwork.LocalPlayer.NickName = nickName;
    }
    public static string GetNickName() => nickName;

    public static void ConnectToPhoton()
    {
        if (PhotonNetwork.IsConnected) return;
        MenuUIManager.Instance.WaitingPanelPopup(true);
        PhotonNetwork.ConnectUsingSettings();
    }

    public static void CreateRoom()
    {
        MenuUIManager.Instance.WaitingPanelPopup(true);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;
        PhotonNetwork.CreateRoom(roomCode, roomOptions);
    }

    public static void JoinRoom()
    {
        MenuUIManager.Instance.WaitingPanelPopup(true);
        PhotonNetwork.JoinRoom(roomCode);
    }

    public static void LeftRoom()
    {
        MenuUIManager.Instance.WaitingPanelPopup(true);
        PhotonNetwork.LeaveRoom();
    }

    public void SceneLoaded(Scenes scene)
    {
        print("run loadlevel method");
        if (PhotonNetwork.IsMasterClient)
            SceneChange(scene);
    }

    public override void OnConnected()
    {
        base.OnConnected();
        print("connected");
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        print("connected to master");
        print("request joining lobby");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print("joined lobby");
        MenuUIManager.Instance.ChangeNetworkState(NetworkState.Online);
        MenuUIManager.Instance.WaitingPanelPopup(false);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print("created room");
        MenuUIManager.Instance.WaitingPanelPopup(false);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        print($"failed to create room, error code: {returnCode}, message: {message}");
        MenuUIManager.Instance.WaitingPanelPopup(false);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print($"joined room, Player count: {PhotonNetwork.CurrentRoom.PlayerCount}, IsMaster {PhotonNetwork.IsMasterClient}");
        print(PhotonNetwork.CurrentRoom.Name);
        MenuUIManager.Instance.ChangeNetworkState(NetworkState.Room);
        MenuUIManager.Instance.ChangeRoomState(PhotonNetwork.IsMasterClient, PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);
        MenuUIManager.Instance.WaitingPanelPopup(false);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        print($"failed to join room, error code: {returnCode}, message: {message}");
        MenuUIManager.Instance.WaitingPanelPopup(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        print($"player entered room, Player count: {PhotonNetwork.CurrentRoom.PlayerCount}");
        MenuUIManager.Instance.ChangeRoomState(PhotonNetwork.IsMasterClient, PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        print($"player left room, Player count: {PhotonNetwork.CurrentRoom.PlayerCount}");
        MenuUIManager.Instance.ChangeRoomState(PhotonNetwork.IsMasterClient, PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        MenuUIManager.Instance.ChangeNetworkState(NetworkState.Online);
        MenuUIManager.Instance.WaitingPanelPopup(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        MenuUIManager.Instance.ChangeNetworkState(NetworkState.Offline);
    }

    [PunRPC]
    public void ChangeEnemyCount(int enemyCount)
    {
        InGameUIManager.Instance.CurrentEnemyCount.text = enemyCount.ToString();
    }

    [PunRPC]
    public void ChangeStageCount(int stageCount)
    {
        InGameUIManager.Instance.WaveCount = stageCount;
        InGameUIManager.Instance.RefreshWaveIcon(stageCount);
    }

    [PunRPC]
    public void OnMasterDead()
    {
        InGameUIManager.Instance.GameOver();
    }

    [PunRPC]
    public void OnGameClear()
    {
        InGameUIManager.Instance.GameClear();
    }
}
