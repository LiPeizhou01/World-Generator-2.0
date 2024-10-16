using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;
using static Cell;
using static MapCreator;
using Random = System.Random;
public struct MapParameters
{
    public double density;
    public double continents;
    public double erosion;
    public double pv;
    public double weirdness;
    public double temperature;
    public double humidity;

    // 正规化至0~1
    public void Normalize()
    {
        density = Mathf.Clamp( (float) (density + 1.0f) / 2.0f ,-1.0f,1.0f);
        continents = Mathf.Clamp((float)(continents + 1.0f) / 2.0f, -1.0f, 1.0f);
        erosion = Mathf.Clamp((float)(erosion + 1.0f) / 2.0f, -1.0f, 1.0f);
        pv = Mathf.Clamp((float)(pv + 1.0f) / 2.0f, -1.0f, 1.0f);
        weirdness = Mathf.Clamp((float)(weirdness + 1.0f) / 2.0f, -1.0f, 1.0f);
        temperature = Mathf.Clamp((float)(temperature + 1.0f) / 2.0f, -1.0f, 1.0f);
        humidity = Mathf.Clamp((float)(humidity + 1.0f) / 2.0f, -1.0f, 1.0f);
    }
}
public class MapCreator
{
    static float radio_d = 0.01f;
    static float radio_c = 0.004f;
    static float radio_e = 0.008f;
    static float radio_w = 0.002f;
    static float radio_t = 0.005f;
    static float radio_h = 0.005f;

    static int octaves_d = 4;
    static float persistence_d = 0.8f;
    static int octaves_c = 3;
    static float persistence_c = 0.8f;
    static int octaves_e = 4;
    static float persistence_e = 0.6f;
    static int octaves_w = 4;
    static float persistence_w = 0.8f;
    static int octaves_t = 2;
    static float persistence_t = 0.6f;
    static int octaves_h = 2;
    static float persistence_h = 0.6f;

    private readonly ushort _mapSeed_d;   // 地块密度
    private readonly ushort _mapSeed_c;   //  
    private readonly ushort _mapSeed_e;
    private readonly ushort _mapSeed_w;
    private readonly ushort _mapSeed_t;
    private readonly ushort _mapSeed_h;

    private ushort[] OffSet_d
    {
        get
        {
            ushort offSet_1 = (ushort)(_mapSeed_d & 0x00FF);
            ushort offSet_2 = (ushort)(_mapSeed_d & 0xFF00 >> 8);
            return new ushort[] { offSet_1,offSet_2};
        }
    }

    private ushort[] OffSet_c
    {
        get
        {
            ushort offSet_1 = (ushort)(_mapSeed_c & 0x00FF);
            ushort offSet_2 = (ushort)(_mapSeed_c & 0xFF00 >> 8);
            return new ushort[] { offSet_1, offSet_2 };
        }
    }
    private ushort[] OffSet_e
    {
        get
        {
            ushort offSet_1 = (ushort)(_mapSeed_e & 0x00FF);
            ushort offSet_2 = (ushort)(_mapSeed_e & 0xFF00 >> 8);
            return new ushort[] { offSet_1, offSet_2 };
        }
    }

    private ushort[] OffSet_w
    {
        get
        {
            ushort offSet_1 = (ushort)(_mapSeed_w & 0x00FF);
            ushort offSet_2 = (ushort)(_mapSeed_w & 0xFF00 >> 8);
            return new ushort[] { offSet_1, offSet_2 };
        }
    }

    private ushort[] OffSet_t
    {
        get
        {
            ushort offSet_1 = (ushort)(_mapSeed_t & 0x00FF);
            ushort offSet_2 = (ushort)(_mapSeed_t & 0xFF00 >> 8);
            return new ushort[] { offSet_1, offSet_2 };
        }
    }

    private ushort[] OffSet_h
    {
        get
        {
            ushort offSet_1 = (ushort)(_mapSeed_h & 0x00FF);
            ushort offSet_2 = (ushort)(_mapSeed_h & 0xFF00 >> 8);
            return new ushort[] { offSet_1, offSet_2 };
        }
    }

    public string MapSeed
    {
        get
        {
            return _mapSeed_d.ToString("X").PadRight(4, '0') + _mapSeed_c.ToString("X").PadRight(4, '0') +
                _mapSeed_e.ToString("X").PadRight(4, '0')+ _mapSeed_w.ToString("X").PadRight(4, '0') +
                _mapSeed_t.ToString("X").PadRight(4, '0') + _mapSeed_h.ToString("X").PadRight(4, '0');
        }
    }
    public PerlinNoise perlinNoise = new PerlinNoise();
    public MapCreator()
    {
        // 随机生成种子
        Random rand = new Random();
        _mapSeed_d = (ushort)rand.Next(ushort.MinValue, ushort.MaxValue);
        _mapSeed_c = (ushort)rand.Next(ushort.MinValue, ushort.MaxValue);
        _mapSeed_e = (ushort)rand.Next(ushort.MinValue, ushort.MaxValue);
        _mapSeed_w = (ushort)rand.Next(ushort.MinValue, ushort.MaxValue);
        _mapSeed_t = (ushort)rand.Next(ushort.MinValue, ushort.MaxValue);
        _mapSeed_h = (ushort)rand.Next(ushort.MinValue, ushort.MaxValue);
    }

    public enum PointerType
    {
        c = 0,
        e = 1,
        pv = 2,
        t = 3,
        h = 4,
    }

    // 获取当前位置在地图中的位置
    public Vector2 GetNoizeMapPosition(Vector3 Position, PointerType pointerType)
    {
        switch ((int)pointerType)
        {
            case 0:
                return new Vector2(-128.0f + (Position.x * radio_c + OffSet_c[0]) * 256f / NoizeMap.textureWidth , -128.0f + (Position.z * radio_c + OffSet_c[1]) * 256f / NoizeMap.textureHeight);
            case 1:
                return new Vector2(-128.0f + (Position.x * radio_e + OffSet_e[0]) * 256f / NoizeMap.textureWidth, -128.0f + (Position.z * radio_e + OffSet_e[0]) * 256f / NoizeMap.textureHeight);
            case 2:
                return new Vector2(-128.0f + (Position.x * radio_w + OffSet_e[0]) * 256f / NoizeMap.textureWidth, -128.0f + (Position.z * radio_w + OffSet_e[0]) * 256f / NoizeMap.textureHeight);
            case 3:
                return new Vector2(-128.0f + (Position.x * radio_t + OffSet_t[0]) * 256f / NoizeMap.textureWidth, -128.0f + (Position.z * radio_t + OffSet_t[0]) * 256f / NoizeMap.textureHeight);
            case 4:
                return new Vector2(-128.0f + (Position.x * radio_h + OffSet_h[0]) * 256f / NoizeMap.textureWidth, -128.0f + (Position.z * radio_h + OffSet_h[0]) * 256f / NoizeMap.textureHeight);
            default:
                return new Vector2(0.0f ,0.0f);
        }

    }

    public MapParameters GetMapParameters(Vector3 cellPosition)
    {
        MapParameters mapParameters = new MapParameters();
        // Debug.Log(OffSet_d_c);

        // d
        Vector3 cellOnMap_d = cellPosition * radio_d + new Vector3(OffSet_d[0]*1.01f, OffSet_d[1] * 1.01f, OffSet_d[1] * 1.01f);
        mapParameters.density = PerlinNoise.OctavePerlinNoise3D(cellOnMap_d.x, cellOnMap_d.y, cellOnMap_d.z,octaves_d,persistence_d);

        // c
        Vector3 cellOnMap_c = cellPosition * radio_c + new Vector3(OffSet_c[0] * 1.01f, OffSet_c[1] * 1.01f, OffSet_c[1] * 1.01f);
        mapParameters.continents = PerlinNoise.OctavePerlinNoise2D(cellOnMap_c.x,cellOnMap_c.z, octaves_c,persistence_c);

        // e
        Vector3 cellOnMap_e = cellPosition * radio_e + new Vector3(OffSet_e[0] * 1.01f, OffSet_e[1] * 1.01f, OffSet_e[1] * 1.01f);
        mapParameters.erosion = PerlinNoise.OctavePerlinNoise2D(cellOnMap_e.x,cellOnMap_e.z, octaves_e, persistence_e);

        // w
        Vector3 cellOnMap_w = cellPosition * radio_w + new Vector3(OffSet_w[0] * 1.01f, OffSet_w[1]* 1.01f, OffSet_w[1] * 1.01f);
        mapParameters.weirdness = PerlinNoise.OctavePerlinNoise2D(cellOnMap_w.x,cellOnMap_w.z, octaves_w, persistence_w);

        // t
        Vector3 cellOnMap_t = cellPosition * radio_t + new Vector3(OffSet_t[0] * 1.01f, OffSet_t[1] * 1.01f, OffSet_t[1] * 1.01f);
        mapParameters.temperature = PerlinNoise.OctavePerlinNoise2D(cellOnMap_t.x,cellOnMap_t.z, octaves_t, persistence_t);

        // h
        Vector3 cellOnMap_h = cellPosition * radio_h + new Vector3(OffSet_h[0] * 1.01f, OffSet_h[1] * 1.01f, OffSet_h[1] * 1.01f);
        mapParameters.humidity = PerlinNoise.OctavePerlinNoise2D(cellOnMap_h.x, cellOnMap_h.z, octaves_h, persistence_h);

        // 调整输出值到-1.0f ~ 1.0f
        mapParameters.density = mapParameters.density * 2.0f - 1.0f;
        mapParameters.continents = mapParameters.continents * 2.0f - 1.0f;
        mapParameters.erosion = mapParameters.erosion * 2.0f - 1.0f;
        mapParameters.weirdness = mapParameters.weirdness * 2.0f - 1.0f;
        mapParameters.temperature = mapParameters.temperature * 2.0f - 1.0f;
        mapParameters.humidity = mapParameters.humidity * 2.0f - 1.0f;
        // pv 从动系数
        mapParameters.pv = 1.0f - Mathf.Abs(3.0f * Mathf.Abs((float)mapParameters.weirdness) - 2.0f);

        return mapParameters;
    }

    static int Lerp(double x, Vector2 point_1, Vector2 point_2)
    {
        float k = (point_2.y - point_1.y) / (point_2.x - point_1.x);
        return (int)(k * (x - point_2.x) + point_2.y);
    }

    public static int GetSnowHeightBasic(MapParameters parameters,out BlockTypes blockTypes)
    {
        double t = parameters.temperature;
        double h = parameters.humidity;

        if(t < -0.2 && t> -0.4)
        {
            blockTypes = BlockTypes.snow;
            if (h < -0.65)
            {
                return 0;
            }
            else if(h < -0.2)
            { 
                return 1;
            }
            else if(h < 0.0)
            {
                return 2;
            }
            else if (h < 0.3)
            {
                return 4;
            }
            else
            {
                return 8;
            }
        }
        else if (t <= -0.4)
        {
            blockTypes = BlockTypes.ice;
            if (h < -0.1)
            {
                return 0;
            }
            else if (h < -0.0)
            {
                return 1;
            }
            else if (h < 0.3)
            {
                return 4;
            }
            else if (h < 0.5)
            {
                return 8;
            }
            else
            {
                return 10;
            }
        }
        else
        {
            blockTypes = BlockTypes.empty;
            return 0;
        }
    }

    // 待重构
    public static int GetHeightBasic(double c)
    {
        // 获取该位置基础高度的函数

        // 设置基础浮动设置点
        Vector2 basicPoint_1 = new Vector2(-1f, 5);            //固定
        Vector2 point0 = new Vector2(-0.99f, 5);
        Vector2 point1 = new Vector2(-0.455f, 5);
        Vector2 point2 = new Vector2(-0.3f, 64);
        Vector2 point3 = new Vector2(0.3f, 64);
        Vector2 point4 = new Vector2(0.5f, 150);
        Vector2 point5 = new Vector2(0.7f, 160);
        Vector2 point6 = new Vector2(0.8f, 150);
        Vector2 point7 = new Vector2(0.85f, 200);
        Vector2 point8 = new Vector2(0.99f, 220);
        Vector2 basicPoint_2 = new Vector2(1f, 230);   //固定

        // 懒得改，凑合用
        if (c< point0.x)
        {
            return Lerp(c,basicPoint_1, point0);
        }
        else if (point0.x <= c && c < point1.x)
        {
            return Lerp(c, point0, point1);
        }
        else if (point1.x <= c && c < point2.x)
        {
            return Lerp(c, point1, point2);
        }
        else if (point2.x <= c && c < point3.x)
        {
            return Lerp(c, point2, point3);
        }
        else if (point3.x <= c && c < point4.x)
        {
            return Lerp(c, point3, point4);
        }
        else if (point4.x <= c && c < point5.x)
        {
            return Lerp(c, point4, point5);
        }
        else if (point5.x <= c && c < point6.x)
        {
            return Lerp(c, point5, point6);
        }
        else if (point6.x <= c && c < point7.x)
        {
            return Lerp(c, point6, point7);
        }
        else if (point7.x <= c && c < point8.x)
        {
            return Lerp(c, point7, point8);
        }
        else
        {
            return Lerp(c, point8, basicPoint_2);
        }
    }

    // 待重构
    public static int GetHeightErosion(double e)
    {
        // 获取该位置基础高度的函数

        // 设置基础浮动设置点
        Vector2 basicPoint_1 = new Vector2(-1f, 50);            //固定
        Vector2 point0 = new Vector2(-0.99f, 50);
        Vector2 point1 = new Vector2(-0.78f, 40);
        Vector2 point2 = new Vector2(-0.375f, 20);
        Vector2 point3 = new Vector2(0.2225f, 10);
        Vector2 point4 = new Vector2(0.05f, 2);
        Vector2 point5 = new Vector2(0.45f, 10);
        Vector2 point6 = new Vector2(0.5f, 5);
        Vector2 point7 = new Vector2(0.85f, 2);
        Vector2 point8 = new Vector2(0.99f, 2);
        Vector2 basicPoint_2 = new Vector2(1f, 1);   //固定

        // 懒得改，凑合用
        if (e < point0.x)
        {
            return Lerp(e, basicPoint_1, point0);
        }
        else if (point0.x <= e && e < point1.x)
        {
            return Lerp(e, point0, point1);
        }
        else if (point1.x <= e && e < point2.x)
        {
            return Lerp(e, point1, point2);
        }
        else if (point2.x <= e && e < point3.x)
        {
            return Lerp(e, point2, point3);
        }
        else if (point3.x <= e && e < point4.x)
        {
            return Lerp(e, point3, point4);
        }
        else if (point4.x <= e && e < point5.x)
        {
            return Lerp(e, point4, point5);
        }
        else if (point5.x <= e && e < point6.x)
        {
            return Lerp(e, point5, point6);
        }
        else if (point6.x <= e && e < point7.x)
        {
            return Lerp(e, point6, point7);
        }
        else if (point7.x <= e && e < point8.x)
        {
            return Lerp(e, point7, point8);
        }
        else
        {
            return Lerp(e, point8, basicPoint_2);
        }
    }

    // 获取地图基础地表
    public static int GetMapHeightBasic(MapParameters parameters)
    {
        return GetHeightBasic(parameters.continents) + GetHeightErosion(parameters.erosion) +  (int)(parameters.pv * GetHeightErosion(parameters.erosion));
    }

    public static int GetWaterHeightLevel(double continent, double erosion)
    {
        return GetHeightBasic(continent) + GetHeightErosion(erosion) + (int)(-0.7f * GetHeightErosion(erosion));
    }

    public static Cell.CellStates GetBlock(Vector3 cellPosition, MapParameters parameters, int HeightBasic)
    {
        double cellDensity;
        double extrusion;

        // Debug.Log(extrusion * (cellPosition.y - HeightBasic));
        if (cellPosition.y - HeightBasic > 0)
        {
            extrusion = 1.0/16.0;
            cellDensity = parameters.density - extrusion * (cellPosition.y - HeightBasic);
        }
        else
        {
            extrusion = 1.0/64.0;
            cellDensity = parameters.density - extrusion * (cellPosition.y - HeightBasic);
        }

        if (cellDensity >= 0.0)
        {
            return Cell.CellStates.block;
        }
        else
        {
            return Cell.CellStates.empty;
        }
    }
}
