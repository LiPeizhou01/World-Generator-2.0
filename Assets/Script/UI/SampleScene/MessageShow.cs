using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class MessageShow : MonoBehaviour
{
    public Text mapMessage;
    public Text mapSeed;
    public Text mapHeight;
    public Button showNoizeMapButton;
    public Text showNoizeMapButtonText;
    public GameObject noizeMapPanelObject;

    bool IsNoizeMapPanelAvailabel
    {
        get
        {
            return noizeMapPanelObject.activeSelf;
        }
    }

    void AvailableNoizeMap()
    {
        if (IsNoizeMapPanelAvailabel)
        {
            noizeMapPanelObject.SetActive(false);
            showNoizeMapButtonText.text = "启用噪声显示面板";
        }
        else
        {
            noizeMapPanelObject.SetActive(true);
            showNoizeMapButtonText.text = "停用噪声显示面板";
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        noizeMapPanelObject.SetActive(false);
        mapSeed.text = "地图种子:" + NewWorldLoader.Instance.mapCreator.MapSeed;
        showNoizeMapButton.onClick.AddListener(() => AvailableNoizeMap());
    }

    // Update is called once per frame
    void Update()
    {
        MapMessage.Instance.ChangeMapMessage(mapMessage);
        MapMessage.Instance.HeightMessage(mapHeight);
    }
}
