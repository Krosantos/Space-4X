using System.Diagnostics;
using System.Linq;
using Assets.Scripts.MapGen;

namespace Assets.Scripts.Networking
{
    //Me is a place to keep some of the business logic the server performs when it does stuff like validate unit movement, or trade pact offering.
    static class ServerCalcLogic
    {
        public static bool UnitCanMove(Unit unit, HexTile targetTile, out int moveCost)
        {
            var pathToTile = unit.CurrentTile.AStarPath(targetTile, unit);
            moveCost = pathToTile.Sum(hex => unit.MoveCost[hex.Terrain]);

            return (moveCost >= unit.MovesLeft);
        }
    }
}
