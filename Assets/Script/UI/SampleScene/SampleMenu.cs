using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SampleMenu : MonoBehaviour
{
    public Button quitButton;
    public Button reGenerateButton;

    void ReGenerate()
    {
        SceneManager.LoadScene(0);
    }

    void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        
    }
    // Start is called before the first frame update
    void Start()
    {
        quitButton.onClick.AddListener(Quit);
        reGenerateButton.onClick.AddListener(ReGenerate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
