using Assets.Scripts.MapGen;
using Assets.Scripts.Networking;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts
{
    class SpawnUnitTest : MonoBehaviour
    {
        public bool IsStart = true;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && IsStart)
            {
                TestShips();
                IsStart = false;
            }
        }
        //Test function! Kill me later!
        public void TestShips()
        {
            Debug.Log("Spawning it upppppp");
            var blueTile = GameState.Me.HexMap.AllTiles[74, 67];
            var redTile = GameState.Me.HexMap.AllTiles[75, 68];


            var testUnitMsg = new CreateUnitMsg
            {
                MaxHealth = 3,
                MaxMoves = 3,
                OnDeath = "",
                Scale = ShipScale.Small,
                Sprite = "Sprites/PH_BlueShip",
                TileId = blueTile.Id,
                Abilities = new[] { "" },
                MoveCost = new[] { 1, 2, 999, 999, 999, 999, 1, 999, 0, 2, 1, 1 }
            };

            Player.Me.Client.Send(Messages.CreateUnit, testUnitMsg);
        }
    }
}
