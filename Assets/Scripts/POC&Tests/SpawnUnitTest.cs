using Assets.Scripts.MapGen;
using Assets.Scripts.Networking;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts
{
    class SpawnUnitTest : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TestShips();
            }
        }
        //Test function! Kill me later!
        public void TestShips()
        {
            Debug.Log("Spawning it upppppp");
            var blueTile = HexTile.ParentMap.AllTiles[74, 67];
            var redTile = HexTile.ParentMap.AllTiles[75, 68];


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
