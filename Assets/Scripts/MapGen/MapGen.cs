using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Networking;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.MapGen
{
    public class MapSetting
    {
        public bool Spiral;
        public int PlayerCount;
        public int RichnessScore;
        public int XZones, YZones;
        public int MixedScore, AsteroidScore, IonScore;
    }
    public class MapGen {
        
        public List<HexRegion> RegionList; 
        public static GameObject HexPrefab
        {
            get { return Resources.Load<GameObject>("PlaceholderHex"); }
        }
        public static GameObject Map;
        public NetworkClient Client;

        public void Launch(MapSetting setting, Action callback)
        {
            Singleton<MonoBehaviour>.Instance.StartCoroutine(GenerateMap(setting, callback));
        }

        IEnumerator<WaitForSeconds> GenerateMap(MapSetting setting, Action callback )
        {
            SpawnTiles(setting);
            Debug.Log("From the formless void, she wrought the expanse of space.");
            yield return new WaitForSeconds(0.05f);
            MapTiles();
            Debug.Log("And into this expanse she whispered Order.");
            yield return new WaitForSeconds(0.05f);
            AssignRegions(setting);
            Debug.Log("And each place was given a Name.");
            yield return new WaitForSeconds(0.05f);
            AssignSectors();
            Debug.Log("And from one thing sprung many things, all of them with Names of their own.");
            yield return new WaitForSeconds(0.05f);
            AssignTiles();
            Debug.Log("And to Name a thing is to shape it, and the world heaved into shape.");
            AssignResources(setting);
            Debug.Log("And into this shape, she Divided herself.");
            callback();
        }

        public void SpawnTiles (MapSetting setting) {
            Map = new GameObject {name = "Map"};
            GameState.Me.HexMap = new HexMap();
            var zoneCoords = new List<Vector2>();
            if (setting.Spiral)
            {
                
                zoneCoords.Add(new Vector2(1, 0));
                zoneCoords.Add(new Vector2(2, 0));
                zoneCoords.Add(new Vector2(0, 1));
                zoneCoords.Add(new Vector2(0, 2));
                zoneCoords.Add(new Vector2(1, 1));
                zoneCoords.Add(new Vector2(2, 1));
                zoneCoords.Add(new Vector2(1, 2));

                
                zoneCoords.Add(new Vector2(3, 1));
                zoneCoords.Add(new Vector2(4, 1));
                zoneCoords.Add(new Vector2(2, 2));
                zoneCoords.Add(new Vector2(2, 3));
                zoneCoords.Add(new Vector2(3, 2));
                zoneCoords.Add(new Vector2(4, 2));
                zoneCoords.Add(new Vector2(3, 3));
                
                zoneCoords.Add(new Vector2(0, 3));
                zoneCoords.Add(new Vector2(1, 3));
                zoneCoords.Add(new Vector2(-1, 4));
                zoneCoords.Add(new Vector2(-1, 5));
                zoneCoords.Add(new Vector2(0, 4));
                zoneCoords.Add(new Vector2(1, 4));
                zoneCoords.Add(new Vector2(0, 5));
                
            }
            else
            {
                for (var x = 0; x < setting.XZones; x++)
                {
                    for (var y = 0; y < setting.YZones; y++)
                    {
                        zoneCoords.Add(new Vector2(x, y));
                    }
                }
            }
            //Normalize the coordinates to be based off 0.
            var xMin = zoneCoords.Select(r => r.x).Min();
            var yMin = zoneCoords.Select(r => r.y).Min();

            //Actually spawn all our tiles (regions spawn sectors, sectors spawn tiles).
            RegionList = zoneCoords.Select(coord => new HexRegion((int) (coord.x - xMin), (int) (coord.y - yMin))).ToList();
        }

        public void MapTiles()
        {
            var list = GameState.Me.HexMap.TileList;
            var xMin = list.Select(t => t.X).Min();
            var yMin = list.Select(t => t.Y).Min();
            var xMax = list.Select(t => t.X).Max();
            var yMax = list.Select(t => t.Y).Max();

            GameState.Me.HexMap.AllTiles = new HexTile[xMax - xMin + 1, yMax - yMin + 1];
            //Set tiles to a 0 index, and stick them in the map.
            foreach (var tile in list)
            {
                tile.X -= xMin;
                tile.Y -= yMin;
                GameState.Me.HexMap.AllTiles[tile.X, tile.Y] = tile;
                if (!GameState.Me.AllTiles.ContainsKey(tile.Id)) GameState.Me.AllTiles.Add(tile.Id, tile);
                tile.ParentMap = GameState.Me.HexMap;
            }
        }

        public void AssignRegions(MapSetting setting)
        {
            RegionList.AssignAllRegions(setting);
        }

        public void AssignSectors()
        {
            RegionList.AssignSectorTypes();
        }

        public void AssignTiles()
        {
            RegionList.AssignAllTiles();
        }

        public void AssignResources(MapSetting setting)
        {
            RegionList.AssignResources(setting);
        }
    }
}
