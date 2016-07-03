#region

using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

#endregion

namespace Assets.Scripts.Networking
{
    public class SpawnNetworkComponent : NetworkBehaviour
    {
        public Client Client;
        public GameObject HexPrefab;

        public bool IsStartUp = true;
        public bool UseSpiralPattern;

        [UsedImplicitly]
        private void Update()
        {
            if (IsStartUp)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    Debug.Log("Server is go.");
                    SetupServer();
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    Debug.Log("Client get.");
                    SetupClient();
                }
            }
        }

        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        private void OnGUI()
        {
            if (!IsStartUp) return;
            GUI.Label(new Rect(2, 10, 150, 100), "Press S for server");
            GUI.Label(new Rect(2, 50, 150, 100), "Press C for client");
        }

        private void SetupServer()
        {
            IsStartUp = false;
            gameObject.AddComponent<Server>();
        }

        private void SetupClient()
        {
            Client = new Client();
            Client.Awake();
            IsStartUp = false;
        }
    }
}