using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MapGen
{
    public static class MapTypeAssignment
    {
        public static void AssignAllRegions(this List<HexRegion> regions, MapSetting setting)
        {
            //For each player, find a region with no neighbouring players.
            while(PushStartRegions(regions, setting.PlayerCount)) { }
            foreach (var region in regions)
            {
                if (region.Type == RegionType.SolarSystem)
                {
                    Debug.Log("Region X: "+region.X+" Y: "+region.Y);
                    foreach (var sector in region.ChildSectors)
                    {
                        foreach (var tile in sector.ChildTiles)
                        {
                            tile.TintColor = Color.cyan;
                        }
                    }
                }
                
            }
            //For each type with hard counts (e.g., you want minimum 1 region of type "riches"), force an assignment.
            

            //For the rest, randomly choose.
        }

        public static bool PushStartRegions(this List<HexRegion> regions, int playerCount)
        {
            //Create a map/array of all regions.
            var xMin = regions.Select(r => r.X).Min();
            var yMin = regions.Select(r => r.Y).Min();
            var xRange = regions.Select(r => r.X).Max() - xMin;
            var yRange = regions.Select(r => r.Y).Max() - yMin;
            var regionMap = new HexRegion[xRange+2, yRange+2];
            foreach (var region in regions)
            {
                region.X -= xMin;
                region.Y -= yMin;
                regionMap[region.X, region.Y] = region;
                region.Type = RegionType.Unassigned;
            }
            
            //So, this is an ugly, brute force way of working through the problem. It'll place things randomly
            //but track its progress. If it runs out of possibilities, it'll restart. #shittyRecursion.

            //By the way, this will totally break if it's mathematically impossible to fit so many players in one place. Don't do that?
            for (var x = 0; x < playerCount; x++)
            {
                var isSet = false;
                var blackList = new List<HexRegion>();

                while (!isSet)
                {
                    var whiteList = regions.Where(r => r.Type != RegionType.SolarSystem && !blackList.Contains(r)).ToList();
                    if (whiteList.Count <= 0)
                    {
                        return true;
                    }
                    var index = Random.Range(0, whiteList.Count);
                    var possibleLocation = whiteList[index];
                    if (possibleLocation.NeighbourIsType(regionMap, RegionType.SolarSystem))
                    {
                        blackList.Add(possibleLocation);
                    }
                    else
                    {
                        possibleLocation.Type = RegionType.SolarSystem;
                        isSet = true;
                    }
                }
            }
            return false;


        }

        public static void PushRegionType(this List<HexRegion> regions, RegionType type)
        {
            var possibleRegions = regions.Where(x => x.Type == RegionType.Unassigned).ToList();
            var index = Random.Range(0, possibleRegions.Count);
            possibleRegions[index].Type = type;
        }

        public static bool NeighbourIsType(this HexRegion region, HexRegion[,] map, RegionType type)
        {
            var neighbourList = new List<HexRegion>();
            var x = region.X;
            var y = region.Y;
            Debug.Log("map is: "+map.GetLength(0)+", "+map.GetLength(1));
            Debug.Log("I am coord "+x+", "+y);

            //UL
            if (x > 0 && y < map.GetLength(1))
            {
                if (map[x - 1, y + 1] != null) neighbourList.Add(map[x - 1, y + 1]);
            }
            //LL
            if (x > 0)
            {
                if (map[x - 1, y] != null) neighbourList.Add(map[x - 1, y]);
            }
            //UR
            if (y < map.GetLength(1))
            {
                if (map[x, y + 1] != null) neighbourList.Add(map[x, y + 1]);
            }
            //DL
            if (y > 0)
            {
                if (map[x, y - 1] != null) neighbourList.Add(map[x, y - 1]);
            }
            //RR
            if (x < map.GetLength(1))
            {
                if (map[x + 1, y] != null) neighbourList.Add(map[x + 1, y]);
            }
            //DR
            if (x < map.GetLength(1) && y > 0)
            {
                if (map[x + 1, y - 1] != null) neighbourList.Add(map[x + 1, y - 1]);
            }

            return neighbourList.Any(r=>r.Type == type);
        }
    }
}
