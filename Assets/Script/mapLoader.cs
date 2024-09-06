using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class MapLoader : MonoBehaviour
{
    private static MapLoader _Instance;
    private MapLoader() { }
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

    public static int _mapShowSize = 9;
    static int _mapUpLoadSize = 11;
    static int _gameMapSize = 2; //控制实例化地图大小，_gameMapSize * 2 + 1

    // 实用地块矩阵
    public Chunk[,] chunkNeedToLoad = new Chunk[_mapShowSize, _mapShowSize];
    // 更新地块矩阵
    public Chunk[,] chunkNeedToUpLoad = new Chunk[_mapUpLoadSize, _mapUpLoadSize];
    public UniTask chunkNeedToUpLoadUpLoadTask;
    public GameObject player;
    public GameObject block;
    public static float totalInitializeNeed = (_gameMapSize * 2.0f + 1.0f) * (_gameMapSize * 2.0f + 1.0f);
    public static float initializeProcess = 0.0f;

    Vector3 PlayerPosition
    {
        get 
        { 
            if(player.transform.position != null)
            {
                return player.transform.position;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
    ChunkID _chunkWasIn = new ChunkID(0, 0);
    // 上一次确认时玩家所在地块ID
    ChunkID ChunkWasIn
    {
        set
        {
            _chunkWasIn = value;
        }
        get
        {
            return _chunkWasIn;
        }
    }

    // 当前玩家所在地块ID
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

    public bool IsEnterNewChunk
    {
        get
        {
            if (ChunkIsIn != ChunkWasIn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    //是否在mapNeedToLoad 5x5 中
    bool IsChunkInShowMap(ChunkID chunkID, out Chunk chunkNeeded)
    {
        foreach(Chunk chunk in chunkNeedToLoad)
        {
            if (chunk.chunkID == chunkID)
            {
                chunkNeeded = chunk;
                return true;
            }
        }
        chunkNeeded = null;
        return false;
    }

    // 是否在chunkNeedToUpLoad 7x7 里
    bool IsChunkInUpLoadMap(ChunkID chunkID, out Chunk chunkNeeded)
    {
        foreach (Chunk chunk in chunkNeedToUpLoad)
        {
            if (chunk.chunkID == chunkID)
            {
                chunkNeeded = chunk;
                return true;
            }
        }
        chunkNeeded = null;
        return false;
    }

    // 判断是否需要生成
    bool IsChunkInAreaNeedGenerate(Chunk chunk)
    {
        // 生成区域80*80格
        if((ChunkIsIn.x-_gameMapSize) <= chunk.chunkID.x && (ChunkIsIn.x + _gameMapSize) >= chunk.chunkID.x && (ChunkIsIn.z - _gameMapSize) <= chunk.chunkID.z && (ChunkIsIn.z + _gameMapSize) >= chunk.chunkID.z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 异步更新地块方法总集
    public async UniTaskVoid ChunkUpLoadAsync()
    {

        while (true)
        {
            await UniTask.Delay(500);

            if (IsEnterNewChunk)
            {
                List<UniTask> upLoadTaskList = new List<UniTask>();
                // 更新ChunkWasIn位置
                ChunkWasIn = ChunkIsIn;
                // mapNeedToLoad 更新地图状态
                foreach (Chunk chunk in chunkNeedToLoad)
                {
                    int sum = 0;
                    //如果chunk在需要生成地块中且该chunk未必实例化，携程实例化该地块
                    if (IsChunkInAreaNeedGenerate(chunk) && chunk.IsInstantiated == false)
                    {
                        Debug.Log("生成新地块");
                        GameObject chunkStorage = new GameObject("chunk_(" + chunk.chunkID.x.ToString() + "." + chunk.chunkID.z.ToString() + ")");
                        foreach (Cell cell in chunk.cells)
                        {
                            sum++;
                            if (cell.BlockType != Cell.BlockTypes.empty)
                            {
                                if (cell.InstantiateNeeded)
                                {
                                    
                                    GameObject newBlock = Instantiate(block, chunk.ChunkZeroPoint + cell.cellPosition, Quaternion.Euler(-90, 0, 0));
                                    newBlock.transform.parent = chunkStorage.transform;
                                    // 贴图
                                    newBlock.GetComponent<MeshRenderer>().materials = NewWorldLoader.Instance.MaterialDict[cell.BlockType.ToString()];
                                }
                            }
                            if (sum % (16*16*10) == 0)
                            {
                                await UniTask.NextFrame();
                            }
                        }
                        chunk.IsInstantiated = true;
                        //异步执行实例化新区域
                        //StartCoroutine(InstantiateChunkCoroutine(chunk));
                    }
                    // 如果chunk不在需要生成地块中且该chunk已被实例化，摧毁该地块
                    else if (!IsChunkInAreaNeedGenerate(chunk) && chunk.IsInstantiated == true)
                    {
                        GameObject chunkStorage = GameObject.Find("chunk_(" + chunk.chunkID.x.ToString() + "." + chunk.chunkID.z.ToString() + ")");
                        Destroy(chunkStorage);
                        chunk.IsInstantiated = false;
                        await UniTask.NextFrame();
                    }
                }

                // 更新 加载地图矩阵
                upLoadTaskList.Add(UpdateChunkNeedToLoadAsync());
                await UniTask.WhenAll(upLoadTaskList);
                upLoadTaskList.Clear();
                // 更新 更新地图矩阵
                upLoadTaskList.Add(UpdateChunkNeedToUpLoadAsync());
                //UpdateChunkNeedToUpLoad();

                await UniTask.WhenAll(upLoadTaskList);
            }
        }
        //是否进入一个新地块

    }

    // 初始化 加载地图矩阵方法
    void InitializeChunkNeedToLoad()
    {
        for (int i = 0; i < _mapShowSize; i++)
        {
            for (int j = 0; j < _mapShowSize; j++)
            {
                ChunkID loadingChunkID = new ChunkID(ChunkIsIn.x - (_mapShowSize - 1) / 2 + i, ChunkIsIn.z - (_mapShowSize - 1) / 2 + j);
                chunkNeedToLoad[i, j] = new Chunk(loadingChunkID);
            }
        }
    }

    // 更新 加载地图矩阵
    async UniTask UpdateChunkNeedToLoadAsync()
    {
        for (int i = 0; i < _mapShowSize; i++)
        {
            for (int j = 0; j < _mapShowSize; j++)
            {
                ChunkID loadingChunkID = new ChunkID(ChunkIsIn.x - (_mapShowSize - 1) / 2 + i, ChunkIsIn.z - (_mapShowSize - 1) / 2 + j);
                if (IsChunkInUpLoadMap(loadingChunkID,out Chunk chunkNeeded))
                {
                    chunkNeedToLoad[i, j] = chunkNeeded;
                }
                else
                {
                    Debug.Log("err?");
                    chunkNeedToLoad[i, j] = await UniTask.RunOnThreadPool(() =>  new Chunk(loadingChunkID));
                }
            }
        }
    }

    // 初始化 更新地图矩阵方法
    void InitializeChunkNeedToUpLoad()
    {
        for (int i = 0; i < _mapUpLoadSize; i++)
        {
            for (int j = 0; j < _mapUpLoadSize; j++)
            {
                ChunkID loadingChunkID = new ChunkID(ChunkIsIn.x - (_mapUpLoadSize - 1) / 2 + i, ChunkIsIn.z - (_mapUpLoadSize - 1) / 2 + j);

                if (IsChunkInShowMap(loadingChunkID, out Chunk chunkNeeded))
                {
                    chunkNeedToUpLoad[i,j] = chunkNeeded;
                }
                else
                {
                    chunkNeedToUpLoad[i, j] = new Chunk(loadingChunkID);
                }
            }
        }
    }

    //
    // 
    void UpdateChunkNeedToUpLoad()
    {
        Chunk[,] newUpLoadChunk = new Chunk[_mapUpLoadSize, _mapUpLoadSize];
        for (int i = 0; i < _mapUpLoadSize; i++)
        {
            for (int j = 0; j < _mapUpLoadSize; j++)
            {
                ChunkID loadingChunkID = new ChunkID(ChunkIsIn.x - (_mapUpLoadSize - 1) / 2 + i, ChunkIsIn.z - (_mapUpLoadSize - 1) / 2 + j);
                if (IsChunkInUpLoadMap(loadingChunkID, out Chunk chunkNeeded_0))
                {
                    newUpLoadChunk[i, j] = chunkNeeded_0;
                }
                else
                {
                    /*
                    if(IsChunkInShowMap(loadingChunkID, out Chunk chunkNeeded_1))
                    {
                        newUpLoadChunk[i, j] = chunkNeeded_1;
                    }
                    */
                    newUpLoadChunk[i, j] = new Chunk(loadingChunkID);
                }
            }
        }
        chunkNeedToUpLoad = newUpLoadChunk;
    }

    // 更新 更新地图矩阵方法
    async UniTask UpdateChunkNeedToUpLoadAsync()
    {
        Chunk[,] newUpLoadChunk = new Chunk[_mapUpLoadSize,_mapUpLoadSize];
        for (int i = 0; i < _mapUpLoadSize; i++)
        {
            for (int j = 0; j < _mapUpLoadSize; j++)
            {
                ChunkID loadingChunkID = new ChunkID(ChunkIsIn.x - (_mapUpLoadSize - 1) / 2 + i, ChunkIsIn.z - (_mapUpLoadSize - 1) / 2 + j);
                if (IsChunkInUpLoadMap(loadingChunkID, out Chunk chunkNeeded_0))
                {
                    newUpLoadChunk[i, j] = chunkNeeded_0;
                }
                else
                {
                    if(IsChunkInShowMap(loadingChunkID, out Chunk chunkNeeded_1))
                    {
                        newUpLoadChunk[i, j] = chunkNeeded_1;
                    }
                    newUpLoadChunk[i, j] = await UniTask.RunOnThreadPool(() => new Chunk(loadingChunkID));
                }
            }
        }
        chunkNeedToUpLoad = newUpLoadChunk;
    }

    // 查询当前实例化方块数量 主要用与compute shader
    /*
    public int GetCurrentBlockCount()
    {
        int i = 0;
        foreach (Chunk chunk in chunkNeedToLoad)
        {
            if (IsChunkInAreaNeedGenerate(chunk))
            {
                foreach (Cell cell in chunk.cells)
                {
                    if (cell.BlockType != Cell.BlockTypes.empty)
                    {
                        if (cell.InstantiateNeeded)
                        {
                            i++;
                        }
                    }
                }
            }
        }

        return i;
    }
    */

    /*
     * compute shader 用， 暂不使用
    public Matrix4x4[] GetLoadingBlockMatrixs()
    {
        List<Matrix4x4> BlockMatrixs = new List<Matrix4x4>();

        foreach (Chunk chunk in chunkNeedToLoad)
        {
            if (IsChunkInAreaNeedGenerate(chunk))
            {
                foreach (Cell cell in chunk.cells)
                {
                    if (cell.BlockType != Cell.BlockTypes.empty)
                    {
                        if (cell.InstantiateNeeded)
                        {
                            BlockMatrixs.Add(Matrix4x4.TRS(chunk.ChunkZeroPoint + cell.cellPosition, Quaternion.identity, Vector3.one));
                        }
                    }
                }
            }
        }
        return BlockMatrixs.ToArray();
    }
    */
    void InitializeGenerateBlock()
    {
        int i=0;
        int chunkNum = 0;
        foreach(Chunk chunk in chunkNeedToLoad)
        {
            if (IsChunkInAreaNeedGenerate(chunk))
            {
                GameObject BlockStorage = new GameObject("chunk_(" + chunk.chunkID.x.ToString() + "." + chunk.chunkID.z.ToString() + ")");
                foreach (Cell cell in chunk.cells)
                {
                    if(cell.BlockType != Cell.BlockTypes.empty)
                    {
                        if (cell.InstantiateNeeded)
                        {
                            GameObject newBlock = Instantiate(block, chunk.ChunkZeroPoint + cell.cellPosition, Quaternion.Euler(-90, 0, 0));
                            newBlock.transform.parent = BlockStorage.transform;
                            // 贴图
                            newBlock.GetComponent<MeshRenderer>().materials = NewWorldLoader.Instance.MaterialDict[cell.BlockType.ToString()];
                            /*
                            if (cell.BlockType == Cell.BlockTypes.water)
                            {
                                Collider collider = newBlock.GetComponent<Collider>();

                                // 禁用 Collider
                                
                                if (collider != null)
                                {
                                    collider.enabled = false;
                                }
                            }
                            */
                            i++;
                        }
                    }
                }
                chunk.IsInstantiated = true;
            }
            chunkNum++;
            initializeProcess = chunkNum / totalInitializeNeed;
            Debug.Log(initializeProcess);
        }
        Debug.Log($"bolck has being creat{i}");
    }

    public void InitializeMap()
    {
        InitializeChunkNeedToLoad();
        InitializeChunkNeedToUpLoad();
        InitializeGenerateBlock();
        foreach(Chunk chunk in chunkNeedToLoad)
        {
            Debug.Log($"chunk.ID{chunk.chunkID.x}.{chunk.chunkID.z},IsChunkInAreaNeedGenerate: {IsChunkInAreaNeedGenerate(chunk)}, IsInstantiated: {chunk.IsInstantiated}");
        }

        foreach(Chunk chunk in chunkNeedToUpLoad)
        {
            Debug.Log($"chunkUp.ID{chunk.chunkID.x}.{chunk.chunkID.z},IsChunkInAreaNeedGenerate: {IsChunkInAreaNeedGenerate(chunk)}, IsInstantiated: {chunk.IsInstantiated}");
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
