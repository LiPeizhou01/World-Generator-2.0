using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

public class NewWorldLoader : MonoBehaviour
{
    private NewWorldLoader(){}
    static private NewWorldLoader _Instance;
    static public NewWorldLoader Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new NewWorldLoader();
                return _Instance;
            }
            else
            {
                return _Instance;
            }

        }
    }
    public GameObject block;
    public GameObject player;

    public string blockFilePath = Path.Combine(Application.streamingAssetsPath, "BlockINFO.xml");
    Sprite[] blockSprites;
    public Texture2D[] blockTextures;
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
            }

            foreach (var blockType in blockTypes)
            {
                string tag = blockType.Element("Tag").Value;
                Debug.Log(tag);
                Material[] blockTypeMaterial = SetMaterial(TextureList, blockType);

                GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);
                if (gameObjects != null)
                {
                    Debug.Log("Find block tag object: " + gameObjects.Length);
                    foreach (GameObject gameObject in gameObjects)
                    {
                        Material[] objectMaterial = gameObject.GetComponent<MeshRenderer>().materials;

                        for (int j = 0; j < objectMaterial.Length; j++)
                        {
                            Debug.Log("Material numbers: " + objectMaterial.Length);
                            objectMaterial[j] = blockTypeMaterial[j];
                        }
                        gameObject.GetComponent<MeshRenderer>().materials = blockTypeMaterial;

                    }
                }
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

        GameObject gameObject = Instantiate(player,new Vector3(0f,10f,0f), Quaternion.identity);
    }
    public void GenerateWorld()
    {
        // 获取预制体的长宽高
        float standardXScale = block.transform.localScale.x;
        float standardYScale = block.transform.localScale.y;
        float standardZScale = block.transform.localScale.z;
    }

    private void Awake()
    {
        try
        {
            blockSprites = Resources.LoadAll<Sprite>("");
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
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayerInitialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
