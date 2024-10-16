using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PerlinNoise
{
    public static float PerlinNoise2D(float x, float z)
    {
        return Mathf.PerlinNoise(x, z);
    }

    public static float OctavePerlinNoise2D(float x, float z, int octaves, float persistence)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;

        for (int i = 0; i < octaves; i++)
        {
            total += PerlinNoise2D(x * frequency, z * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= 2;
        }

        return total / maxValue;
    }


    // 自制柏林噪声，由于个别取值上的问题现在弃用
    // 留供参考
    // 梯度置换表，完全使用Ken Perlin的源码中的置换表
    private static readonly int[] permutation = { 151,160,137,91,90,15,
    131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
    190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
    88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
    77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
    102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
    135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
    5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
    223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
    129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
    251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
    49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
    138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180};

    private static int[] p;

    static PerlinNoise()
    {
        // 静态构造函数，为防止索引溢出将上面置换表再重复一遍
        p = new int[512];
        for (int i = 0; i < 512; i++)
        {
            p[i] = permutation[i % 256];
        }
    }
    public static double Fade(double t)
    {
        // 插值函数，func = 6t^5 -15t^4 + 10t^3
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static double Lerp(double x, double a, double b)
    {
        // 线性插值计算
        return a + (b - a) * x;
    }

    private static double Grad_2D(int hash, double x, double y, double angle_1, double angle_2)
    {
        switch (hash & 0x07)
        {
            // 梯度表 8种情况，这里angle_1和angle_2是从地图种子来随机的
            // 同一个地图种子，会一定生成同个角度。
            // 从每一个角度，生成单位圆向量，再通过旋转90度生成4个新的单位圆向量，两个角度生成8个。
            case 0x00: return Math.Cos(angle_1) * x + Math.Sin(angle_1) * y;
            case 0x01: return -Math.Sin(angle_1) * x + Math.Cos(angle_1) * y;
            case 0x02: return -Math.Cos(angle_1) * x - Math.Sin(angle_1) * y;
            case 0x03: return +Math.Sin(angle_1) * x - Math.Cos(angle_1) * y;
            case 0x04: return Math.Cos(angle_2) * x + Math.Sin(angle_2) * y;
            case 0x05: return -Math.Sin(angle_2) * x + Math.Cos(angle_2) * y;
            case 0x06: return -Math.Cos(angle_2) * x - Math.Sin(angle_2) * y;
            case 0x07: return Math.Sin(angle_2) * x - Math.Cos(angle_2) * y;
            default: return 0;
        }
    }

    public double PerlinNoise2D_self(double x, double y, double angle_1, double angle_2)
    {
        // 利用强转和位与求该点整数部分除与256的余数。
        int xi = (int)x & 255;
        int yi = (int)y & 255;

        // 分离小数部分
        double xf = x - (int)x;
        double yf = y - (int)y;

        // 取得小数部分的插值结果
        double u = Fade(xf);
        double v = Fade(yf);

        // 从置换表里置换一个伪随机数，用于之后的抽取梯度
        int AA = p[p[xi] + yi];
        int AB = p[p[xi] + yi + 1];
        int BA = p[p[xi + 1] + yi];
        int BB = p[p[xi + 1] + yi + 1];

        // 用Grad函数伪随机梯度，并用Lerp做插值计算
        return Lerp(v, Lerp(u, Grad_2D(p[AA], xf, yf, angle_1, angle_2), Grad_2D(p[BA], xf - 1, yf, angle_1, angle_2)), Lerp(u, Grad_2D(p[AB], xf, yf - 1, angle_1, angle_2), Grad_2D(p[BB], xf - 1, yf - 1, angle_1, angle_2)));
    }
    public double OctavePerlinNoise2D_self(double x, double y, int octaves, double persistence, double angle_1, double angle_2)
    {
        // 波形叠加函数，octaves是波形叠加次数，persistence为振幅衰减
        double total = 0;
        double frequency = 1;
        double amplitude = 1;
        double maxValue = 0;
        for (int i = 0; i < octaves; i++)
        {
            total += PerlinNoise2D_self(x * frequency, y * frequency, angle_1, angle_2) * amplitude;

            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= 2;
        }

        return total / maxValue;
    }


    public static double PerlinNoise3D(double x, double y, double z)
    {

        int xi = (int)x & 255;                              // Calculate the "unit cube" that the point asked will be located in
        int yi = (int)y & 255;                              // The left bound is ( |_x_|,|_y_|,|_z_| ) and the right bound is that
        int zi = (int)z & 255;                              // plus 1.  Next we calculate the location (from 0.0 to 1.0) in that cube.
        double xf = x - (int)x;                             // We also fade the location to smooth the result.
        double yf = y - (int)y;
        double zf = z - (int)z;

        double u = Fade(xf);
        double v = Fade(yf);
        double w = Fade(zf);

        int AAA, ABA, AAB, ABB, BAA, BBA, BAB, BBB;
        AAA = p[p[p[xi] + yi] + zi];
        ABA = p[p[p[xi] + yi + 1] + zi];
        AAB = p[p[p[xi] + yi] + zi + 1];
        ABB = p[p[p[xi] + yi + 1] + zi + 1];
        BAA = p[p[p[xi + 1] + yi] + zi];
        BBA = p[p[p[xi + 1] + yi + 1] + zi];
        BAB = p[p[p[xi + 1] + yi] + zi + 1];
        BBB = p[p[p[xi + 1] + yi + 1] + zi + 1];

        double x1, x2, y1, y2;
        x1 = Lerp(u,Grad_3D(AAA, xf, yf, zf),                // The gradient function calculates the dot product between a pseudorandom
                    Grad_3D(BAA, xf - 1, yf, zf)              // gradient vector and the vector from the input coordinate to the 8
                    );                                     // surrounding points in its unit cube.
        x2 = Lerp(u,Grad_3D(ABA, xf, yf - 1, zf),                // This is all then lerped together as a sort of weighted average based on the faded (u,v,w)
                    Grad_3D(BBA, xf - 1, yf - 1, zf)              // values we made earlier.
                      );
        y1 = Lerp(v, x1, x2);

        x1 = Lerp(u,Grad_3D(AAB, xf, yf, zf - 1),
                    Grad_3D(BAB, xf - 1, yf, zf - 1)
                    );
        x2 = Lerp(u,Grad_3D(ABB, xf, yf - 1, zf - 1),
                      Grad_3D(BBB, xf - 1, yf - 1, zf - 1)
                      );
        y2 = Lerp(v, x1, x2);

        return (Lerp(w, y1, y2)+1.0)/2.0;                       // For convenience we bound it to 0 - 1 (theoretical min/max before is -1 - 1)
    }

    public static double Grad_3D(int hash, double x, double y, double z)
    {
        switch (hash & 0xF)
        {
            case 0x0: return x + y;
            case 0x1: return -x + y;
            case 0x2: return x - y;
            case 0x3: return -x - y;
            case 0x4: return x + z;
            case 0x5: return -x + z;
            case 0x6: return x - z;
            case 0x7: return -x - z;
            case 0x8: return y + z;
            case 0x9: return -y + z;
            case 0xA: return y - z;
            case 0xB: return -y - z;
            case 0xC: return y + x;
            case 0xD: return -y + z;
            case 0xE: return y - x;
            case 0xF: return -y - z;
            default: return 0; // never happens
        }
    }

    public static double OctavePerlinNoise3D(double x, double y, double z, int octaves, double persistence)
    {
        double total = 0;
        double frequency = 1;
        double amplitude = 1;
        double maxValue = 0;  // Used for normalizing result to 0.0 - 1.0
        for (int i = 0; i < octaves; i++)
        {
            total += PerlinNoise3D(x * frequency, y * frequency, z * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= 2;
        }

        return total / maxValue;
    }
    
}


