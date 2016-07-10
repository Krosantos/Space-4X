using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utility;
using UnityEngine;
using Terrain = Assets.Scripts.Utility.Terrain;

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
                switch (sector.Type)
                {
                    case SectorType.Anomaly:
                        sector.SeedTerrain(Terrain.Deadspace, 4,0.33f, 0.75f);
                        sector.SeedTerrain(Terrain.IonCloud, 3, 0.4f, 0.75f, canOverwrite: true);
                        sector.SeedAsteroids(3,0.75f,0.85f,0.2f,canOverwrite: true);
                        sector.CenterTile.SetTerrain(Terrain.Blackhole);
                        break;
                    case SectorType.Asteroids:
                        sector.SeedAsteroids(6,0.85f,0.95f,0.15f);
                        break;
                    case SectorType.Unassigned:
                    case SectorType.Blend:
                        sector.SeedAsteroids(3,0.45f,0.65f,0.1f,initialStrength:0.7f);
                        sector.SeedTerrain(Terrain.IonCloud, 3,0.45f);
                        break;
                    case SectorType.Clouds:
                        sector.SeedTerrain(Terrain.IonCloud, 4, 0.1f);
                        break;
                    case SectorType.Deadspace:
                        sector.SeedTerrain(Terrain.Deadspace, 4, 0.15f);
                        break;
                    case SectorType.Planet:
                        sector.SeedAsteroids(3, 0.45f, 0.65f, 0.1f, initialStrength: 0.7f);
                        sector.SeedTerrain(Terrain.IonCloud, 3, 0.33f,0.85f);
                        sector.PlaceTile(Terrain.Planet,true);
                        break;
                    case SectorType.SystemCenter:
                        sector.SeedAsteroids(3, 0.45f, 0.65f, 0.1f, initialStrength: 0.7f);
                        sector.SeedTerrain(Terrain.IonCloud, 3, 0.33f,0.85f);
                        sector.PlaceTile(Terrain.Planet,true);
                        sector.CenterTile.SetTerrain(Terrain.Star);
                    break;
                }
            }
        }

        //Generic function to spread terrain. You can use the combination of spread and carry chance to play around with non-contiguous results.
        // canBreakOut can be used to constrain spread to just this sector.
        private static void SeedTerrain(this HexSector sector, Terrain terrain, int passes, float convertChance, float spreadChance = 1f, bool canBreakOut = true, bool canOverwrite = false)
        {
            //Pick a random place to start.
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
                    tilesToAdd.AddRange(tile.Neighbours.Where(x=> !closedList.Contains(x) && Random.value <= spreadChance).ToList());

                    if (Random.value <= convertChance && (canBreakOut || tile.ParentSector == seedTile.ParentSector) && (canOverwrite || tile.Terrain == Terrain.Space)) tile.SetTerrain(terrain);
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
        private static void SeedAsteroids(this HexSector sector, int passes, float spreadChance, float decayRate, float maxShrinkChance, bool canBreakOut = true, bool canOverwrite = false, float initialStrength = 1f)
        {
            var emptyTiles = sector.ChildTiles.Where(x => x.Terrain == Terrain.Space).ToList();
            var seedTile = emptyTiles[Random.Range(0, emptyTiles.Count)];

            var openList = new List<HexTile> { seedTile };
            var closedList = new List<HexTile> { seedTile };
            var asteroidSize = new Dictionary<HexTile,float> {{seedTile,initialStrength}};

            var decayIndex = initialStrength;
            for (var i = 0; i < passes; i++)
            {
                var tilesToAdd = new List<HexTile>();
                var tilesToRemove = new List<HexTile>();

                foreach (var tile in openList)
                {
                    //If a given neighbour of the tiles in openList HASN'T already been considered, we add it.
                    //Then, it has a chance to get assigned the terrain, or to pass on its genes to its own neighbours.
                    tilesToAdd.AddRange(tile.Neighbours.Where(x => !closedList.Contains(x) && Random.value <= spreadChance).ToList());
                    if (!asteroidSize.ContainsKey(tile))
                        asteroidSize.Add(tile, decayIndex - Random.Range(0f, maxShrinkChance));
                    tilesToRemove.Add(tile);
                    closedList.Add(tile);
                    decayIndex *= decayRate;
                }

                openList.AddRange(tilesToAdd);
                foreach (var tile in tilesToRemove)
                {
                    openList.Remove(tile);
                }
            }

            //Now that everything's in the asteroidSize dictionary, go through it and convert the value to asteroid size.
            foreach (var pair in asteroidSize.Where(pair => (canOverwrite || pair.Key.Terrain == Terrain.Space) && (canBreakOut || pair.Key.ParentSector == seedTile.ParentSector)))
            {
                if (pair.Value >= 0.2f) pair.Key.SetTerrain(Terrain.AsteroidS);
                if (pair.Value >= 0.4f) pair.Key.SetTerrain(Terrain.AsteroidM);
                if (pair.Value >= 0.6f) pair.Key.SetTerrain(Terrain.AsteroidL);
                if (pair.Value >= 0.8f) pair.Key.SetTerrain(Terrain.AsteroidX);
            }
        }

        private static void PlaceTile(this HexSector sector, Terrain terrain, bool CanOverwrite = false)
        {
            var possibleTiles = CanOverwrite
                ? sector.ChildTiles
                : sector.ChildTiles.Where(x => x.Terrain == Terrain.Space).ToList();

            possibleTiles[Random.Range(0, possibleTiles.Count)].SetTerrain(terrain);
        }
    }
}
