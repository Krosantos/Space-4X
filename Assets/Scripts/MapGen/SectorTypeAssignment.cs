using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MapGen
{
    public static class SectorTypeAssignment
    {
        public static void AssignSectorTypes(this List<HexRegion> regions)
        {
            foreach (var region in regions)
            {
                foreach (var sector in region.ChildSectors)
                {
                    sector.Type = SectorType.Unassigned;
                }
                switch (region.Type)
                {
                    case RegionType.SolarSystem:
                        region.AssignSolarSystemSectors();
                        break;
                    case RegionType.Riches:
                        region.AssignRichesSectors();
                        break;
                    case RegionType.Dead:
                        region.AssignDeadSectors();
                        break;
                    case RegionType.Asteroids:
                        region.AssignAsteroidsSectors();
                        break;
                    case RegionType.Sneaky:
                        region.AssignSneakySectors();
                        break;
                    case RegionType.Mixed:
                        region.AssignMixedSectors();
                        break; 
                    default:
                        region.AssignMixedSectors();
                        break;
                }
            }
        }

        static void AssignSolarSystemSectors(this HexRegion region)
        {
            region.CenterSector.Type = SectorType.SystemCenter;
            region.PushSectorType(SectorType.Planet);
            region.PushSectorType(SectorType.Planet);
            region.PushSectorType(SectorType.Asteroids);
            region.PushSectorType(SectorType.Clouds);
            region.PushSectorType(SectorType.Blend);
            region.PushSectorType(SectorType.Blend);
        }

        static void AssignRichesSectors(this HexRegion region)
        {
            region.CenterSector.Type = Random.value >= 0.33f ? SectorType.Anomaly : SectorType.Asteroids;
            region.PushSectorType(Random.value >= 0.50f ? SectorType.Planet : SectorType.Clouds);
            region.PushSectorType(Random.value >= 0.45f ? SectorType.Deadspace : SectorType.Blend);
            region.PushSectorType(SectorType.Clouds);
            region.PushSectorType(SectorType.Blend);
            region.PushSectorType(SectorType.Asteroids);
            region.PushSectorType(SectorType.Blend);
        }

        static void AssignDeadSectors(this HexRegion region)
        {
            region.CenterSector.Type = Random.value >= 0.20f ? SectorType.Anomaly : SectorType.Deadspace;
            region.PushSectorType(Random.value >= 0.50f ? SectorType.Asteroids : SectorType.Clouds);
            region.PushSectorType(Random.value >= 0.40f ? SectorType.Clouds : SectorType.Deadspace);
            region.PushSectorType(SectorType.Deadspace);
            region.PushSectorType(SectorType.Deadspace);
            region.PushSectorType(SectorType.Deadspace);
            region.PushSectorType(SectorType.Deadspace);
        }

        static void AssignAsteroidsSectors(this HexRegion region)
        {
            region.PushSectorType(Random.value >= 0.20f ? SectorType.Planet : SectorType.Asteroids);
            region.PushSectorType(SectorType.Asteroids);
            region.PushSectorType(SectorType.Asteroids);
            region.PushSectorType(SectorType.Asteroids);
            region.PushSectorType(SectorType.Asteroids);
            region.PushSectorType(SectorType.Blend);
            region.PushSectorType(SectorType.Clouds);
        }

        static void AssignSneakySectors(this HexRegion region)
        {
            region.PushSectorType(Random.value >= 0.20f ? SectorType.Planet : SectorType.Clouds);
            region.PushSectorType(SectorType.Clouds);
            region.PushSectorType(SectorType.Clouds);
            region.PushSectorType(SectorType.Clouds);
            region.PushSectorType(SectorType.Clouds);
            region.PushSectorType(SectorType.Blend);
            region.PushSectorType(SectorType.Asteroids);
        }

        static void AssignMixedSectors(this HexRegion region)
        {
            region.PushSectorType(Random.value >= 0.20f ? SectorType.Planet : SectorType.Blend);
            region.PushSectorType(SectorType.Blend);
            region.PushSectorType(SectorType.Blend);
            region.PushSectorType(SectorType.Blend);
            region.PushSectorType(SectorType.Blend);
            region.PushSectorType(SectorType.Clouds);
            region.PushSectorType(SectorType.Asteroids);
        }

        public static void PushSectorType(this HexRegion region, SectorType type)
        {
            var possibleSectors = region.ChildSectors.Where(x => x.Type == SectorType.Unassigned).ToList();
            var index = Random.Range(0, possibleSectors.Count);
            possibleSectors[index].Type = type;
        }
    }
}
