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
            while (PushStartRegions(regions, setting.PlayerCount)) { }

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
            var regionMap = new HexRegion[xRange, yRange];
            foreach (var region in regions)
            {
                regionMap[region.X - xMin, region.Y - yMin] = region;
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
                        return false;
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

                        //For debugging!
                        foreach (var childSector in possibleLocation.ChildSectors)
                        {
                            foreach (var tile in childSector.ChildTiles)
                            {
                                tile.TintColor = Color.cyan;
                            }
                        }

                        isSet = true;
                    }
                }
            }
            return true;


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

            //Upper Left
            if (region.X % 2 == 0 && region.X > 0)
            {
                if (map[region.X - 1, region.Y] != null) neighbourList.Add(map[region.X - 1, region.Y]);
            }
            else if (region.X > 0 && region.Y + 1 < map.GetLength(1))
            {
                if (map[region.X - 1, region.Y + 1] != null) neighbourList.Add(map[region.X - 1, region.Y + 1]);
            }

            //Upper
            if (region.Y +1 < map.GetLength(1))
            {
                if (map[region.X, region.Y + 1] != null) neighbourList.Add(map[region.X, region.Y + 1]);
            }

            //Upper Right
            if (region.X % 2 == 0 && region.X +1 < map.GetLength(0))
            {
                if (map[region.X + 1, region.Y] != null) neighbourList.Add(map[region.X + 1, region.Y]);
            }
            else if (region.X % 2 != 0 && region.X +1 < map.GetLength(0) && region.Y + 1 < map.GetLength(1))
            {
                if (map[region.X + 1, region.Y + 1] != null) neighbourList.Add(map[region.X + 1, region.Y + 1]);
            }

            //Lower Left
            

            return neighbourList.Any(x=>x.Type == type);
        }
    }
}
