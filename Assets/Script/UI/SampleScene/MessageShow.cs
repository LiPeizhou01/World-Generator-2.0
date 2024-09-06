using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class MessageShow : MonoBehaviour
{
    public Text mapMessage;
    public Text mapSeed;
    public Text mapHeight;
    // Start is called before the first frame update
    void Start()
    {
        mapSeed.text = "地图种子:" + NewWorldLoader.Instance.mapCreator.MapSeed;
    }

    // Update is called once per frame
    void Update()
    {
        MapMessage.Instance.ChangeMapMessage(mapMessage);
        MapMessage.Instance.HeightMessage(mapHeight);
    }
}
