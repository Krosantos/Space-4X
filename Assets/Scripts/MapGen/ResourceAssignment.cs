using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utility;
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

        //We'll loop through this to distribute resources in the right order/proportion.
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

        //This dictates the kind of sector a given resource can appear in.
        static Dictionary<ResourceType, List<SectorType>> ResourceSpawnTypes
        {
            get
            {
                var result = new Dictionary<ResourceType, List<SectorType>>
                {
                    {ResourceType.Aluminum,new List<SectorType> {SectorType.Asteroids, SectorType.Blend, SectorType.Planet}},
                    {ResourceType.Water, new List<SectorType> {SectorType.Clouds, SectorType.Blend, SectorType.Planet}},
                    {ResourceType.NuclearMaterial, new List<SectorType> {SectorType.Asteroids, SectorType.Blend}},
                    {ResourceType.HeavyGas, new List<SectorType> {SectorType.Clouds}},
                    {ResourceType.DarkMatter, new List<SectorType> {SectorType.Anomaly}},
                    {ResourceType.NeutronMass, new List<SectorType> {SectorType.Deadspace}}
                };
                return result;
            }
        } 

        //Main entry point into this entire mess.
        public static void AssignResources(this List<HexRegion> inputRegions, MapSetting setting)
        {
            var allSectors = new Dictionary<HexSector,int>();
            foreach (var sector in inputRegions.SelectMany(region => region.ChildSectors))
            {
                int richness;
                switch (sector.Type)
                {
                    case SectorType.Anomaly:
                        richness = -10;
                        break;
                    case SectorType.Deadspace:
                        richness = -10;
                        break;
                    case SectorType.SystemCenter:
                        richness = 0;
                        break;
                    case SectorType.Asteroids:
                        richness = -5;
                        break;
                    case SectorType.Clouds:
                        richness = -5;
                        break;
                    case SectorType.Blend:
                        richness = -3;
                        break;
                    case SectorType.Planet:
                        richness = -4;
                        break;
                    case SectorType.Unassigned:
                        richness = 0;
                        break;
                    default:
                        richness = 0;
                        break;
                }
                //Liiiiiittle bonus for rich regions.
                if (sector.ParentRegion.Type == RegionType.Riches && richness != 0) richness -= 2;
                allSectors.Add(sector,richness);
            }

            //First, put in a baseline quantity of resources in player solar systems.
            foreach (var region in inputRegions.Where(x=>x.Type == RegionType.SolarSystem))
            {
                region.DistributeSystemResources(ResourceType.Aluminum, allSectors);
                region.DistributeSystemResources(ResourceType.Water, allSectors);
                region.DistributeSystemResources(Random.value < 0.5f ? ResourceType.NuclearMaterial : ResourceType.HeavyGas, allSectors);
            }

            // Now, pachinko bonus resources all around the galaxy until you run out of points to assign.
            // We have a minimum totalRichness to ensure at least a scrap of dark matter and neutron mass appear.
            var totalRichness = setting.RichnessScore * setting.PlayerCount * 35;
            if (totalRichness < 30) totalRichness = 30;

            var orderIndex = 0;
            var order = ResourceOrder;
            while (totalRichness >= 0)
            {
                var typeToAssign = order[orderIndex];
                totalRichness -= EvenlyDistributeResource(allSectors, typeToAssign);
                orderIndex++;
            }
        }

        //Hardcoded method to make sure each starting area has a bare minimum of resources.
        static void DistributeSystemResources(this HexRegion region, ResourceType type, Dictionary<HexSector,int> scoreTracking )
        {

            var allSectors = region.ChildSectors.ToDictionary(sector => sector, sector => 0);
            var targetSector =
                allSectors.Keys.Where(x => ResourceSpawnTypes[type].Contains(x.Type))
                    .OrderBy(x => allSectors[x])
                    .First();

            //Find a nice empty tile to populate.
            var possibleTiles = targetSector.ChildTiles.Where(x => x.Resource.Type == ResourceType.Nothing).ToList();

            possibleTiles[Random.Range(0, possibleTiles.Count)].Resource = new Resource(type);

            scoreTracking[targetSector] += ResourceCost[type];
        }

        // Evenly parse out resources to the right type of region.
        static int EvenlyDistributeResource(Dictionary<HexSector, int> allSectors, ResourceType type)
        {
            //Find the most resource-poor sector of the appropriate type, and stick this resource in there somewhere.
            var targetSector =
                allSectors.Keys.Where(x => ResourceSpawnTypes[type].Contains(x.Type))
                    .OrderBy(x => allSectors[x])
                    .First();

            //Find a nice empty tile to populate.
            var possibleTiles = targetSector.ChildTiles.Where(x => x.Resource.Type == ResourceType.Nothing).ToList();

            possibleTiles[Random.Range(0,possibleTiles.Count)].Resource = new Resource(type);

            allSectors[targetSector] += ResourceCost[type];
            return ResourceCost[type];
        }

    }
}
