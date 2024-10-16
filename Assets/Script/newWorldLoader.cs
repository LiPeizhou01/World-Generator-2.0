using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class NewWorldLoader : MonoBehaviour
{
    private NewWorldLoader() { }
    static private NewWorldLoader _Instance;
    UniTask mapUpLoadTask;
    static public NewWorldLoader Instance
    {
        get
        {
            if ( _Instance == null)
            {
                _Instance = (NewWorldLoader)FindObjectOfType(typeof(NewWorldLoader)); ;
                return _Instance;
            }
            else
            {
                return _Instance;
            }

        }
    }

    // 全局地图创建器，所有地图信息演算要用单例模式引用这个
    public MapCreator mapCreator;

    public GameObject block;
    public GameObject player;

    public static string blockFilePath = Application.streamingAssetsPath + "/BlockINFO.xml";
    Sprite[] blockSprites;
    public Texture2D[] blockTextures;
    public Dictionary<string, Material[]> MaterialDict = new Dictionary<string, Material[]>();
    void LoadBlockResourse(string path, Texture2D[] TextureList)
    {
        Debug.Log(path);
        IEnumerable<XElement> blockTypes;
        if (File.Exists(path))
        {
            XDocument blockINFO = XDocument.Load(blockFilePath);
            Debug.Log("read BlockINFO.xml");
            XElement rt = blockINFO.Root;
            Debug.Log(rt);
            blockTypes = rt.Elements();

            foreach (var blockType in blockTypes)
            {
                string tag = blockType.Element("Tag").Value;
                Debug.Log(tag);
                // 配置所有种类方块贴图资源。
                Material[] blockTypeMaterial = SetMaterial(TextureList, blockType);
                MaterialDict.Add(tag, blockTypeMaterial);
            }
            foreach (var item in MaterialDict.Keys)
            {
                Debug.Log(item);
            }
        }
        else
        {
            Debug.Log("BlockINFO.xml is not exist!");
        }

    }

    Texture2D SpriteToTexture2D(Sprite sprite)
    {
        // 获取 Sprite 的纹理数据
        Texture2D texture = sprite.texture;

        // 获取 Sprite 的边界和矩形区域
        Rect rect = sprite.textureRect;
        Texture2D newTexture = new Texture2D((int)rect.width, (int)rect.height);
        Debug.Log($"rect.width: {rect.width},rect.height: {rect.height}");
        newTexture.SetPixels(texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height));
        newTexture.Apply();

        return newTexture;
    }

    Material[] SetMaterial(Texture2D[] TextureList, XElement blockType)
    {
        try
        {
            Material[] materialList = new Material[6];
            XElement TypeIndex = blockType.Element("SpriteIndex");
            IEnumerable<XElement> IndexSettment = TypeIndex.Elements();


            foreach (XElement Index in IndexSettment)
            {
                switch (Index.Name.ToString())
                {
                    case "Bottom":
                        Material newMaterialBottom = new Material(Shader.Find("Standard"));
                        Debug.Log(TypeIndex.ToString() + ":" + Index.Name.ToString() + " is Setting" + Index.Value);
                        newMaterialBottom.SetTexture("_MainTex", TextureList[int.Parse(Index.Value)]);
                        materialList[0] = newMaterialBottom;
                        break;
                    case "Top":
                        Material newMaterialTop = new Material(Shader.Find("Standard"));
                        Debug.Log(TypeIndex.ToString() + ":" + Index.Name.ToString() + " is Setting" + Index.Value);
                        newMaterialTop.SetTexture("_MainTex", TextureList[int.Parse(Index.Value)]);
                        materialList[1] = newMaterialTop;
                        break;
                    case "Left":
                        Material newMaterialLeft = new Material(Shader.Find("Standard"));
                        Debug.Log(TypeIndex.ToString() + ":" + Index.Name.ToString() + " is Setting" + Index.Value);
                        newMaterialLeft.SetTexture("_MainTex", TextureList[int.Parse(Index.Value)]);
                        materialList[2] = newMaterialLeft;
                        break;
                    case "Right":
                        Material newMaterialRight = new Material(Shader.Find("Standard"));
                        Debug.Log(TypeIndex.ToString() + ":" + Index.Name.ToString() + " is Setting" + Index.Value);
                        newMaterialRight.SetTexture("_MainTex", TextureList[int.Parse(Index.Value)]);
                        materialList[3] = newMaterialRight;
                        break;
                    case "Back":
                        Material newMaterialBack = new Material(Shader.Find("Standard"));
                        Debug.Log(TypeIndex.ToString() + ":" + Index.Name.ToString() + " is Setting" + Index.Value);
                        newMaterialBack.SetTexture("_MainTex", TextureList[int.Parse(Index.Value)]);
                        materialList[4] = newMaterialBack;
                        break;
                    case "Front":
                        Material newMaterialFront = new Material(Shader.Find("Standard"));
                        Debug.Log(TypeIndex.ToString() + ":" + Index.Name.ToString() + " is Setting" + Index.Value);
                        newMaterialFront.SetTexture("_MainTex", TextureList[int.Parse(Index.Value)]);
                        materialList[5] = newMaterialFront;
                        break;
                }
            }
            return materialList;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return null;
        }
    }

    private void PlayerInitialize()
    {
        Transform playerTransform = player.transform;

        GameObject gameObject = Instantiate(player,new Vector3(8f,Chunk.chunkHight,8f), Quaternion.identity);
    }

    private void Awake()
    {
        try
        {
            Debug.Log(NewWorldLoader.Instance);
            blockSprites = Resources.LoadAll<Sprite>("201301031509_terrain");
            blockTextures = new Texture2D[blockSprites.Length];
            int i = 0;
            foreach (Sprite sprite in blockSprites)
            {
                Debug.Log($"Name: {sprite.name}, Type: {sprite.GetType()}");
                blockTextures[i] = SpriteToTexture2D(sprite);
                i++;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        LoadBlockResourse(blockFilePath, blockTextures);
    }
    // Start is called before the first frame update
    void Start()
    {
        MainThreadDispatcher.InitializeMainThreadDispatcher();

        mapCreator = new MapCreator();
        NoizeMap.Instance.SetNoizeMapImage();
        PlayerInitialize();
        // player.transform.position = new Vector3(8f, MapLoader.Instance.chunkNeedToLoad[(MapLoader._mapShowSize - 1) /2, (MapLoader._mapShowSize - 1) / 2].chunkSurface[8,8,0]+2, 8f); //从新确定玩家位置，将其设置到地表，好像有些bug
        MapLoader.Instance.UpdateMapAsync().Forget();
        Debug.Log($"mapSeed:{mapCreator.MapSeed}");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
