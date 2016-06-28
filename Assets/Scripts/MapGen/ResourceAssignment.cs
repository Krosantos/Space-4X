using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MapGen
{
    public static class ResourceAssignment
    {
        //Why no polymers or hydrocarbons, you ask? Those get made in stations/on planets.

        static Dictionary<ResourceType, int> ResourceCost
        {
            get
            {
                var result = new Dictionary<ResourceType, int>
                {
                    {ResourceType.Water, 1},
                    {ResourceType.Aluminum, 1},
                    {ResourceType.HeavyGas, 4},
                    {ResourceType.NuclearMaterial, 4},
                    {ResourceType.DarkMatter, 10},
                    {ResourceType.NeutronMass, 10}
                };
                return result;
            }
        }

        static Dictionary<RegionType, int> MaxRichness
        {
            get
            {
                var result = new Dictionary<RegionType, int>();
                result.Add(RegionType.SolarSystem, 8);
                result.Add(RegionType.Dead, 3);
                result.Add(RegionType.Asteroids, 5);
                result.Add(RegionType.Mixed, 5);
                result.Add(RegionType.Sneaky, 5);
                result.Add(RegionType.Riches, 10);
                return result;
            }
        }

        public static void AssignResources(this List<HexRegion> inputRegions, MapSetting setting)
        {
            var totalRichness = setting.RichnessScore*setting.PlayerCount*35;
            if (totalRichness < 30) totalRichness = 30;
            var richnessIndex = totalRichness/4*3;
            Debug.Log("Structured resources account for "+ richnessIndex +" units out of "+totalRichness+".");

            
            //So, I should feed the solar systems first, and then go to town.


            // So, 44's a magic number (as is this whole loop) designed to get a baseline even spread of resource types.
            while (richnessIndex >= 44)
            {
                inputRegions.EvenlyDistributeResources(1, ResourceType.Water, RegionType.SolarSystem);
            }
        }

        public static int EvenlyDistributeResources(this List<HexRegion> inputRegions,int passes, ResourceType type, RegionType regionType)
        {
            var result = 0;

            // We want the lowest scoring region, ideally which hasn't passed the max.
            var targetRegion = inputRegions.Where(y => y.Type == regionType && y.RichnessScore < MaxRichness[y.Type]).OrderBy(y => y.RichnessScore).First() ??
                               inputRegions.Where(y => y.Type == regionType).OrderBy(y => y.RichnessScore).First();

            for (var x = 0; x < passes; x++)
            {
                var targetSector = targetRegion.ChildSectors.OrderBy(y => y.RichnessScore).First();
                var possibleTiles = targetSector.ChildTiles.Where(y => y.Terrain == Terrain.Space).ToList();
                if (possibleTiles.Count < 1) continue;
                possibleTiles[Random.Range(0, possibleTiles.Count)].Resource = new Resource(type);
                result =+ ResourceCost[type];
            }
            return result;
        }
    }
}
