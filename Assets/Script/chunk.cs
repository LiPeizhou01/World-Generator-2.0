using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Cell;


// 地块ID XY代表地块2维位置，可为负
public struct ChunkID:IComparable<ChunkID>
{
    public ChunkID(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public int x;
    public int z;

    public static int GetDistance(ChunkID chunkID_1,ChunkID chunkID_2)
    {
        return Mathf.Abs(chunkID_1.x -chunkID_2.x) + 
            Mathf.Abs(chunkID_1.z-chunkID_2.z);
    }

    public int CompareTo(ChunkID Other)
    {
        int currentDistance = GetDistance(this, MapLoader.Instance.ChunkIsIn);
        int otherDistance = GetDistance(Other, MapLoader.Instance.ChunkIsIn);

        return currentDistance.CompareTo(otherDistance);
    }

    public static bool operator !=(ChunkID chunkID_1,ChunkID chunkID_2)
    {
        if(chunkID_1.x == chunkID_2.x && chunkID_1.z == chunkID_2.z)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static bool operator ==(ChunkID chunkID_1, ChunkID chunkID_2)
    {
        if (chunkID_1.x == chunkID_2.x && chunkID_1.z == chunkID_2.z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool Equals(object obj)
    {
        return obj is ChunkID iD &&
               x == iD.x &&
               z == iD.z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }
}

public enum ChunkLevel
{
        NotNeed = -1,
    Level_0 = 0,
    Level_1 = 1,
    Level_2 = 2,
    Level_3 = 3,
    Level_4 = 4,
    Complete = 10,
}
public enum SurfaceCondition
{
    // A类：热带气候
    热带雨林气候 = 0,    // Af: 热带雨林
    热带季风气候 = 1,    // Am: 热带季风
    热带草原气候 = 2,    // Aw: 热带草原（干湿季）

    // B类：干旱气候
    热带沙漠气候 = 3,    // BWh: 热带沙漠
    温带沙漠气候 = 4,    // BWk: 温带沙漠
    热带草原气候干旱 = 5, // BSh: 半干旱草原（热带）
    温带草原气候干旱 = 6, // BSk: 半干旱草原（温带）

    // C类：温带气候
    温带海洋性气候 = 7,   // Cfb: 温带海洋性
    地中海气候 = 8,      // Csa: 地中海气候
    温带季风气候 = 9,    // Cwa: 温带季风气候

    // D类：大陆性气候
    温带大陆性气候 = 10,   // Dfb: 温带大陆性气候
    亚寒带大陆性气候 = 11, // Dfc: 亚寒带大陆性气候

    // E类：寒带气候
    寒带苔原气候 = 12,     // ET: 寒带苔原
    寒带冰原气候 = 13,     // EF: 寒带冰原
}

public enum TerrainType
{
    // A类: 沿海地区
    近海平原 = 0,
    近海山地 = 1,

    // B类: 大陆地区
    平原 = 2,
    多山 = 3,

    // C类: 高海拔地区
    高原 = 4,
    高峰 = 5,

    // D类: 海洋地区
    海 = 6,
}

// 备忘分类
public enum Type_c
{
    深海 = 0,  //(-1.05 ~ -0.455)
    海洋 = 1,  //(-0.455 ~ -0.19)
    沿岸 = 2,  //(-0.19 ~ -0.11)
    准内陆 = 3,  //(-0.11 ~ 0.03)
    内陆 = 4,  //(0.03 ~ 0.3)
    深大陆 = 5  //(0.3~1.0)
}
public enum Type_e
{
    Level_1 = 0,  //(-1 ~ -0.78)
    Level_2 = 1,  //(-0.78 ~ -0.375)
    Level_3 = 2,  //(-0.375 ~ -0.2225)
    Level_4 = 3,  //(-0.2225 ~ 0.05)
    Level_5 = 4,  //(0.05 ~ 0.45)
    Level_6 = 5,  //(0.55 ~ 1.0)
}
public enum Type_pv
{
    河流 = 0,
    低山带_1 = 1,
    低山带_2 = 2,
    高山带 = 3,
    山峰带 =4,
}

public class Chunk : MonoBehaviour , IComparable<Chunk>
{
    public static Chunk CreatInstance(ChunkID chunkID)
    {
        GameObject chunkStorage = new GameObject("chunk_(" + chunkID.x.ToString() + "." + chunkID.z.ToString() + ")");

        Chunk instance = chunkStorage.AddComponent<Chunk>();
        instance.Intialize(chunkID);

        return instance;
    }

    private void Intialize(ChunkID chunkID)
    {
        this.chunkID = chunkID;
        chunkLevel = 0;
    }

    public ChunkLevel chunkLevel;

    public ChunkID chunkID;
    public GameObject block;

    public static int chunkLength = 16;
    public static int chunkWidth = 16;
    public static int chunkHight = 256;

    // 需要演算来设置的变量
    //level_0
    public int[,] heightBasic = new int[chunkLength, chunkWidth];
    public int averageHeightBasic;
    public double averageTemperature;
    public double averageHumidty;
    public double averageContinents;
    public double averageErosion;
    public SurfaceCondition chunkCondition;
    public SurfaceCondition[,] surfaceConditions = new SurfaceCondition[chunkLength, chunkWidth];
    public TerrainType chunkTerrainType;
    public TerrainType[,] terrainTypes = new TerrainType[chunkLength, chunkWidth];

    //Level_1
    public Cell[,,] cells = new Cell[chunkLength, chunkHight, chunkWidth];
    public int[,,] chunkSurface = new int[16, 16, 2];
    public int averageChunkSurface;

    //Level_2
    //进行cell演算逻辑

    //Level_3
    public int waterHeightLevel;  // 含水高度

    // 是否已被实例化
    public bool IsInstantiated = false;
    public bool IsEnable = false;
    int InstantiateCount = 0;
    public UniTask CaculateTask;

    // 自带属性
    public Vector3 ChunkZeroPoint
    {
        get
        {
            return new Vector3(chunkID.x * chunkLength,0,chunkID.z * chunkWidth);
        }
    }

    public ChunkLevel NeededChunkLevel
    {
        get
        {
            int playerDistance = ChunkID.GetDistance(MapLoader.Instance.ChunkIsIn,chunkID);

            if (playerDistance <= MapLoader.CcomplateSize)
            {
                return ChunkLevel.Complete;
            }
            else if (playerDistance <= MapLoader.DistanceLevel_4)
            {
                return ChunkLevel.Level_3;
            }
            else if (playerDistance <= MapLoader.DistanceLevel_3)
            {
                return ChunkLevel.Level_3;
            }
            else if (playerDistance <= MapLoader.DistanceLevel_2)
            {
                return ChunkLevel.Level_2;
            }
            else if (playerDistance <= MapLoader.DistanceLevel_1)
            {
                return ChunkLevel.Level_1;
            }
            else if (playerDistance <= MapLoader.DistanceLevel_0)
            {
                return ChunkLevel.Level_0;
            }
            else
            {
                return ChunkLevel.NotNeed;
            }
        }
    }

    // 方法
    public ChunkLevel GetChunkLevel(ChunkID referenceChunk)
    {
        int playerDistance = ChunkID.GetDistance(referenceChunk, chunkID);

        if (playerDistance <= MapLoader.CcomplateSize)
        {
            return ChunkLevel.Complete;
        }
        else if (playerDistance <= MapLoader.DistanceLevel_4)
        {
            return ChunkLevel.Level_3;
        }
        else if (playerDistance <= MapLoader.DistanceLevel_3)
        {
            return ChunkLevel.Level_3;
        }
        else if (playerDistance <= MapLoader.DistanceLevel_2)
        {
            return ChunkLevel.Level_2;
        }
        else if (playerDistance <= MapLoader.DistanceLevel_1)
        {
            return ChunkLevel.Level_1;
        }
        else if (playerDistance <= MapLoader.DistanceLevel_0)
        {
            return ChunkLevel.Level_0;
        }
        else
        {
            return ChunkLevel.NotNeed;
        }
    }

    void ClearBlock()
    {
        // 遍历父对象的所有子对象
        foreach (Transform child in gameObject.transform)
        {
            // 销毁子对象
            Destroy(child.gameObject);

        }
        IsInstantiated = false;
        InstantiateCount = 0;
    }

    public int CompareTo(Chunk Other)
    {
        int currentDistance = ChunkID.GetDistance(chunkID, MapLoader.Instance.ChunkIsIn);
        int otherDistance = ChunkID.GetDistance(Other.chunkID, MapLoader.Instance.ChunkIsIn);

        return currentDistance.CompareTo(otherDistance);
    }

    private void Update()
    {
        // 如果地块状态为已完成，返回主线程执行实例化，实例化会适时释放线程
        
        if (chunkLevel == ChunkLevel.Complete && InstantiateCount == 0 && !IsInstantiated)
        {
            if (ChunkID.GetDistance(chunkID,MapLoader.Instance.ChunkIsIn) <= MapLoader.DistanceShowMap)
            {
                InstantiateCount = 1;
                MainThreadDispatcher.Instance().Enqueue(() =>
                {
                    MapLoader.Instance.GenerateBlockAsync(this);
                }, this);
            }
        }

        if (NeededChunkLevel != ChunkLevel.Complete && InstantiateCount != 0 && IsInstantiated)
        {
            ClearBlock();
        }
    }
}
