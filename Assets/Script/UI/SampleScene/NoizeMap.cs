using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoizeMap : MonoBehaviour
{
    private static NoizeMap _intance;

    public static NoizeMap Instance
    {
        get
        {
            if (_intance != null)
            {
                return _intance;
            }
            else
            {
                _intance = FindObjectOfType<NoizeMap>();
                return _intance;
            }
        }
    }

    public RawImage continentsImage;
    public RawImage erosionImage;
    public RawImage pvImage;
    public RawImage tempImage;
    public RawImage humdityImage;

    public RawImage continentsPointer;
    public RawImage erosionPointer;
    public RawImage pvPointer;
    public RawImage tempPointer;
    public RawImage humdityPointer;

    public static int textureHeight = 500;
    public static int textureWidth = 500;

    public void SetNoizeMapImage()
    {
        // 创建
        Texture2D continentsTexture = new Texture2D(textureWidth, textureHeight);
        Texture2D erosionTexture = new Texture2D(textureWidth, textureHeight);
        Texture2D pvTexture = new Texture2D(textureWidth, textureHeight);
        Texture2D tempTexture = new Texture2D(textureWidth, textureHeight);
        Texture2D humdityTexture = new Texture2D(textureWidth, textureHeight);

        int defaultY = 0;
        for (int z = 0; z < textureHeight; z++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                Vector3 pixelPosition = new Vector3(-128.0f + 256.0f / 500.0f * x, defaultY, -128.0f + 256.0f / 500.0f * z);
                MapParameters currentParameters = NewWorldLoader.Instance.mapCreator.GetMapParameters(pixelPosition);
                currentParameters.Normalize();

                // 大陆性赋值
                Color continentsColor = new Color((float)currentParameters.continents, (float)currentParameters.continents, (float)currentParameters.continents, 1f);
                continentsTexture.SetPixel(x, z, continentsColor);

                // 侵蚀度赋值
                Color erosionColor = new Color((float)currentParameters.erosion, (float)currentParameters.erosion, (float)currentParameters.erosion, 1f);
                erosionTexture.SetPixel(x, z, erosionColor);

                // pv赋值
                Color pvColor = new Color((float)currentParameters.pv, (float)currentParameters.pv, (float)currentParameters.pv, 1f);
                pvTexture.SetPixel(x, z, pvColor);

                // 温度赋值
                Color tempColor = new Color((float)currentParameters.temperature, (float)currentParameters.temperature, (float)currentParameters.temperature, 1f);
                tempTexture.SetPixel(x, z, tempColor);

                // 湿度赋值
                Color humdityColor = new Color((float)currentParameters.humidity, (float)currentParameters.humidity, (float)currentParameters.humidity, 1f);
                humdityTexture.SetPixel(x, z, humdityColor);
            }
        }

        continentsTexture.Apply();
        erosionTexture.Apply();
        pvTexture.Apply();
        tempTexture.Apply();
        humdityTexture.Apply();

        // 赋值
        continentsImage.texture = continentsTexture;
        erosionImage.texture = erosionTexture;
        pvImage.texture = pvTexture;
        tempImage.texture = tempTexture;
        humdityImage.texture = humdityTexture;
    }

    public void SetPointer()
    {
        MapMessage.Instance.PointerPositionSetter(continentsPointer,MapCreator.PointerType.c);
        MapMessage.Instance.PointerPositionSetter(erosionPointer, MapCreator.PointerType.e);
        MapMessage.Instance.PointerPositionSetter(pvPointer, MapCreator.PointerType.pv);
        MapMessage.Instance.PointerPositionSetter(tempPointer, MapCreator.PointerType.t);
        MapMessage.Instance.PointerPositionSetter(humdityPointer, MapCreator.PointerType.h);
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SetPointer();
    }
}
