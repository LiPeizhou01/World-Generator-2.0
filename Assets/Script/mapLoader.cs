using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MapLoader : MonoBehaviour
{
    private static MapLoader _Instance;
    public static MapLoader Instance
    {
        get
        {
            if (_Instance != null)
            {
                return _Instance;
            }
            else
            {
                _Instance = (MapLoader)FindObjectOfType(typeof(MapLoader));
                return _Instance;
            }
        }
    }

    public GameObject player;
    public GameObject block;
    public static int rate = 16;

    private SemaphoreSlim semaphore = new SemaphoreSlim(3); // 允许同时执行的最大任务数

    static int _showMapSize =15;
    static int _complate = 23;
    static int _level_4 = 25;
    static int _level_3 = 29;
    static int _level_2 = 31;
    static int _level_1 = 33;
    static int _level_0 = 35;
    static public int DistanceShowMap
    {
        get
        {
            return (_showMapSize - 1) / 2;
        }
    }

    static public int CcomplateSize
    {
        get
        {
            return (_complate - 1) / 2;
        }
    }
    static public int DistanceLevel_4
    {
        get
        {
            return (_level_4 - 1) / 2;
        }
    }
    static public int DistanceLevel_3
    {
        get
        {
            return (_level_3 - 1) / 2;
        }
    }
    static public int DistanceLevel_2
    {
        get
        {
            return (_level_2 - 1) / 2;
        }
    }
    static public int DistanceLevel_1
    {
        get
        {
            return (_level_1 - 1) / 2;
        }
    }
    static public int DistanceLevel_0
    {
        get
        {
            return (_level_0 - 1) / 2;
        }
    }

    public Dictionary<ChunkID, Chunk> mapContains = new Dictionary<ChunkID, Chunk>();
    IEnumerable<ChunkID> ChunkMap()
    {
        for(int r = 0; r <= DistanceLevel_0; r++)
        {
            for (int z = -r; z <= r; z++)
            {
                if (Math.Abs(z) == r)
                {
                    yield return new ChunkID(ChunkIsIn.x + z, ChunkIsIn.z + r - Math.Abs(z));
                }
                else
                {
                    yield return new ChunkID(ChunkIsIn.x + z, ChunkIsIn.z + r - Math.Abs(z));
                    yield return new ChunkID(ChunkIsIn.x + z, ChunkIsIn.z - r + Math.Abs(z));
                }
            }
        }
    }

    // 玩家位置 实时更新
    Vector3 PlayerPosition
    {
        get
        {
            if (player.transform.position != null)
            {
                return player.transform.position;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
    // 玩家所在地块ID
    public ChunkID ChunkIsIn
    {
        get
        {
            int x = 0;
            int z = 0;
            if (PlayerPosition.x >= 0)
            {
                x = (int)PlayerPosition.x / Chunk.chunkLength;
            }
            if (PlayerPosition.x < 0)
            {
                x = (int)PlayerPosition.x / Chunk.chunkLength - 1;
            }

            if (PlayerPosition.z >= 0)
            {
                z = (int)PlayerPosition.z / Chunk.chunkWidth;
            }
            if (PlayerPosition.z < 0)
            {
                z = (int)PlayerPosition.z / Chunk.chunkWidth - 1;
            }
            return new ChunkID(x, z);
        }
    }

    MapAlgorithmLevel_0 CalculatorLevel_0 = new MapAlgorithmLevel_0();
    MapAlgorithmLevel_1 CalculatorLevel_1 = new MapAlgorithmLevel_1();
    MapAlgorithmLevel_2 CalculatorLevel_2 = new MapAlgorithmLevel_2();
    MapAlgorithmLevel_3 CalculatorLevel_3 = new MapAlgorithmLevel_3();
    MapAlgorithmLevel_4 CalculatorLevel_4 = new MapAlgorithmLevel_4();

    // 计算下一层级 地图
    void CalculateNextLevel(Chunk chunk)
    {
        switch ((int)chunk.chunkLevel)
        {
            case 0:
                CalculatorLevel_0.CalculateMap(chunk);
                break;
            case 1:
                CalculatorLevel_1.CalculateMap(chunk);
                break;
            case 2:
                CalculatorLevel_2.CalculateMap(chunk);
                break;
            case 3:
                CalculatorLevel_3.CalculateMap(chunk);
                break;
            case 4:
                CalculatorLevel_4.CalculateMap(chunk);
                break;
            default:
                break;
        }
    }

    void SetChunk(Chunk chunk, ChunkLevel neededChunkLevel)
    {
        while(neededChunkLevel > chunk.chunkLevel)
        {
            CalculateNextLevel(chunk);
        }
    }
    /*
    public async UniTask SetChunkAsync(Chunk chunk, ChunkLevel neededChunkLevel)
    {
        await UniTask.RunOnThreadPool(() =>
        {
            SetChunk(chunk, neededChunkLevel);
        });
    }
    public IEnumerator CheckTaskCompletion(Chunk chunk, ChunkLevel neededChunkLevel)
    {
        while (true)
        {
            // 等待任务完成
            yield return new WaitUntil(() => chunk.CaculateTask.Status.IsCompleted());

            // 当任务完成时，重新启动异步任务
            chunk.CaculateTask = SetChunkAsync(chunk, neededChunkLevel);

            yield return new WaitForSeconds(1f);
        }
    }
    */
    public static void EnableChunk(Chunk chunk,ChunkLevel neededChunkLevel)
    {
        if (chunk.IsInstantiated && neededChunkLevel == ChunkLevel.Complete && !chunk.IsEnable)
        {
            GameObject chunkStorage = chunk.gameObject;
            if (chunkStorage == null)
            {
                Debug.LogWarning("Chunk storage not found: " + "chunk_(" + chunk.chunkID.x.ToString() + "." + chunk.chunkID.z.ToString() + ")");
                return;
            }
            chunkStorage.SetActive(true);
            chunk.IsEnable = true;
        }
    }

    public static void DisableChunk(Chunk chunk, ChunkLevel neededChunkLevel)
    {
        if (chunk.IsInstantiated && neededChunkLevel != ChunkLevel.Complete && chunk.IsEnable)
        {
            GameObject chunkStorage = chunk.gameObject;
            if (chunkStorage == null)
            {
                Debug.LogWarning("Chunk storage not found: " + "chunk_(" + chunk.chunkID.x.ToString() + "." + chunk.chunkID.z.ToString() + ")");
                return;
            }
            chunkStorage.SetActive(false);
            chunk.IsEnable = false;
        }
    }

    // 必须在主线程中调用，适时释放程序
    public async void GenerateBlockAsync(Chunk chunk)
    {
        // await UniTask.SwitchToMainThread();

        //实例化该地块
        if (chunk.IsInstantiated == false)
        {
            int sum = 0;
            chunk.gameObject.SetActive(false);
            foreach (Cell cell in chunk.cells)
            {
                sum++;
                if (cell.BlockType != Cell.BlockTypes.empty)
                {
                    if (cell.InstantiateNeeded)
                    {

                        GameObject newBlock = Instantiate(block, chunk.ChunkZeroPoint + cell.cellPosition, Quaternion.Euler(-90, 0, 0));
                        newBlock.transform.parent = chunk.gameObject.transform;
                        // 贴图
                        newBlock.GetComponent<MeshRenderer>().materials = NewWorldLoader.Instance.MaterialDict[cell.BlockType.ToString()];
                    }
                }
                
                if (sum % (16 * 16 * rate) == 0)
                {
                    await UniTask.NextFrame();
                }
            }
            chunk.IsInstantiated = true;
            chunk.gameObject.SetActive(true);
            chunk.IsEnable = true;
        }
    }

    void AddChunkMap()
    {
        foreach (ChunkID chunkID in ChunkMap())
        {
            if (mapContains.ContainsKey(chunkID))
            {
                continue;
            }
            else
            {
                mapContains.Add(chunkID,　Chunk.CreatInstance(chunkID));
            }
        }
    }
    void RemoveChunkMap()
    {
        List<ChunkID> keysToRemove = new List<ChunkID>();

        foreach (KeyValuePair<ChunkID, Chunk> pairs in mapContains)
        {
            if (pairs.Value.NeededChunkLevel == ChunkLevel.NotNeed)
            {
                GameObject chunkStorage = pairs.Value.gameObject;
                Destroy(chunkStorage);
                pairs.Value.IsInstantiated = false;
                keysToRemove.Add(pairs.Key);
            }
        }

        // 遍历结束后再移除
        foreach (ChunkID key in keysToRemove)
        {
            mapContains.Remove(key);
        }
    }

    public async UniTaskVoid UpdateMapAsync()
    {
        while (true)
        {
            Debug.Log("new!");
            AddChunkMap();
            RemoveChunkMap();

            IOrderedEnumerable<KeyValuePair< ChunkID,Chunk>> sortedContains = mapContains.OrderBy(pair => pair.Key);
            ChunkID referenceChunk = ChunkIsIn;

            
            await UniTask.RunOnThreadPool(() =>
                {
                    foreach (KeyValuePair<ChunkID, Chunk> pairs in mapContains)
                    {
                        Chunk currentChunk = pairs.Value;
                        ChunkLevel neededChunkLevel = currentChunk.GetChunkLevel(referenceChunk);
                        SetChunk(currentChunk, neededChunkLevel);
                        int threadId = Thread.CurrentThread.ManagedThreadId;
                        Debug.Log($"{currentChunk.chunkID.x},{currentChunk.chunkID.z},in threadId:{threadId}, has been set!");
                    }
                }
            );

            /*
            var tasks = new List<UniTask>();

            foreach (KeyValuePair<ChunkID, Chunk> pairs in sortedContains)
            {
                Chunk currentChunk = pairs.Value;
                ChunkLevel neededChunkLevel = currentChunk.NeededChunkLevel;

                await semaphore.WaitAsync(); // 等待可用的信号量
                tasks.Add(UniTask.RunOnThreadPool(() =>
                {
                    try
                    {
                        SetChunk(currentChunk, neededChunkLevel);
                        int threadId = Thread.CurrentThread.ManagedThreadId;
                    }
                    finally
                    {
                        semaphore.Release(); // 释放信号量
                    }
                }));
            }

            // 等待所有任务完成
            await UniTask.WhenAll(tasks);
            */
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}