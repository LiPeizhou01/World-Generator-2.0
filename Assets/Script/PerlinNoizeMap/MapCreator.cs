using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;
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
    static float mRadio = 0.005f;
    static int octaves_d = 5;
    static float persistence_d = 0.8f;
    static int octaves_c = 5;
    static float persistence_c = 0.8f;
    static int octaves_e = 2;
    static float persistence_e = 0.6f;
    static int octaves_pv = 4;
    static float persistence_pv = 0.8f;
    static int octaves_t_h = 2;
    static float persistence_t_h = 0.6f;

    private readonly int _mapSeed_d_c;// 地图种子
    private readonly int _mapSeed_e;
    private readonly int _mapSeed_t_h;

    private int OffSet_d_c
    {
        get
        {
            return _mapSeed_d_c & 255;
        }
    }

    private int OffSet_e
    {
        get
        {
            return _mapSeed_e & 255;
        }
    }

    private int OffSet_t_h
    {
        get
        {
            return _mapSeed_t_h & 255;
        }
    }
    public string MapSeed
    {
        get
        {
            return _mapSeed_d_c.ToString("X").PadRight(8, '0') + _mapSeed_e.ToString("X").PadRight(8, '0')+
                _mapSeed_t_h.ToString("X").PadRight(8, '0');
        }
    }
    public PerlinNoise perlinNoise = new PerlinNoise();
    public MapCreator()
    {
        // 随机生成种子
        Random rand = new Random();
        _mapSeed_d_c = rand.Next(int.MinValue, int.MaxValue);
        _mapSeed_e = rand.Next(int.MinValue, int.MaxValue);
        _mapSeed_t_h = rand.Next(int.MinValue, int.MaxValue);
    }

    public MapCreator(string mapSeed_d_c,string mapSeed_e, string mapSeed_t_h)
    {
        Random rand = new Random();

        // 判断种子是否合法，并根据提供的种子初始化地图
        if (mapSeed_d_c.Length == 8 && int.TryParse(mapSeed_d_c, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int seedValue))
        {
            this._mapSeed_d_c = seedValue;
        }
        else
        {
            _mapSeed_d_c = rand.Next(int.MinValue, int.MaxValue);
            throw new ArgumentException("the seed is incorrect!");
        }

        if (mapSeed_e.Length == 8 && int.TryParse(mapSeed_e, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int seedValue_1))
        {
            this._mapSeed_e = seedValue_1;
        }
        else
        {
            _mapSeed_e = rand.Next(int.MinValue, int.MaxValue);
            throw new ArgumentException("the seed is incorrect!");
        }

        if (mapSeed_t_h.Length == 8 && int.TryParse(mapSeed_t_h, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int seedValue_2))
        {
            this._mapSeed_t_h = seedValue_2;
        }
        else
        {
            _mapSeed_t_h = rand.Next(int.MinValue, int.MaxValue);
            throw new ArgumentException("the seed is incorrect!");
        }
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
                return new Vector2(-128.0f + (Position.x * mRadio + OffSet_d_c) * 256f / NoizeMap.textureWidth , -128.0f + (Position.z * mRadio + OffSet_d_c) * 256f / NoizeMap.textureHeight);
            case 1:
                return new Vector2(-128.0f + (Position.x * mRadio + OffSet_e) * 256f / NoizeMap.textureWidth, -128.0f + (Position.z * mRadio + OffSet_e) * 256f / NoizeMap.textureHeight);
            case 2:
                return new Vector2(-128.0f + (Position.x * mRadio + OffSet_e) * 256f / NoizeMap.textureWidth, -128.0f + (Position.z * mRadio + OffSet_e) * 256f / NoizeMap.textureHeight);
            case 3:
                return new Vector2(-128.0f + (Position.x * mRadio + OffSet_t_h) * 256f / NoizeMap.textureWidth, -128.0f + (Position.z * mRadio + OffSet_t_h) * 256f / NoizeMap.textureHeight);
            case 4:
                return new Vector2(-128.0f + (Position.x * mRadio + OffSet_t_h) * 256f / NoizeMap.textureWidth, -128.0f + (Position.z * mRadio + OffSet_t_h) * 256f / NoizeMap.textureHeight);
            default:
                return new Vector2(0.0f ,0.0f);
        }

    }

    public MapParameters GetMapParameters(Vector3 cellPosition)
    {
        MapParameters mapParameters = new MapParameters();
        // Debug.Log(OffSet_d_c);
        Vector3 cellOnMap_d_c = cellPosition * mRadio + new Vector3(OffSet_d_c, OffSet_d_c, OffSet_d_c);
        mapParameters.density = perlinNoise.OctavePerlinNoise3D(cellOnMap_d_c.x, cellOnMap_d_c.y, cellOnMap_d_c.z,octaves_d,persistence_d);
        mapParameters.continents = perlinNoise.OctavePerlinNoise3D(cellOnMap_d_c.x,0.5f + OffSet_d_c,cellOnMap_d_c.z, octaves_c,persistence_c);

        Vector3 cellOnMap_e_pv_w = (cellPosition) * mRadio + new Vector3(OffSet_e, OffSet_e, OffSet_e);
        mapParameters.pv = perlinNoise.OctavePerlinNoise3D(cellOnMap_e_pv_w.x,0.5f + OffSet_e,cellOnMap_e_pv_w.z, octaves_pv, persistence_pv);
        mapParameters.erosion = perlinNoise.OctavePerlinNoise3D(cellOnMap_e_pv_w.x,2.5f +OffSet_e ,cellOnMap_e_pv_w.z, octaves_e, persistence_e);
        mapParameters.weirdness = perlinNoise.PerlinNoise3D(cellOnMap_e_pv_w.x,4.5f + OffSet_e,cellOnMap_e_pv_w.z);

        Vector3 cellOnMap_t_h = (cellPosition) * mRadio + new Vector3(OffSet_t_h, OffSet_t_h, OffSet_t_h);
        mapParameters.temperature = perlinNoise.OctavePerlinNoise3D(cellOnMap_t_h.x,1.5f + OffSet_t_h ,cellOnMap_t_h.z, octaves_t_h, persistence_t_h);
        mapParameters.humidity = perlinNoise.OctavePerlinNoise3D(cellOnMap_t_h.x,2.5f + OffSet_t_h,cellOnMap_t_h.z, octaves_t_h, persistence_t_h);

        return mapParameters;
    }

    static int Lerp(double x, Vector2 point_1, Vector2 point_2)
    {
        float k = (point_2.y - point_1.y) / (point_2.x - point_1.x);
        return (int)(k * (x - point_2.x) + point_2.y);
    }

    // 待重构
    public static int GetHeightBasic(double c)
    {
        // 获取该位置基础高度的函数

        // 设置基础浮动设置点
        Vector2 basicPoint_1 = new Vector2(-1f, Chunk.chunkHight / 2.0f * (3.0f/4.0f));            //固定
        Vector2 point0 = new Vector2(-0.99f,-Chunk.chunkHight/2.0f * (16.0f/16.0f));
        Vector2 point1 = new Vector2(-0.6f, -Chunk.chunkHight/2.0f * (7.0f/8.0f));
        Vector2 point2 = new Vector2(-0.2f, -Chunk.chunkHight/2.0f * (1.0f/16.0f));
        Vector2 point3 = new Vector2(0.1f, Chunk.chunkHight / 2.0f * (0.0f/16.0f));
        Vector2 point4 = new Vector2(0.5f, Chunk.chunkHight/2.0f * (7.0f/8.0f));
        Vector2 point5 = new Vector2(0.7f, Chunk.chunkHight/2.0f * (13.0f/16.0f));
        Vector2 point6 = new Vector2(0.8f, Chunk.chunkHight / 2.0f * (15.0f/16.0f));
        Vector2 point7 = new Vector2(0.85f, Chunk.chunkHight / 2.0f * (14.0f/16.0f));
        Vector2 point8 = new Vector2(0.99f, Chunk.chunkHight / 2.0f * (13.0f /16.0f));
        Vector2 basicPoint_2 = new Vector2(1f, Chunk.chunkHight / 2.0f * (13.0f / 16.0f));   //固定

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
        Vector2 basicPoint_1 = new Vector2(-1f, Chunk.chunkHight / 2.0f * (1.0f / 2.0f));            //固定
        Vector2 point0 = new Vector2(-0.99f, Chunk.chunkHight / 2.0f * (1.0f / 2.0f));
        Vector2 point1 = new Vector2(-0.6f, Chunk.chunkHight / 2.0f * (1.0f / 4.0f));
        Vector2 point2 = new Vector2(-0.1f, Chunk.chunkHight / 2.0f * (1.0f / 8.0f));
        Vector2 point3 = new Vector2(0.1f, Chunk.chunkHight / 2.0f * (1.0f / 16.0f));
        Vector2 point4 = new Vector2(0.2f, Chunk.chunkHight / 2.0f * (1.5f / 16.0f));
        Vector2 point5 = new Vector2(0.3f, Chunk.chunkHight / 2.0f * (1.5f / 16.0f));
        Vector2 point6 = new Vector2(0.5f, Chunk.chunkHight / 2.0f * (1.0f / 32.0f));
        Vector2 point7 = new Vector2(0.85f, Chunk.chunkHight / 2.0f * (1.0f / 64.0f));
        Vector2 point8 = new Vector2(0.99f, Chunk.chunkHight / 2.0f * (1.0f / 64.0f));
        Vector2 basicPoint_2 = new Vector2(1f, Chunk.chunkHight / 2.0f * (1.0f / 64.0f));   //固定

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
    public Cell.BlockTypes GetBlock(Vector3 cellPosition, MapParameters parameters, int HeightBasic)
    {
        double cellDensity;
        double extrusion;

        // Debug.Log(extrusion * (cellPosition.y - HeightBasic));
        if (cellPosition.y - HeightBasic > 0)
        {
            extrusion = 1.0 / 100.0;
            cellDensity = parameters.density - extrusion * (cellPosition.y - HeightBasic);
        }
        else
        {
            extrusion = 1.0 / 80.0;
            cellDensity = parameters.density - extrusion * (cellPosition.y - HeightBasic);
        }

        parameters.temperature -= 1.0 / 100.0 * (cellPosition.y);
        parameters.humidity += 1.0/100.0 * (cellPosition.y);

        // 密度判断层
        if (cellDensity >= 0.0)
        {
            // 大陆性判断层
            if (parameters.continents >= -0.2 && parameters.continents <= 0.05)
            {
                // 侵蚀度判断层
                if (parameters.erosion >= 0.3 && parameters.pv <=0.2)
                {
                    return Cell.BlockTypes.sand;
                }
                else
                {
                    return Cell.BlockTypes.soil;
                }
            }
            else
            {
                // 侵蚀度判断层

                //温度判断层
                if (parameters.temperature <= -0.25)
                {
                    if(parameters.humidity >= 0.3)
                    {
                        return Cell.BlockTypes.ice;
                    }
                    else
                    {
                        return Cell.BlockTypes.snow;
                    }
                }
                else if (parameters.temperature >= 0.25)
                {
                    if (parameters.humidity >= 0.3)
                    {
                        return Cell.BlockTypes.ashLand;
                    }
                    else
                    {
                        return Cell.BlockTypes.badland;
                    }
                }
                else
                {
                    return Cell.BlockTypes.soil;
                }
            }
        }
        else
        {
            return Cell.BlockTypes.empty;
        }
    }
}
