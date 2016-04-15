using UnityEngine;
using System.Collections.Generic;

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
