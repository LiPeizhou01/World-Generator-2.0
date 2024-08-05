using Map_generator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using static UnityEditor.Progress;
using Directory = System.IO.Directory;

public class MapIO : MonoBehaviour
{
    public GameObject player;
    public GameObject block;
    public XDocument gameMap;
    public XElement mapRt;
    const int strongCorrelationZone = 25; // 懒着改了，想了一下如果加载3维的时候之间用正方体遍历逻辑方便很多
    const int weakCorrelationZone = 51;  // 区分强弱区域目的是为做一些预先计算，如判断判断区域是否含水
    const int strongCorrelationZoneHalf = 12;
    const int weakCorrelationZoneHalf = 25;
    float mapUpdataTimer;
    BasicMap newMap;
    void MapLoad(int playerPositionX, int playerPositionY, int playerPositionZ)
    {
        float standardXScale = block.transform.localScale.x;
        float standardYScale = block.transform.localScale.y;
        float standardZScale = block.transform.localScale.z;
        for (int i = playerPositionX - weakCorrelationZoneHalf; i <= playerPositionX + weakCorrelationZoneHalf; i++)
        {
            for (int j = playerPositionZ - weakCorrelationZoneHalf; j <= playerPositionZ + weakCorrelationZoneHalf; j++)
            {
                int pointPositionX = i;
                int pointPositionY = playerPositionY;
                int pointPositionZ = j;

                //在这里编辑弱相关区域逻辑

                if ( pointPositionX >= (playerPositionX - strongCorrelationZoneHalf) && pointPositionX <= (playerPositionX + strongCorrelationZoneHalf))
                {
                    if (pointPositionZ >= (playerPositionZ - strongCorrelationZoneHalf) && pointPositionZ <= (playerPositionZ + strongCorrelationZoneHalf))
                    {
                        var mapINFOX = (from mapINFO in mapRt.Elements() 
                                      where mapINFO.Value == pointPositionX.ToString()
                                      select mapINFO).SingleOrDefault();

                        if (mapINFOX == null)
                        {
                            GameObject newBlock = Instantiate(block, new Vector3(pointPositionX * standardXScale, 0, pointPositionZ * standardZScale), Quaternion.Euler(-90, 0, 0));
                            newBlock.tag = "soil_grass";
                            mapRt.Add(new XElement("X",pointPositionX.ToString(),
                                new XElement("Z", pointPositionZ.ToString(),
                                new XElement("Y", "0",
                                new XAttribute("Tag", "soil_grass")))));
                        }
                        else
                        {
                            var mapINFOXY = (from mapINFO in mapINFOX.Elements()
                                            where mapINFO.Value == pointPositionY.ToString()
                                            select mapINFO).SingleOrDefault();
                            if (mapINFOXY == null)
                            {
                                GameObject newBlock = Instantiate(block, new Vector3(pointPositionX * standardXScale, 0, pointPositionZ * standardZScale), Quaternion.Euler(-90, 0, 0));
                            }
                            else
                            {
                                GameObject newBlock = Instantiate(block, new Vector3(pointPositionX * standardXScale, 0, pointPositionZ * standardZScale), Quaternion.Euler(-90, 0, 0));
                                newBlock.tag = "soil_grass";
                                mapINFOX.Add(new XElement("Z",pointPositionZ.ToString(),
                                    new XElement("Y","0",
                                    new XAttribute("Tag", "soil_grass"))));
                            }
                        }
                    }
                }
            }
        }
    }
    string GetTotalMapFileCount()
    {
        if (Directory.Exists(Application.persistentDataPath))
        {
            Debug.Log("Input path has been found");
            int fileNum = Directory.GetFiles(Application.persistentDataPath).Length;
            return fileNum.ToString();
        }
        else
        {
            Debug.Log("directory is not exist!");
            return null;
        }
    }

    void BasicMapReader()
    {
        gameMap = XDocument.Load(newMap.mapSavePath);
        mapRt = gameMap.Root;
    }

    public async void MapLoader()
    {
        Vector3 playerPosition = player.transform.position;
        Debug.Log(player.transform.position);

        await Task.Run(() => MapLoad((int)playerPosition.x, (int)playerPosition.y, (int)playerPosition.y));
    }


    // Start is called before the first frame update
    void Start()
    {
        newMap = new BasicMap(GetTotalMapFileCount(),"AAFF01F2");
        BasicMapReader();
    }

    // Update is called once per frame
    void Update()
    {
        mapUpdataTimer += Time.deltaTime;

        if (mapUpdataTimer > 5.0f)
        {
            Debug.Log("update!");
            MapLoader();
            mapUpdataTimer = 0f;
        }
    }
}

public class BasicMap
{
    public BasicMap(string id, string seed)
    {
        this.id = id;
        this.seed = seed;
        mapSavePath = Path.Combine(Application.persistentDataPath, "mapdata_" + id + ".xml");
        Debug.Log(mapSavePath);

        BasicMapSetter();
    }

    public string id;
    public string seed;
    public string mapSavePath;
    XDocument map;

    void BasicMapSetter()
    {
        Debug.Log("Creating new mapfile");
        map = new XDocument(
        new XElement("maproot",
                new XAttribute("seed", seed)
            )
        );

        map.Save(mapSavePath);
        Debug.Log("new map has been saved");
    }
}
