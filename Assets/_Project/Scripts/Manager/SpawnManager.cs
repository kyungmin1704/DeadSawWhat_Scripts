using DG.Tweening;
using Lean.Pool;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : Singleton<SpawnManager>
{
    public GameObject enemy;
    public EnemyListData enemyListData;
    private Transform[] spawnPoints;
    private List<Transform> spawnAblePoints;
    private Dictionary<EnemyType, GameObject> enemies;
    private int currentWave = 0;
    public int CurrentWave
    {
        get => currentWave;
        private set => currentWave = value;
    }
    public int MaxWave => currentStageWavesData.waveList.Count;

    private bool isReadyForNextWave = false;

    private readonly Queue<SpawnQueue> spawnQueue = new Queue<SpawnQueue>();
    /// <summary>
    /// 적 스폰 큐를 생성하는 프로퍼티
    /// SpawnQueue 데이터 타입을 할당하여 큐를 생성
    /// </summary>
    public SpawnQueue Spawn
    {
        set => spawnQueue.Enqueue(value);
    }

    private readonly List<GameObject> spawned = new List<GameObject>();
    /// <summary>
    /// 적 사망시 적 리스트에서 해당 오브젝트를 제거하는 프로퍼티
    /// 제거할 오브젝트를 GameObject 타입으로 할당하여 제거
    /// </summary>
    public GameObject Despawn
    {
        set
        {
            if (spawned.Contains(value))
                spawned.Remove(value);
            
            if (!PhotonNetwork.InRoom)
            {
                LeanPool.Despawn(value);
            }
            else if (PhotonNetwork.IsMasterClient)
            {
                value.SetActive(false);
                DOVirtual.DelayedCall(3f, () => PhotonNetwork.Destroy(value));
            }
            else
            {
                value.SetActive(false);
            }
            
            InGameUIManager.Instance.KillCount++;
            if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
            {
                InGameUIManager.Instance.CurrentEnemyCount.text = spawned.Count.ToString();
                if (PhotonNetwork.InRoom) NetworkManager.Pv.RPC("ChangeEnemyCount", RpcTarget.Others, spawned.Count);
            }
        }
    }

    public GameObject Spawned
    {
        set
        {
            value.GetComponent<IInitializable>().Init();
            if (!spawned.Contains(value))
                spawned.Add(value);
            if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
            {
                InGameUIManager.Instance.CurrentEnemyCount.text = spawned.Count.ToString();
                if (PhotonNetwork.InRoom) NetworkManager.Pv.RPC("ChangeEnemyCount", RpcTarget.Others, spawned.Count);
            }
        }
    }

    [SerializeField] private StageWavesData currentStageWavesData;

    private bool isOnWave;
    public bool IsOnWave
    {
        get => isOnWave; private set => isOnWave = value;
    }

    public StageWavesData CurrentStageWavesData
    {
        get => currentStageWavesData;
        private set => currentStageWavesData = value;
    }

    private bool isEndGame = false;
    public bool IsEndGame
    {
        get => isEndGame;
        private set
        {
            isEndGame = value;
            if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
            {
                InGameUIManager.Instance.GameClear();
                if (PhotonNetwork.InRoom) NetworkManager.Pv.RPC("OnGameClear", RpcTarget.Others);
            }
        }
    }

    private void Update()
    {
        ResolveSpawnQueue();
    }

    /// <summary>
    /// 스테이지 진입시 스테이지의 웨이브 정보를 파라미터로 전달 받는 메서드입니다.
    /// 해당 메서드를 통해 웨이브 정보를 할당하고 적스폰 코루틴이 시작됩니다.
    /// </summary>
    /// <param name="stageWavesData">스테이지의 스폰 정보</param>
    public void Init(StageWavesData stageWavesData)
    {
        spawnAblePoints = new List<Transform>();
        enemies = new Dictionary<EnemyType, GameObject>();
        foreach (EnemyData i in enemyListData.enemyDatas)
        {
            enemies.Add(i.type, i.enemyPrefab);
        }

        spawnPoints = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }
        CurrentStageWavesData = stageWavesData;
        StartCoroutine(SpawningCoroutin());
    }

    void ResolveSpawnQueue()
    {
        if (spawnQueue.Count > 0)
        {
            //  스폰된 적 오브젝트가 21개 이상이라면 객체를 해결하지 않음
            if (spawned.Count > 20) return;
            
            spawnAblePoints.Clear();
            foreach (Transform i in spawnPoints)
            {
                if (Vector3.Distance(GameManager.Instance.player.transform.position, i.position) > 10f)
                {
                    spawnAblePoints.Add(i);
                }
            }

            //  스폰 가능한 장소가 없다면 해결하지 않음
            if (spawnAblePoints.Count < 1) return;

            SpawnQueue queue = spawnQueue.Dequeue();
            Vector3 spawnPos = spawnAblePoints[Random.Range(0, spawnAblePoints.Count)].position;
            if (PhotonNetwork.InRoom)
            {
                Spawned = PhotonNetwork.Instantiate(enemies[queue.Type].name, spawnPos, Quaternion.identity);
            }
            else
            {
                Spawned = LeanPool.Spawn(enemies[queue.Type], spawnPos, Quaternion.identity);
            }
        }
    }

    /// <summary>
    /// StageWavesData를 기반으로 적 스폰 큐에 적재해주는 코루틴 입니다.
    /// 해당 코루틴은 직접 적을 생성하진 않습니다.
    /// 스테이지 준비가 완료된 이후 Init 메서드를 통해 시작됩니다.
    /// </summary>
    IEnumerator SpawningCoroutin()
    {
        yield return null;
        WaitUntil waitUntilReady = new WaitUntil(() => isReadyForNextWave);
        foreach (WaveData wd in currentStageWavesData.waveList)
        {
            isReadyForNextWave = false;
            StartCoroutine(WaitForNextWaveCoroutin());
            yield return waitUntilReady;
            InGameUIManager.Instance.RefreshWaveIcon(CurrentWave);
            CurrentWave++;
            InGameUIManager.Instance.WaveCount = CurrentWave;
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                NetworkManager.Pv.RPC("ChangeStageCount", RpcTarget.Others, CurrentWave);
            }
            foreach (SpawnData sd in wd.spawnList)
            {
                for (int i = 0; i < sd.count; i++)
                {
                    Spawn = new SpawnQueue(sd.type, sd.healthMultiplier);
                }

                yield return new WaitForSeconds(sd.interval);
            }
        }
        yield return new WaitUntil(() => spawnQueue.Count == 0);
        yield return new WaitUntil(() => spawned.Count == 0);
        IsEndGame = true;
    }

    IEnumerator WaitForNextWaveCoroutin()
    {
        yield return new WaitUntil(() => spawned.Count == 0);
        IsOnWave = false;
        yield return new WaitForSeconds(5f);
        IsOnWave = true;
        isReadyForNextWave = true;
    }
}