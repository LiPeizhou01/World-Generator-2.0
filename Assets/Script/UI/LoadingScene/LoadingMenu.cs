using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{
    public AsyncOperation sampleSceneLoad;
    public Slider LoadingBar;
    public Button enterButton;
    public Button startButton;
    public Text LoadingMessage;
    float loadingTime;
    int textChangeCount;

    void StartLoad()
    {
        startButton.interactable = false;
        StartCoroutine(LoadScene("SampleScene"));
    }

    IEnumerator LoadScene(string loadSceneName)
    {
        loadingTime = 0.0f;
        sampleSceneLoad = SceneManager.LoadSceneAsync(loadSceneName,LoadSceneMode.Single);
        sampleSceneLoad.allowSceneActivation = false;
        while (!sampleSceneLoad.isDone)
        {
            // 伪缓冲
            LoadingBar.value = sampleSceneLoad.progress / 2.0f + loadingTime*0.1f;

            //加载文本更新
            if(enterButton.interactable == false)
            {
                if (LoadingBar.value == 1.0f)
                {
                    LoadingMessage.text = "请点击上方按钮";
                    enterButton.interactable = true;
                }
                else
                {
                    if (textChangeCount <= 10)
                    {
                        LoadingMessage.text = "Loading" + ".";
                    }
                    else if (textChangeCount <= 20 && textChangeCount > 10)
                    {
                        LoadingMessage.text = "Loading" + "..";
                    }
                    else if (textChangeCount > 20 && textChangeCount <= 30)
                    {
                        LoadingMessage.text = "Loading" + "...";
                    }
                    else
                    {
                        textChangeCount = 0;
                    }
                }
            }

            Debug.Log("Scene loaded, waiting for initialization to complete...");
            yield return null;
        }
    }

    void EnterNewWorld()
    {
        LoadingMessage.text = "正在实例化地块，请稍等";
        sampleSceneLoad.allowSceneActivation = true;
    }

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        enterButton.interactable = false;
        enterButton.onClick.AddListener(EnterNewWorld);
        LoadingBar.interactable = false;
        startButton.onClick.AddListener(StartLoad);
    }

    // Update is called once per frame
    void Update()
    {
        textChangeCount++;
        loadingTime += Time.deltaTime;

    }
}
