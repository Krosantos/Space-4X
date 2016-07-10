using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float MaxSpeed;
    public float ScrollSpeed = 20f;
    public float SpeedRamp = 1.1f;
    public Camera Camera;

    public void Update()
    {
        var resetSpeed = true;
        Camera.fieldOfView += Input.mouseScrollDelta.y * -1.6f;
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
            ScrollSpeed = 20f;
        }

        ClampEverything();
    }

    void ClampEverything()
    {
        if (ScrollSpeed >= MaxSpeed) ScrollSpeed = MaxSpeed;
        if (Camera.fieldOfView <= 8f) Camera.fieldOfView = 8f;
        if (Camera.fieldOfView >= 40f) Camera.fieldOfView = 40f;
    }
}
