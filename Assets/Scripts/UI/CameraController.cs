using System.Linq;
using Assets.Scripts.Networking;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class CameraController : MonoBehaviour
    {
        public float MaxSpeed;
        public float ScrollSpeed = 10f;
        public float SpeedRamp = 1.005f;
        public float MaxX, MaxY, MinX, MinY;
        public bool EdgesLocked = false;
        public Camera Camera;

        public void Awake()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        private static bool IsOutsideScreen()
        {
            return !(Input.mousePosition.x >= 0 && Input.mousePosition.y >= 0 && Input.mousePosition.x <= Screen.width &&
                     Input.mousePosition.y <= Screen.height);
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

            if (Input.mousePosition.y >= Screen.height*0.98f && !IsOutsideScreen())
            {
                Camera.transform.Translate(Vector3.up * Time.deltaTime * ScrollSpeed, Space.World);
                ScrollSpeed *= SpeedRamp;
                resetSpeed = false;
            }
            if (Input.mousePosition.y <= Screen.height * 0.02f && !IsOutsideScreen())
            {
                Camera.transform.Translate(Vector3.down * Time.deltaTime * ScrollSpeed, Space.World);
                ScrollSpeed *= SpeedRamp;
                resetSpeed = false;
            }
            if (Input.mousePosition.x >= Screen.width * 0.98f && !IsOutsideScreen())
            {
                Camera.transform.Translate(Vector3.right * Time.deltaTime * ScrollSpeed, Space.World);
                ScrollSpeed *= SpeedRamp;
                resetSpeed = false;
            }
            if (Input.mousePosition.x <= Screen.width* 0.02f && !IsOutsideScreen())
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
            if (!EdgesLocked && GameState.Me != null)
            {
                if (GameState.Me.HexMap != null)
                {
                    if (GameState.Me.HexMap.TileList.Count > 30)
                    {
                        MaxX = GameState.Me.HexMap.TileList.Max(x => x.transform.position.x);
                        MaxY = GameState.Me.HexMap.TileList.Max(x => x.transform.position.y);
                        MinX = GameState.Me.HexMap.TileList.Min(x => x.transform.position.x);
                        MinY = GameState.Me.HexMap.TileList.Min(x => x.transform.position.y);
                        EdgesLocked = true;
                    }
                }
            }
            if (ScrollSpeed >= MaxSpeed) ScrollSpeed = MaxSpeed;
            if (Camera.fieldOfView <= 8f) Camera.fieldOfView = 8f;
            if (Camera.fieldOfView >= 60f) Camera.fieldOfView = 60f;
            if (Camera.transform.position.x > MaxX) Camera.transform.position = new Vector3(MaxX, Camera.transform.position.y, Camera.transform.position.z);
            if (Camera.transform.position.y > MaxY) Camera.transform.position = new Vector3(Camera.transform.position.x, MaxY, Camera.transform.position.z);
            if (Camera.transform.position.x < MinX) Camera.transform.position = new Vector3(MinX, Camera.transform.position.y, Camera.transform.position.z);
            if (Camera.transform.position.y < MinY) Camera.transform.position = new Vector3(Camera.transform.position.x,MinY, Camera.transform.position.z);
        }
    }
}
