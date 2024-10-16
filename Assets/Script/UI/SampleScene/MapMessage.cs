using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMessage : MonoBehaviour
{
    static private MapMessage _instance;
    private MapMessage() { }
    static public MapMessage Instance 
    { 
        get 
        { 
            if(_instance != null)
            {
                return _instance;
            }
            else
            {
                _instance = FindObjectOfType<MapMessage>();
                return _instance;
            }
        } 
    }
    public GameObject player;
    Vector3 PlayerWasIn;

    Vector3 PlayerPosition
    {
        get
        {
            return new Vector3((int)player.transform.position.x, (int)player.transform.position.y-1, (int)player.transform.position.z);
        }
    }

    // 更新地图地图信息
    public void ChangeMapMessage(Text mapMessage)
    {
        Debug.Log(PlayerPosition);
        if (OnEnterNewCell)
        {
            mapMessage.text = $"Player position(x,z,y):({PlayerPosition.x},{PlayerPosition.z},{PlayerPosition.y})\n" +
            $"density:{mapParameters.density}\ncontinents:{mapParameters.continents}\n" +
            $"erosion:{mapParameters.erosion}\npv:{mapParameters.pv}\n" +
            $"weirdness:{mapParameters.weirdness}\nT:{mapParameters.temperature}\n" +
                                $"H:{mapParameters.humidity}";
        }
    }

    //更新地图高度
    public void HeightMessage(Text heightMessage)
    {
        int height_c = MapCreator.GetHeightBasic(mapParameters.continents);
        int height_e = MapCreator.GetHeightErosion(mapParameters.erosion);
        heightMessage.text = "演算信息:\n" +
            $"大陆性影响等级:{height_c}\n" +
            $"侵蚀度影响等级:{height_e}\n" +
            $"pv值叠算影响等级:{height_c+(int)(height_e * mapParameters.pv)}\n";
    }


    // 用于设置地图上的玩家坐标
    public void PointerPositionSetter(RawImage Pointer, MapCreator.PointerType pointerType)
    {
        Pointer.transform.localPosition = NewWorldLoader.Instance.mapCreator.GetNoizeMapPosition(PlayerPosition,pointerType);
    }

    public MapParameters mapParameters
    {
        get
        {
            return NewWorldLoader.Instance.mapCreator.GetMapParameters(PlayerPosition);
        }
    }

    public bool OnEnterNewCell
    {
        get
        {
            if(PlayerPosition == PlayerWasIn)
            {
                return false;
            }
            else
            {
                PlayerWasIn = PlayerPosition;
                return true; 
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayerWasIn = PlayerPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
