using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitCameraAsepect : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector2 aspect = new Vector2();

    // Start is called before the first frame update
    void Start()
    {
        var cam = GetComponent<Camera>();
        if (aspect.y != 0f)
        {
            cam.aspect = aspect.x / aspect.y;
            var rectH = Screen.width / cam.aspect / Screen.height;
            cam.rect = new Rect(0, (1 - rectH) / 2, 1, rectH);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
