using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static Cell;
using static Chunk;

public static class MapCondition
{
    public static int seaLevel = 64;
    static public SurfaceCondition DetermineCondition(double temperature, double humidity, double continents)
    {
        // 将温度从 -1.0 ~ 1.0 映射到实际温度范围，假设 -1.0 对应 -30°C，1.0 对应 40°C
        double actualTemperature = temperature * 60; // 实际温度范围约为 -50°C 到 50°C

        // A类：热带气候（年均温 > 18°C）
        if (actualTemperature > 30)
        {
            if (humidity > 0.8)
                return SurfaceCondition.热带雨林气候; // 热带雨林 (Af)
            else if (humidity > 0.6)
                return SurfaceCondition.热带季风气候; // 热带季风 (Am)
            else
                return SurfaceCondition.热带草原气候; // 热带草原 (Aw)
        }
        // B类：干旱气候
        else if (humidity < -0.5) // 湿度小于 -0.5
        {
            if (actualTemperature > 30)
                return SurfaceCondition.热带沙漠气候; // 热带沙漠 (BWh)
            else
                return SurfaceCondition.温带沙漠气候; // 温带沙漠 (BWk)
        }
        else if (humidity < -0.2) // 湿度小于 -0.2
        {
            if (actualTemperature > 30)
                return SurfaceCondition.热带草原气候干旱; // 半干旱草原 (BSh)
            else
                return SurfaceCondition.温带草原气候干旱; // 半干旱草原 (BSk)
        }
        // C类：温带气候（最冷月平均温度 > -3°C）
        else if (actualTemperature > -3 && actualTemperature <= 30)
        {
            if (humidity > 0.7 && continents < -0.1) // 大陆性偏低
                return SurfaceCondition.温带海洋性气候; // 温带海洋性气候 (Cfb)
            else if (humidity > 0.5 && continents > 0.5) // 大陆性偏高，湿度偏低
                return SurfaceCondition.地中海气候; // 地中海气候 (Csa)
            else
                return SurfaceCondition.温带季风气候; // 温带季风气候 (Cwa)
        }
        // D类：大陆性气候（最冷月平均温度 < -3°C，最热月平均温度 > -10°C）
        else if (actualTemperature <= -3 && actualTemperature > -10)
        {
            if (continents > 0.5)
                return SurfaceCondition.温带大陆性气候; // 温带大陆性气候 (Dfb)
            else
                return SurfaceCondition.亚寒带大陆性气候; // 亚寒带大陆性气候 (Dfc)
        }
        // E类：寒带气候（最热月平均温度 < 10°C）
        else if (actualTemperature <= -10)
        {
            if (actualTemperature > -30)
                return SurfaceCondition.寒带苔原气候; // 寒带苔原气候 (ET)
            else
                return SurfaceCondition.寒带冰原气候; // 寒带冰原气候 (EF)
        }
        // 如果没有匹配到，返回默认值
        return SurfaceCondition.寒带苔原气候;
    }

    static public TerrainType GetTerrainType(double continents, double erosion)
    {
        // D类 海 大陆性低于-0.1
        if (continents < -0.1)
        {
            return TerrainType.海;  // 海
        }
        // A类 沿海地区
        else if (continents >= -0.1 && continents < 0.2)
        {
            if (erosion > 0.3f)  
            {
                return TerrainType.近海平原;  //近海平原
            }
            else
            {
                return TerrainType.近海山地;  //近海山地
            }
        }
        // B类 大陆地区
        else if (continents >= 0.2 && continents < 0.6)
        {
            if (erosion > 0.3f)
            {
                return TerrainType.平原;
            }
            else
            {
                return TerrainType.多山;
            }
        }
        // C类 高海拔地区
        else
        {
            if (erosion > 0.3f)
            {
                return TerrainType.高原;
            }
            else
            {
                return TerrainType.高峰;
            }
        }
    }
    static public BlockTypes GetBlockTypes(SurfaceCondition surfaceCondition, TerrainType terrainType)
    {
        switch (surfaceCondition)
        {
            case SurfaceCondition.热带雨林气候:             //Af 热带雨林
                if (terrainType == TerrainType.近海平原)
                {
                    return BlockTypes.sand;
                }
                else
                {
                    return BlockTypes.soil;
                }
            case SurfaceCondition.热带季风气候:             //Am 热代季风
                if (terrainType == TerrainType.近海平原)
                {
                    return BlockTypes.sand;
                }
                else
                {
                    return BlockTypes.soil;
                }
            case SurfaceCondition.热带草原气候:             //Aw 热代季风
                if (terrainType == TerrainType.近海平原)
                {
                    return BlockTypes.sand;
                }
                else
                {
                    return BlockTypes.soil;
                }
            case SurfaceCondition.热带沙漠气候:
                return BlockTypes.sand;
            case SurfaceCondition.温带沙漠气候:
                return BlockTypes.badland;
            case SurfaceCondition.温带草原气候干旱:
                return BlockTypes.ashLand;
            case SurfaceCondition.热带草原气候干旱:
                return BlockTypes.sand;                   // 待修正贴图
            case SurfaceCondition.温带海洋性气候:         // Cfb 不需要判断地形
                return BlockTypes.soil;
            case SurfaceCondition.地中海气候:             // Csa 不需要判断地形
                return BlockTypes.soil;
            case SurfaceCondition.温带季风气候:
                return BlockTypes.soil;
            case SurfaceCondition.温带大陆性气候:
                return BlockTypes.soil;
            case SurfaceCondition.亚寒带大陆性气候:
                return BlockTypes.soil;
            case SurfaceCondition.寒带苔原气候:
                if (terrainType == TerrainType.平原 || terrainType == TerrainType.多山)
                {
                    return BlockTypes.soil;
                }
                else if (terrainType == TerrainType.高原 || terrainType == TerrainType.高峰)
                {
                    return BlockTypes.mossy_2;
                }
                else
                {
                    return BlockTypes.soil;
                }
            case SurfaceCondition.寒带冰原气候:
                if (terrainType == TerrainType.海)
                {
                    return BlockTypes.ice;
                }
                else
                {
                    return BlockTypes.soil;
                }
            default:
                return BlockTypes.soil;
        }
    }
    static public BlockTypes GetSurfaceBlockTypes(SurfaceCondition surfaceCondition, TerrainType terrainType)
    {
        //在DetermineCondition判断
        switch (surfaceCondition)
        {
            case SurfaceCondition.热带雨林气候:             //Af 热带雨林
                if(terrainType == TerrainType.近海平原)
                {
                    return BlockTypes.sand;
                }
                else
                {
                    return BlockTypes.soil_grass;
                }
            case SurfaceCondition.热带季风气候:             //Am 热代季风
                if (terrainType == TerrainType.近海平原)
                {
                    return BlockTypes.sand;
                }
                else
                {
                    return BlockTypes.soil_grass;
                }
            case SurfaceCondition.热带草原气候:             //Aw 热代季风
                if (terrainType == TerrainType.近海平原)
                {
                    return BlockTypes.sand;
                }
                else
                {
                    return BlockTypes.soil_grass;
                }
            case SurfaceCondition.热带沙漠气候:
                return BlockTypes.sand;
            case SurfaceCondition.温带沙漠气候:
                return BlockTypes.badland;
            case SurfaceCondition.温带草原气候干旱:
                return BlockTypes.ashLand;
            case SurfaceCondition.热带草原气候干旱:
                return BlockTypes.sand;                   // 待修正贴图
            case SurfaceCondition.温带海洋性气候:         // Cfb 不需要判断地形
                return BlockTypes.soil;
            case SurfaceCondition.地中海气候:             // Csa 不需要判断地形
                return BlockTypes.soil_grass;
            case SurfaceCondition.温带季风气候:
                if (terrainType == TerrainType.近海平原)
                {
                    return BlockTypes.soil;
                }
                else
                {
                    return BlockTypes.soil_grass;
                }
            case SurfaceCondition.温带大陆性气候:
                if (terrainType == TerrainType.近海平原)
                {
                    return BlockTypes.soil;
                }
                else
                {
                    return BlockTypes.soil_grass;
                }
            case SurfaceCondition.亚寒带大陆性气候:
                if (terrainType == TerrainType.近海平原)
                {
                    return BlockTypes.soil;
                }
                else
                {
                    return BlockTypes.soil_grass;
                }
            case SurfaceCondition.寒带苔原气候:
                if (terrainType == TerrainType.平原 || terrainType == TerrainType.多山)
                {
                    return BlockTypes.soil_snow;
                }
                else if (terrainType == TerrainType.高原 || terrainType == TerrainType.高峰)
                {
                    return BlockTypes.mossy_2;
                }
                else
                {
                    return BlockTypes.soil;
                }
            case SurfaceCondition.寒带冰原气候:
                if (terrainType == TerrainType.海)
                {
                    return BlockTypes.ice;
                }
                else
                {
                    return BlockTypes.soil_snow;
                }
            default:
                return BlockTypes.soil_grass;
        }
    }

    // 区分地块 土壤
    public static bool IsTypeOfSoil(BlockTypes blockTypes)
    {
        if (blockTypes == BlockTypes.soil || blockTypes == BlockTypes.soil_grass || blockTypes == BlockTypes.soil_snow || blockTypes == BlockTypes.ashLand)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //区分地块 水
    public static bool IsTypeOfWater(BlockTypes blockTypes)
    {
        if (blockTypes == BlockTypes.water || blockTypes == BlockTypes.ice)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 区分地块 岩石
    public static bool IsTypeOfStone(BlockTypes blockTypes)
    {
        if (blockTypes == BlockTypes.stone || blockTypes == BlockTypes.badland || blockTypes == BlockTypes.sand)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

}

// 运算该层级地图委托
public delegate void CalculateMapLevel(Chunk chunk);

public abstract class MapAlgorithm
{
    protected CalculateMapLevel delCalculateMapLevel;
    // 所有地图层级逻辑类必须实现，设置地块当前层级的方法
    protected abstract void SetChunkLevel(Chunk chunk);

    protected abstract void MapLevelDelSetter(Chunk chunk);

    public void CalculateMap(Chunk chunk)
    {
        // 添加所有方法
        MapLevelDelSetter(chunk);

        if (delCalculateMapLevel != null)
        {
            delCalculateMapLevel(chunk); 
        }
        else
        {
            throw new InvalidOperationException("delCalculateMapLevel 委托未被设置");
        }
    }
}

//第0层地图运算逻辑
public class MapAlgorithmLevel_0 : MapAlgorithm
{
    void GetBasic(Chunk chunk)
    {
        int heightAverage = 0;
        double temperatureAverage = 0.0f;
        double humdityAverage = 0.0f;
        double continentsAverage = 0.0f;
        double erosionAverage = 0.0f;
        for (int z = Chunk.chunkWidth - 1; z >= 0; z--)
        {
            for (int x = Chunk.chunkLength - 1; x >= 0; x--)
            {
                // 获取当前地块位置
                Vector3 cellPosition = chunk.ChunkZeroPoint + new Vector3(x, 0, z);
                // 获取当前地块生成参数
                MapParameters mapParameters = NewWorldLoader.Instance.mapCreator.GetMapParameters(cellPosition);

                // 演算所有平均值
                int height = MapCreator.GetMapHeightBasic(mapParameters);
                chunk.heightBasic[x, z] = height;
                chunk.surfaceConditions[x, z] = MapCondition.DetermineCondition(mapParameters.temperature,mapParameters.humidity, mapParameters.continents);
                chunk.terrainTypes[x, z] = MapCondition.GetTerrainType(mapParameters.continents,mapParameters.erosion);
                heightAverage += chunk.heightBasic[x, z];
                erosionAverage += mapParameters.erosion;
                temperatureAverage += mapParameters.temperature;
                humdityAverage += mapParameters.humidity;
                continentsAverage += mapParameters.continents;
            }
        }

        chunk.averageErosion = erosionAverage/(Chunk.chunkLength * Chunk.chunkWidth);
        chunk.averageHeightBasic = heightAverage/(Chunk.chunkLength * Chunk.chunkWidth);
        chunk.averageTemperature = temperatureAverage / (Chunk.chunkLength * Chunk.chunkWidth) - 1.0 / 100.0 * chunk.averageHeightBasic;
        chunk.averageContinents = continentsAverage / (Chunk.chunkLength * Chunk.chunkWidth) - 1.0 / 100.0 * chunk.averageHeightBasic;
        chunk.averageHumidty = humdityAverage / (Chunk.chunkLength * Chunk.chunkWidth);

    }

    void GetChunkCondition(Chunk chunk)
    {
        chunk.chunkCondition = MapCondition.DetermineCondition(chunk.averageTemperature,chunk.averageHumidty,chunk.averageContinents);
        chunk.chunkTerrainType = MapCondition.GetTerrainType(chunk.averageContinents,chunk.averageContinents);
    }

    protected override void SetChunkLevel(Chunk chunk)
    {
        chunk.chunkLevel = ChunkLevel.Level_1;
    }

    // 重写委托运算委托
    protected override void MapLevelDelSetter(Chunk chunk)
    {
        delCalculateMapLevel = null;
        delCalculateMapLevel += GetBasic;
        delCalculateMapLevel += GetChunkCondition;
        delCalculateMapLevel += SetChunkLevel;
    }
}

public class MapAlgorithmLevel_1 : MapAlgorithm
{
    protected override void SetChunkLevel(Chunk chunk)
    {
        chunk.chunkLevel = ChunkLevel.Level_2;
    }

    void CreatCell(Chunk chunk)
    {
        for (int z = Chunk.chunkWidth - 1; z >= 0; z--)
        {
            for (int x = Chunk.chunkLength - 1; x >= 0; x--)
            {
                for (int y = Chunk.chunkHight - 1; y >= 0; y--)
                {

                    // 获取当前地块位置
                    Vector3 cellPosition = chunk.ChunkZeroPoint + new Vector3(x, y, z);
                    // 获取当前地块生成参数
                    MapParameters mapParameters = NewWorldLoader.Instance.mapCreator.GetMapParameters(cellPosition);

                    CellStates cellState = MapCreator.GetBlock(cellPosition, mapParameters, chunk.heightBasic[x, z]);

                    // 第二个参数传入的是相对位置
                    chunk.cells[x, y, z] = new Cell(cellState, new Vector3(x, y, z), mapParameters);
                }
            }
        }
    }

    void GetSurface(Chunk chunk)
    {
        for (int z = Chunk.chunkWidth - 1; z >= 0; z--)
        {
            for (int x = Chunk.chunkLength - 1; x >= 0; x--)
            {
                // 表面计数
                int num = 0;
                CellStates lastCellState = new CellStates();
                for (int y = Chunk.chunkHight - 1; y >= 0; y--)
                {
                    CellStates cellState = chunk.cells[x,y,z].CellState;
                    // 获取地表
                    if (num <= 1)
                    {
                        if (lastCellState == CellStates.empty && cellState != CellStates.empty)
                        {
                            chunk.chunkSurface[x, z, num] = y;
                            chunk.cells[x, y, z].IsSurface = true;
                            num++;
                        }
                        lastCellState = cellState;
                    }
                }
            }
        }
    }

    void GetAverageSurfaceHeight(Chunk chunk)
    {
        int averageSurfaceHeight = 0;
        int[,] firstSurface = new int[Chunk.chunkLength, Chunk.chunkWidth];  // 创建二维数组

        for (int i = 0; i < Chunk.chunkLength; i++)
        {
            for (int j = 0; j < Chunk.chunkWidth; j++)
            {
                firstSurface[i, j] = chunk.chunkSurface[i, j, 0];  // 将第0层的数据提取出来
            }
        }
        foreach (int surfaceHeight in firstSurface)
        {
            averageSurfaceHeight += surfaceHeight;
        }

        chunk.averageChunkSurface = averageSurfaceHeight / (Chunk.chunkLength * Chunk.chunkWidth);
    }
    protected override void MapLevelDelSetter(Chunk chunk)
    {
        delCalculateMapLevel = null;
        // 添加逻辑
        delCalculateMapLevel += CreatCell;
        delCalculateMapLevel += GetSurface;
        delCalculateMapLevel += GetAverageSurfaceHeight;
        delCalculateMapLevel += SetChunkLevel;
    }
}

public class MapAlgorithmLevel_2 : MapAlgorithm
{
    protected override void SetChunkLevel(Chunk chunk)
    {
        chunk.chunkLevel = ChunkLevel.Level_3;
    }

    void SetBlockType(Chunk chunk)
    {
        for (int z = Chunk.chunkWidth - 1; z >= 0; z--)
        {
            for (int x = Chunk.chunkLength - 1; x >= 0; x--)
            {
                for (int y = Chunk.chunkHight - 1; y >= 0; y--)
                {
                    Cell currentCell = chunk.cells[x, y, z];

                    if (currentCell.CellState == CellStates.empty)
                    {
                        // 晶格状态为空时
                        currentCell.BlockType = BlockTypes.empty;
                    }
                    else
                    {
                        if (currentCell.IsSurface)
                        {
                            //表面Cell的情况
                            currentCell.BlockType = MapCondition.GetSurfaceBlockTypes(chunk.surfaceConditions[x, z], chunk.terrainTypes[x,z]);
                        }
                        else
                        {
                            //非表面Cell的情况
                            //可添加表面距离判断逻辑
                            //


                            currentCell.BlockType = MapCondition.GetBlockTypes(chunk.surfaceConditions[x, z], chunk.terrainTypes[x,z]);
                        }
                    }
                }
            }
        }
    }

    void SetWater(Chunk chunk)
    {
        chunk.waterHeightLevel = MapCreator.GetWaterHeightLevel(chunk.averageContinents, chunk.averageErosion);
    }

    void AddWater(Chunk chunk)
    {
        for (int z = Chunk.chunkWidth - 1; z >= 0; z--)
        {
            for (int x = Chunk.chunkLength - 1; x >= 0; x--)
            {
                for (int y = Chunk.chunkHight - 1; y >= 0; y--)
                {
                    Cell currentCell = chunk.cells[x, y, z];

                    if (currentCell.BlockType == BlockTypes.empty)
                    {
                        // 晶格地块状态为空时判断是否需要放置水为空时
                        if (y <= MapCondition.seaLevel)
                        {
                            currentCell.BlockType = BlockTypes.water;
                        }
                        else if (y <= chunk.waterHeightLevel)
                        {
                            currentCell.BlockType = BlockTypes.water;
                        }
                    }
                }
            }
        }
    }

    void AddSnow(Chunk chunk)
    {
        for (int z = Chunk.chunkWidth - 1; z >= 0; z--)
        {
            for (int x = Chunk.chunkLength - 1; x >= 0; x--)
            {
                if (chunk.chunkSurface[x, z, 0] > chunk.waterHeightLevel && chunk.chunkSurface[x, z, 0] > MapCondition.seaLevel)
                {
                    int AddHeight = MapCreator.GetSnowHeightBasic(chunk.cells[x, 0, z].cellParameters, out BlockTypes addBlockType);
                    for (int nums = 1; nums <= AddHeight; nums++)
                    {
                        if (chunk.chunkSurface[x, z, 0] + nums < Chunk.chunkHight)
                        {
                            chunk.cells[x, chunk.chunkSurface[x, z, 0] + nums, z].BlockType = addBlockType;
                        }
                    }
                }
            }
        }
    }



    protected override void MapLevelDelSetter(Chunk chunk)
    {
        delCalculateMapLevel = null;
        // 添加逻辑
        delCalculateMapLevel += SetBlockType;
        delCalculateMapLevel += SetWater;
        delCalculateMapLevel += AddWater;
        delCalculateMapLevel += AddSnow;
        delCalculateMapLevel += SetChunkLevel;
    }
}

public class MapAlgorithmLevel_3 : MapAlgorithm
{
    protected override void SetChunkLevel(Chunk chunk)
    {
        chunk.chunkLevel = ChunkLevel.Level_4;
    }

    void ChunkInstantiateNeed(Chunk chunk)
    {
        for (int z = 0; z < chunkWidth; z++)
        {
            for (int y = 0; y < chunkHight; y++)
            {
                for (int x = 0; x < chunkLength; x++)
                {
                    BlockTypes blockType = chunk.cells[x, y, z].BlockType;
                    int currentSurface_1 = chunk.chunkSurface[x, z, 0];
                    int currentSurface_2 = chunk.chunkSurface[x, z, 1];
                    // 判断晶格是否为空
                    if (blockType != Cell.BlockTypes.empty)
                    {
                        // 设置地块是否需要实例化
                        if (x == 0 | y == 0 | z == 0 | x == chunkLength - 1 | y == chunkHight - 1 | z == chunkWidth - 1)
                        {
                            // 如果该晶格在chunk边缘直接设置其为周围全空
                            chunk.cells[x, y, z].BlockAround = 0x00;
                        }
                        else
                        {
                            // 检查上方晶格
                            if (chunk.cells[x, y + 1, z].BlockType != Cell.BlockTypes.empty)
                            {
                                chunk.cells[x, y, z].BlockAround |= BlockArounds.Top;
                            }
                            else
                            {
                                continue;
                            }
                            // 检查下方晶格
                            if (chunk.cells[x, y - 1, z].BlockType != Cell.BlockTypes.empty)
                            {
                                chunk.cells[x, y, z].BlockAround |= BlockArounds.bottom;
                            }
                            else
                            {
                                continue;
                            }
                            // 检查左方晶格
                            if (chunk.cells[x - 1, y, z].BlockType != Cell.BlockTypes.empty)
                            {
                                chunk.cells[x, y, z].BlockAround |= BlockArounds.left;
                            }
                            else
                            {
                                continue;
                            }
                            // 检查右方晶格
                            if (chunk.cells[x + 1, y, z].BlockType != Cell.BlockTypes.empty)
                            {
                                chunk.cells[x, y, z].BlockAround |= BlockArounds.right;
                            }
                            else
                            {
                                continue;
                            }
                            // 检查前方晶格
                            if (chunk.cells[x, y, z - 1].BlockType != Cell.BlockTypes.empty)
                            {
                                chunk.cells[x, y, z].BlockAround |= BlockArounds.front;
                            }
                            else
                            {
                                continue;
                            }
                            // 检查后方晶格
                            if (chunk.cells[x, y, z + 1].BlockType != Cell.BlockTypes.empty)
                            {
                                chunk.cells[x, y, z].BlockAround |= BlockArounds.back;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
            }
        }
    }

    protected override void MapLevelDelSetter(Chunk chunk)
    {
        delCalculateMapLevel = null;
        delCalculateMapLevel += ChunkInstantiateNeed;
        delCalculateMapLevel += SetChunkLevel;
        // 添加逻辑
    }
}
public class MapAlgorithmLevel_4 : MapAlgorithm
{
    protected override void MapLevelDelSetter(Chunk chunk)
    {
        delCalculateMapLevel = null;
        delCalculateMapLevel += SetChunkLevel;
    }

    protected override void SetChunkLevel(Chunk chunk)
    {
        chunk.chunkLevel = ChunkLevel.Complete;
    }
}
 