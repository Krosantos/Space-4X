using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float MaxSpeed;
    public float ScrollSpeed = 10f;
    public float SpeedRamp = 1.005f;
    public Camera Camera;

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Update()
    {
        var resetSpeed = true;
        Camera.fieldOfView += Input.mouseScrollDelta.y * -1.6f;
        if (Input.mouseScrollDelta.y > 0f)
        {
            var mousePos = Input.mousePosition;
            mousePos.x -= Screen.width / 2f;
            mousePos.y -= Screen.height / 2f;
            Camera.transform.Translate(mousePos * Time.deltaTime /2, Space.World);
        }

        if (Input.mousePosition.y >= Screen.height*0.95f)
        {
            Camera.transform.Translate(Vector3.up * Time.deltaTime * ScrollSpeed, Space.World);
            ScrollSpeed *= SpeedRamp;
            resetSpeed = false;
        }
        if (Input.mousePosition.y <= Screen.height * 0.05f)
        {
            Camera.transform.Translate(Vector3.down * Time.deltaTime * ScrollSpeed, Space.World);
            ScrollSpeed *= SpeedRamp;
            resetSpeed = false;
        }
        if (Input.mousePosition.x >= Screen.width * 0.95f)
        {
            Camera.transform.Translate(Vector3.right * Time.deltaTime * ScrollSpeed, Space.World);
            ScrollSpeed *= SpeedRamp;
            resetSpeed = false;
        }
        if (Input.mousePosition.x <= Screen.width*0.05f)
        {
            Camera.transform.Translate(Vector3.left*Time.deltaTime*ScrollSpeed, Space.World);
            ScrollSpeed *= SpeedRamp;
            resetSpeed = false;
        }
        if(resetSpeed)
        {
            ScrollSpeed = 10f;
        }

        ClampEverything();
    }

    void ClampEverything()
    {
        if (ScrollSpeed >= MaxSpeed) ScrollSpeed = MaxSpeed;
        if (Camera.fieldOfView <= 8f) Camera.fieldOfView = 8f;
        if (Camera.fieldOfView >= 60f) Camera.fieldOfView = 60f;
    }
}
