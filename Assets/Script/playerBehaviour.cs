using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBehaviour : MonoBehaviour
{
    public GameObject Player;
    Vector3 PlayerPosition
    {
        get { return Player.transform.position; }
    }
    float TurnSpeed = 5.0f;
    float LookUpSpeed = 5.0f;
    float walkSpeed = 4.0f;
    float maxRotation = 80f;
    float minRotation = -80f;

    Vector3 moveSpeed = Vector3.zero;
    Rigidbody playerRb = null;

    void Walk()
    {
        moveSpeed.x = Input.GetAxis("Horizontal") * walkSpeed;
        moveSpeed.z = Input.GetAxis("Vertical") * walkSpeed;
        Player.transform.Translate(moveSpeed * Time.deltaTime, Space.Self);
    }
    void Jump()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerRb.AddRelativeForce(new Vector3(0,3,0),ForceMode.Impulse);
            Debug.Log("jumping");
        }
    }

    void MoveBehaviour()
    {
        Walk();
        Jump();
    }
    // Start is called before the first frame updateS
    void Start()
    {
        Camera mainCamera = Camera.main;
        mainCamera.transform.SetParent(Player.transform);


        if (Player.transform.Find("Head") != null)
        {
            Transform head = Player.transform.Find("Head");
            Debug.Log("set main camera!");
            mainCamera.transform.position = head.position + new Vector3(0f,0f,0.35f);
        }

        if (Player.GetComponent<Rigidbody>() != null)
        {
            Debug.Log("CharacterController of player has be found!");
            playerRb = Player.GetComponent<Rigidbody>();
            playerRb.freezeRotation = true;
        }
        else
        {
            Debug.Log("CharacterController of player has setting!");
            playerRb = Player.AddComponent<Rigidbody>();
            playerRb.freezeRotation = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Camera mainCamera = Camera.main;
        float h = TurnSpeed * Input.GetAxis("Mouse X");
        float v = LookUpSpeed * Input.GetAxis("Mouse Y");

        Player.transform.Rotate(new Vector3(0, h, 0), Space.World);
        Vector3 headEulerAngles = mainCamera.transform.eulerAngles;
        headEulerAngles.x += v;
        headEulerAngles.x = (headEulerAngles.x + 180f) % 360f - 180f;
        headEulerAngles.x = Mathf.Clamp(headEulerAngles.x, minRotation, maxRotation);
        mainCamera.transform.rotation = Quaternion.Euler(headEulerAngles);

        MoveBehaviour();
    }
}
