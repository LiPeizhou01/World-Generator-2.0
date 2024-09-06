using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cell
{
    public Cell(Cell.BlockTypes blockType, Vector3 cellPosition, MapParameters cellParameters)
    {
        this.cellParameters = cellParameters; 
        BlockType = blockType;
        this.cellPosition = cellPosition;
    }


    public Vector3 cellPosition;

    public enum BlockTypes
    {
        empty = 0,
        stone = 1,
        soil = 2,
        soil_grass = 3,
        snow = 4,
        soil_snow = 5,
        ice = 6,
        water = 7,
        sand = 8,
        ashLand = 9,
        badland = 10,
    }

    // 晶格左右的晶格状态
    [Flags]
    public enum BlockArounds
    {
        Top = 0x01,
        bottom = 0x02,
        left = 0x04,
        right = 0x08,
        front = 0x10,
        back = 0x20
    }

    public bool IsContainWater;

    public bool HasInstantiated;

    public bool InstantiateNeeded
    {
        get
        {
            //当该晶格四周其他晶格全为非空晶格时设置该晶格不需要实例化
            //当发生任何晶格变动时应该变更BlockAround枚举
            if (((int)BlockAround & 0x3F) == 0x3F)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public MapParameters cellParameters;

    public BlockTypes BlockType
    {
        set;
        get;
    }

    public BlockArounds BlockAround = 0x00;
}
