using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MapGen
{
    public static class ResourceAssignment
    {
        //Why no polymers or hydrocarbons, you ask? Those get made in stations/on planets.

        public static Dictionary<ResourceType, int> ResourceCost
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
                var result = new Dictionary<RegionType, int>
                {
                    {RegionType.SolarSystem, 8},
                    {RegionType.Dead, 3},
                    {RegionType.Asteroids, 5},
                    {RegionType.Mixed, 5},
                    {RegionType.Sneaky, 5},
                    {RegionType.Riches, 10}
                };
                return result;
            }
        }

        static List<ResourceType> ResourceOrder
        {
            get
            {
                var result = new List<ResourceType>
                {
                    ResourceType.DarkMatter,
                    ResourceType.NeutronMass,
                    ResourceType.NuclearMaterial,
                    ResourceType.HeavyGas,
                    ResourceType.Water,
                    ResourceType.Aluminum,
                    ResourceType.Water,
                    ResourceType.HeavyGas,
                    ResourceType.Aluminum,
                    ResourceType.Water,
                    ResourceType.Aluminum,
                    ResourceType.NuclearMaterial,
                    ResourceType.Water,
                    ResourceType.Aluminum,
                    ResourceType.Water,
                    ResourceType.HeavyGas,
                    ResourceType.Aluminum,
                    ResourceType.Water,
                    ResourceType.Aluminum,
                    ResourceType.NuclearMaterial
                };
                return result;
            }
        }

        public static void AssignResources(this List<HexRegion> inputRegions, MapSetting setting)
        {
            //First, put in a baseline quantity of resources in player solar systems. This ignores richness score.
            for (var x = 0; x < setting.PlayerCount; x++)
            {
                var poorestRegion = inputRegions.Where(y => y.Type == RegionType.SolarSystem && y.RichnessScore < MaxRichness[y.Type]).OrderBy(y => y.RichnessScore).First() ??
                               inputRegions.Where(y => y.Type == RegionType.SolarSystem).OrderBy(y => y.RichnessScore).First();
                poorestRegion.DistributeToSpecifiedRegion(1,ResourceType.Water,SectorType.Blend);
                poorestRegion.DistributeToSpecifiedRegion(1, ResourceType.Water, SectorType.Planet);
                poorestRegion.DistributeToSpecifiedRegion(1, ResourceType.Aluminum, SectorType.Blend);
                poorestRegion.DistributeToSpecifiedRegion(1, ResourceType.Aluminum, SectorType.Planet);
                poorestRegion.DistributeToSpecifiedRegion(1,ResourceType.NuclearMaterial, SectorType.Asteroids);
                poorestRegion.DistributeToSpecifiedRegion(1,ResourceType.HeavyGas, SectorType.Clouds);
            }

            // Now, pachinko bonus resources all around the galaxy until you run out of points to assign.
            // We have a minimum totalRichness to ensure at least a scrap of dark matter and neutron mass appear.
            var totalRichness = setting.RichnessScore * setting.PlayerCount * 35;
            if (totalRichness < 30) totalRichness = 30;

            var orderIndex = 0;
            var order = ResourceOrder;
            while (totalRichness >= 0)
            {
                //TODO:make that list a dictionary of region type where each resource should go.
                //Maybe give it a preference for certain regions?
                orderIndex++;
                totalRichness--;
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
                var possibleTiles = targetSector.ChildTiles.Where(y => y.Terrain == Terrain.Space && y.Resource.Type == ResourceType.Nothing).ToList();
                if (possibleTiles.Count < 1) continue;
                possibleTiles[Random.Range(0, possibleTiles.Count)].Resource = new Resource(type);
                targetRegion.RichnessScore += ResourceCost[type];
                targetSector.RichnessScore += ResourceCost[type];
                result =+ ResourceCost[type];
            }
            return result;
        }

        public static void DistributeToSpecifiedRegion(this HexRegion region, int passes, ResourceType type, SectorType sectorPreference)
        {
            for (var x = 0; x < passes; x++)
            {
                var targetSector =
                    region.ChildSectors.Where(y => y.Type == sectorPreference).OrderBy(y => y.RichnessScore).First() ??
                    region.ChildSectors.OrderBy(y => y.RichnessScore).First();
                var possibleTiles = targetSector.ChildTiles.Where(y => y.Terrain == Terrain.Space && y.Resource.Type == ResourceType.Nothing).ToList();
                if (possibleTiles.Count < 1) continue;
                possibleTiles[Random.Range(0, possibleTiles.Count)].Resource = new Resource(type);
                region.RichnessScore += ResourceCost[type];
                targetSector.RichnessScore += ResourceCost[type];
            }
        }
    }
}
