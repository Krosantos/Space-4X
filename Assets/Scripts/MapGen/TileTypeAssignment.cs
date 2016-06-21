using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.MapGen
{
    public static class TileTypeAssignment
    {
        public static void AssignAllTiles(this List<HexRegion> regions )
        {
            var sectors = new List<HexSector>();
            foreach (var region in regions)
            {
                sectors.AddRange(region.ChildSectors);
            }

            foreach (var sector in sectors)
            {
                Color debugColor;
                switch (sector.Type)
                {
                    case SectorType.Anomaly:
                        sector.CenterTile.Terrain = Terrain.BlackHole;
                        break;
                    case SectorType.Asteroids:

                        break;
                    case SectorType.Blend:

                        break;
                    case SectorType.Clouds:

                        break;
                    case SectorType.Deadspace:

                        break;
                    case SectorType.Planet:

                        break;
                    case SectorType.SystemCenter:

                    break;
                    default:

                        break;
                }

                foreach (var tile in sector.ChildTiles)
                {
                    //tile.TintColor = debugColor;
                }
            }
        }

        //Generic function to spread terrain. You can use the combination of spread and carry chance to play around with non-contiguous results.
        // canBreakOut can be used to constrain spread to just this sector.
        private static void SeedTerrain(this HexSector sector, Terrain terrain, int passes, float spreadChance, float carryChance = 1f, bool canBreakOut = true)
        {
            var emptyTiles = sector.ChildTiles.Where(x => x.Terrain == Terrain.Space).ToList();
            var seedTile = emptyTiles[Random.Range(0, emptyTiles.Count)];

            var openList = new List<HexTile> { seedTile };
            var closedList = new List<HexTile> { seedTile };
            seedTile.Terrain = terrain;
            for (var i = 0; i < passes; i++)
            {
                var tilesToAdd = new List<HexTile>();
                var tilesToRemove = new List<HexTile>();

                foreach (var tile in openList)
                {
                    //If a given neighbour of the tiles in openList HASN'T already been considered, we add it.
                    //Then, it has a chance to get assigned the terrain, or to pass on its genes to its own neighbours.
                    tilesToAdd.AddRange(tile.Neighbours.Where(x=> !closedList.Contains(x) && Random.value <= carryChance).ToList());

                    if (Random.value <= spreadChance && (canBreakOut || tile.ParentSector == seedTile.ParentSector)) tile.Terrain = terrain;
                    tilesToRemove.Add(tile);
                    closedList.Add(tile);
                }

                openList.AddRange(tilesToAdd);
                foreach (var tile in tilesToRemove)
                {
                    openList.Remove(tile);
                }

            }
            
        }

        //This works like the generic SeedTerrain function, but the farther you get from the seed, the smaller the asteroids get.
        private static void SeedAsteroids(this HexSector sector, int passes, float spreadChance, float carryChance = 1f, bool canBreakOut = true)
        {
            Debug.Log("Funkyfresh?");
        }

    }
}
