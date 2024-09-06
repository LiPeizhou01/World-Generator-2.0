using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static Cell;


// 地块ID XY代表地块2维位置，可为负
public struct ChunkID
{
    public ChunkID(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public int x;
    public int z;

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

public class Chunk
{
    public Chunk(ChunkID chunkID)
    {
        this.chunkID = chunkID;
        ChunkCellSetter();
    }

    public ChunkID chunkID;
    public GameObject block;
    public static int chunkLength = 16;
    public static int chunkWidth = 16;
    public static int chunkHight = 128;

    public int[,,] chunkSurface = new int[16, 16, 2];

    // 是否已被实例化
    public bool IsInstantiated = false;


    public Vector3 ChunkZeroPoint
    {
        get
        {
            return new Vector3(chunkID.x * chunkLength,-chunkHight/2,chunkID.z * chunkWidth);
        }
    }

    public Cell[,,] cells= new Cell[chunkLength,chunkHight, chunkWidth];

    int[,] HeightBasic = new int[chunkLength,chunkWidth];

    // 区分地块 土壤
    bool IsTypeOfSoil(BlockTypes blockTypes)
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
    bool IsTypeOfWater(BlockTypes blockTypes)
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
    bool IsTypeOfStone(BlockTypes blockTypes)
    {
        if(blockTypes == BlockTypes.stone || blockTypes == BlockTypes.badland || blockTypes == BlockTypes.sand)
        {
            return true;
        }
        else
        {
            return false;
        }
      
    }
    public void ChunkCellSetter()
    {
        for (int z = chunkWidth-1; z >= 0 ; z--)
        {
            for (int x = chunkLength -1; x >= 0; x--)
            {
                // 表面计数
                int num = 0;
                BlockTypes lastBlockTypes = new BlockTypes();
                for (int y = chunkHight - 1; y >= 0; y--)
                {
                   
                    // 获取当前地块位置
                    Vector3 cellPosition = ChunkZeroPoint + new Vector3(x, y, z);
                    // 获取当前地块生成参数
                    MapParameters mapParameters = NewWorldLoader.Instance.mapCreator.GetMapParameters(cellPosition);


                    if (y == chunkHight -1)
                    {
                        HeightBasic[x,z] = MapCreator.GetHeightBasic(mapParameters.continents);
                        int HeightErosion = MapCreator.GetHeightErosion(mapParameters.erosion);
                        // Debug.Log($"heightBasic:{HeightBasic},heightErosion{HeightErosion}");

                        HeightBasic[x,z] =+ (int)(HeightErosion * mapParameters.pv);
                        // Debug.Log($"y:{cellPosition.y},density:{mapParameters.density},C:{mapParameters.continents},e:{mapParameters.erosion},pv:{mapParameters.pv}");
                    }

                    BlockTypes blockType = NewWorldLoader.Instance.mapCreator.GetBlock(cellPosition, mapParameters, HeightBasic[x,z]);

                    // 第二个参数传入的是相对位置
                    cells[x,y,z] = new Cell(blockType, new Vector3(x, y, z), mapParameters);

                    // 生成海洋河流逻辑
                    
                    if (cells[x, y, z].BlockType  == BlockTypes.empty)
                    {
                        // 海洋逻辑
                        if (y <= chunkHight / 2)
                        {
                            if (mapParameters.temperature <= -0.3)
                            {
                                cells[x, y, z].BlockType = BlockTypes.ice;
                            }
                            else
                            {
                                cells[x, y, z].BlockType = BlockTypes.water;
                            }
                        }
                        /*
                        // 河流，湖泊逻辑
                        else
                        {
                            if (y <= HeightBasic[x,z])
                            if (mapParameters.humidity > 0.4 && mapParameters.pv < -0.2)
                            {
                                if (mapParameters.temperature <= -0.2)
                                {
                                    cells[x, y, z].BlockType = BlockTypes.ice;
                                }
                                else
                                {
                                    cells[x, y, z].BlockType = BlockTypes.water;
                                }
                            }
                        }
                        */
                        
                    }
                    


                    // 获取地表
                    if (num <= 1)
                    {
                        if ((lastBlockTypes == BlockTypes.empty || lastBlockTypes == BlockTypes.water) && (blockType != BlockTypes.empty && blockType != BlockTypes.water))
                        {
                            chunkSurface[x, z, num] = y;
                            if (blockType == BlockTypes.soil && mapParameters.temperature > 0)
                            {
                                cells[x, y, z].BlockType = BlockTypes.soil_grass;
                            }
                            else if (blockType == BlockTypes.soil && mapParameters.temperature <= 0)
                            {
                                cells[x, y, z].BlockType = BlockTypes.soil_snow;
                            }
                            num++;
                        }
                        lastBlockTypes = blockType;
                    }
                }
            }
        }


        // 设置晶格实例化需求并调整地块构成
        for (int z = 0; z < chunkWidth; z++)
        {
            for (int y = 0; y < chunkHight; y++)
            {
                for (int x = 0; x < chunkLength; x++)
                {
                    BlockTypes blockType = cells[x, y, z].BlockType;
                    int currentSurface_1 = chunkSurface[x, z, 0];
                    int currentSurface_2 = chunkSurface[x, z, 1];
                    // 判断晶格是否为空
                    if (blockType != Cell.BlockTypes.empty )
                    {
                        // 判断砖块位置切换砖块状态
                        if (IsTypeOfSoil(blockType))
                        {
                            if (y <= currentSurface_1 - 3 && (y <= currentSurface_2 - 3 || y > currentSurface_2))
                            {
                                blockType = BlockTypes.stone;
                            }
                        }
                        else if (IsTypeOfWater(blockType) || IsTypeOfStone(blockType))
                        {
                            //
                        }
                        else
                        {
                            if ((currentSurface_1 - 6<= y && y <= currentSurface_2 - 3) || (y <= currentSurface_1 - 3 && y > currentSurface_2 - 6))
                            {
                                blockType = BlockTypes.soil;
                            }
                            else if ((y > currentSurface_1 - 3 && y <= currentSurface_1) || (y > currentSurface_2 - 3 && y <= currentSurface_2))
                            {
                                //
                            }
                            else
                            {
                                blockType = BlockTypes.stone;
                            }
                        }
                        cells[x, y, z].BlockType = blockType;

                        // 设置地块是否需要实例化
                        if ( x==0 |y==0 |z==0 | x == chunkLength-1 | y == chunkHight - 1| z == chunkWidth -1)
                        {
                            // 如果该晶格在chunk边缘直接设置其为周围全空
                            cells[x, y, z].BlockAround = 0x00;
                        }
                        else
                        {
                            // 检查上方晶格
                            if (cells[x, y + 1, z].BlockType != Cell.BlockTypes.empty)
                            {
                                cells[x, y, z].BlockAround |= BlockArounds.Top;
                            }
                            else
                            {
                                continue;
                            }
                            // 检查下方晶格
                            if (cells[x, y-1, z].BlockType != Cell.BlockTypes.empty)
                            {
                                cells[x, y, z].BlockAround |= BlockArounds.bottom;
                            }
                            else
                            {
                                continue;
                            }
                            // 检查左方晶格
                            if (cells[x -1, y, z].BlockType != Cell.BlockTypes.empty)
                            {
                                cells[x, y, z].BlockAround |= BlockArounds.left;
                            }
                            else
                            {
                                continue;
                            }
                            // 检查右方晶格
                            if (cells[x + 1, y, z].BlockType != Cell.BlockTypes.empty)
                            {
                                cells[x, y, z].BlockAround |= BlockArounds.right;
                            }
                            else
                            {
                                continue;
                            }
                            // 检查前方晶格
                            if (cells[x, y, z-1].BlockType != Cell.BlockTypes.empty)
                            {
                                cells[x, y, z].BlockAround |= BlockArounds.front;
                            }
                            else
                            {
                                continue;
                            }
                            // 检查后方晶格
                            if (cells[x, y, z + 1].BlockType != Cell.BlockTypes.empty)
                            {
                                cells[x, y, z].BlockAround |= BlockArounds.back;
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

    
}
