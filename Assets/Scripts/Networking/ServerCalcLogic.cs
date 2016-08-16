using Assets.Scripts.MapGen;

namespace Assets.Scripts.Networking
{
    //This is a place to keep some of the business logic the server performs when it does stuff like validate unit movement, or trade pact offering.
    static class ServerCalcLogic
    {
        public static bool UnitCanMove(Unit unit, HexTile targetTile, out int moveCost)
        {
            moveCost = 0;
            var pathToTile = unit.CurrentTile.AStarPath(targetTile, unit);
            foreach (var hex in pathToTile)
            {
                moveCost += unit.MoveCost[hex.Terrain];
                if (moveCost > unit.MovesLeft) return false;
            }
            return true;
        }
    }
}
