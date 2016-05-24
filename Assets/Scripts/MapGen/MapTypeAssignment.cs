using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MapGen
{
    public static class MapTypeAssignment
    {
        public static void AssignAllRegions(this List<HexRegion> regions, MapSetting setting)
        {
            //Create a map/array of all regions.
            var xMin = regions.Select(r => r.X).Min();
            var yMin = regions.Select(r => r.Y).Min();
            var xRange = regions.Select(r => r.X).Max() - xMin;
            var yRange = regions.Select(r => r.Y).Max() - yMin;
            var regionMap = new HexRegion[xRange + 2, yRange + 2];
            foreach (var region in regions)
            {
                region.X -= xMin;
                region.Y -= yMin;
                regionMap[region.X, region.Y] = region;
                region.Type = RegionType.Unassigned;
            }

            //For each player, find a region with no neighbouring players.
            while (PushNonAdjacentRegion(regions, regionMap, RegionType.SolarSystem, setting.PlayerCount)){}

            //Push one dead and one rich zone per 7 regions.
            var richRatio = regions.Count/7;

            for (var x = 0; x < richRatio; x++)
            {
                regions.PushRegionType(RegionType.Dead);
                regions.PushRegionType(RegionType.Riches);
            }

            //Fill in remaining slots with random choices, based off setting.


            foreach (var region in regions)
            {
                Color color;
                switch (region.Type)
                {
                    case RegionType.Dead:
                        color = Color.black;
                        break;
                    case RegionType.Riches:
                        color = Color.yellow;
                        break;
                    case RegionType.SolarSystem:
                        color = Color.green;
                        break;
                    default:
                        color = Color.white;
                        break;
                }
                foreach (var sector in region.ChildSectors)
                {
                    foreach (var tile in sector.ChildTiles)
                    {
                        tile.TintColor = color;
                    }
                }
            }
        }

        public static bool PushNonAdjacentRegion(this List<HexRegion> regions, HexRegion[,] map, RegionType typeToPlace,
            int countToPlace)
        {
            //So, this is an ugly, brute force way of working through the problem. It'll place things randomly
            //but track its progress. If it runs out of possibilities, it'll restart. #shittyRecursion.

            //By the way, this will totally break if it's mathematically impossible to fit so many regions in one place. Don't do that?
            for (var x = 0; x < countToPlace; x++)
            {
                var isSet = false;
                var blackList = new List<HexRegion>();

                while (!isSet)
                {
                    var whiteList = regions.Where(r => r.Type != typeToPlace && !blackList.Contains(r)).ToList();
                    if (whiteList.Count <= 0)
                    {
                        return true;
                    }
                    var index = Random.Range(0, whiteList.Count);
                    var possibleLocation = whiteList[index];
                    if (possibleLocation.NeighbourIsType(map, typeToPlace))
                    {
                        blackList.Add(possibleLocation);
                    }
                    else
                    {
                        possibleLocation.Type = typeToPlace;
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

            return neighbourList.Any(r => r.Type == type);
        }
    }
}