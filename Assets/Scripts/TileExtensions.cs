using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.MapGen;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts
{
    public static class TileExtensions {

        public static Color ClearSelect
        {
            get { return new Color(0f, 0f, 0f, 0f); }
        }

        public static Color RedSelect
        {
            get { return new Color(1f,.3f,.02f,.33f);}
        }

        public static Color BlueSelect
        {
            get { return new Color(.35f, .40f, .95f, .33f); }
        }

        public static bool ValidateUnitMovement(this HexTile tile, Unit unit)
        {
            return !((tile.OccupyUnit != null) || (unit.MovesLeft < unit.MoveCost[tile.Terrain]));
        }

        //Awwwww yiss, A* up in this bitch.
        public static List<HexTile> AStarPath(this HexTile from, HexTile to, Unit unit)
        {
            var openList = new List<HexTile>();
            var closedList = new List<HexTile>();
            var parentDict = new Dictionary<HexTile, HexTile>();
            var fScore = new Dictionary<HexTile, int>();
            var gScore = new Dictionary<HexTile, int>();
            var currentTile = from;

            gScore.Add(currentTile,0);
            fScore.Add(currentTile,GetHeuristic(from,to));
            openList.Add(currentTile);
            while (openList.Count > 0)
            {
                //That's right son! Since the last time I was here, I learned me how to Linq.
                currentTile = openList.OrderBy(x => fScore[x]).First();
                if (currentTile == to)
                {
                    return CreatePath(from, to, parentDict);
                }
                openList.Remove(currentTile);
                closedList.Add(currentTile);
                foreach (var hex in currentTile.Neighbours)
                {
                    var tempG = gScore[currentTile] + unit.MoveCost[hex.Terrain];
                    if (!openList.Contains(hex) && !closedList.Contains(hex))
                    {
                        openList.Add(hex);
                        parentDict.Add(hex,currentTile);
                        gScore.Add(hex,tempG);
                        fScore.Add(hex,gScore[hex]+GetHeuristic(hex,to));
                    }
                    if (tempG < gScore[hex])
                    {
                        //This is a better way to get to this tile, replace the entries in our dictionaries.
                        if (parentDict.ContainsKey(hex)) parentDict.Remove(hex);
                        parentDict.Add(hex,currentTile);
                        if (gScore.ContainsKey(hex)) gScore.Remove(hex);
                        gScore.Add(hex,tempG);
                        if (fScore.ContainsKey(hex)) fScore.Remove(hex);
                        fScore.Add(hex,gScore[hex]+GetHeuristic(hex,to));
                    }
                    //999 is a ~MAGIC NUMBER~. It's terrain cost shorthand for completely impassable.
                    if (hex.IsImpassable(unit))
                    {
                        openList.Remove(hex);
                        closedList.Add(hex);
                    }
                }
            }
            //If you made it this far, no route exists. Sorry, breh.
            return null;
        }

        //Helper function to calculate A* heuristic.
        private static int GetHeuristic(HexTile from, HexTile to)
        {
            var result = 0;

            var firstPos = Mathf.Abs(to.Y - from.Y) + 1;
            var secondPos = Mathf.Abs(to.X - from.X) + 1;
            var thirdPos = Mathf.Abs(to.X - to.Y)*-1 - (from.X - from.Y)*-1 + 1;

            //Because hexagons tesselate less smoothly than squares, we take the largest of 3 possible measures.
            if (firstPos >= result) result = firstPos;
            if (secondPos >= result) result = secondPos;
            if (thirdPos >= result) result = thirdPos;

            return result;
        }

        //Helper function to unwrap the list of parents we made in A*.
        private static List<HexTile> CreatePath(HexTile from, HexTile to, Dictionary<HexTile, HexTile> parents)
        {
            var result = new List<HexTile> {to};
            var currentTile = to;
            while (currentTile != from)
            {
                result.Add(parents[currentTile]);
                currentTile = parents[currentTile];
            }
            //We want this in order of to --> from, without the 'to' tile itself.
            result.Reverse();
            result.RemoveAt(0);
            return result;
        }

        //Helper function to see if a tile's impassable for any reason (bet that terrain, or enemy occupation).
        private static bool IsImpassable(this HexTile tile, Unit unit)
        {
            //999 is a ~MAGIC NUMBER~. It's terrain cost shorthand for completely impassable.
            if (unit.MoveCost[tile.Terrain] >= 999) return true;
            if (tile.OccupyUnit != null)
            {
                if (tile.OccupyUnit.PlayerId != unit.PlayerId) return true;
            }
            return false;
        }

        public static List<HexTile> GetMovableTiles(this HexTile tile, Unit unit)
        {
            var openList = new List<HexTile>();
            var tilesToOpen = new List<HexTile>();
            var closedList = new List<HexTile>();
            var result = new List<HexTile>();
            var costDictionary = new Dictionary<HexTile,int>();
            openList.Add(tile);
            costDictionary.Add(tile,0);
            while (openList.Count > 0)
            {
                //While there are still potential places to move, check 'em out.
                foreach (var openTile in openList)
                {
                    foreach (var neighbour in openTile.Neighbours)
                    {   //If it can be moved to, and isn't in the closed list, add it to the open list.
                        if (unit.MoveCost[neighbour.Terrain]+costDictionary[openTile] <= unit.MovesLeft && !closedList.Contains(neighbour) && !openList.Contains(neighbour) && !tilesToOpen.Contains(neighbour))
                        {
                            result.Add(neighbour);
                            tilesToOpen.Add(neighbour);
                            costDictionary.Add(neighbour, unit.MoveCost[neighbour.Terrain] + costDictionary[openTile]);
                        }

                        //If it's already on the list, check to see if we're coming from a more efficient path.
                        else if ((openList.Contains(neighbour) || tilesToOpen.Contains(neighbour)) &&
                                 unit.MoveCost[neighbour.Terrain] + costDictionary[openTile] < costDictionary[neighbour])
                        {
                            costDictionary[neighbour] = unit.MoveCost[neighbour.Terrain] + costDictionary[openTile];
                        }
                    }
                    //Since we've explored all of this tile's neighbours, add it to the closed list.
                    closedList.Add(openTile);
                }
                //Make the necessary adjustments to the open list.
                openList.AddRange(tilesToOpen);
                foreach (var oldTile in closedList)
                {
                    openList.Remove(oldTile);
                }
            }
            return result;
        }

    }
}
