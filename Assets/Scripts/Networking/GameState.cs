using System.Collections.Generic;
using Assets.Scripts.MapGen;

namespace Assets.Scripts.Networking
{
    public class GameState
    {
        //This stores all the various things that make a up a snapshot of a game.
        public int MaxPlayers;
        public MapLoader MapLoader;
        public Dictionary<int, Unit> AllUnits;
        public Dictionary<int, HexTile> AllTiles;
        public Dictionary<int, Resource> AllResources;
        public List<int> AllPlayers;
        public MapState MapState;
        public int[] SerializedMap;

        private static int _currentNumber;

        public GameState()
        {
            _currentNumber = 0;
            AllUnits = new Dictionary<int, Unit>();
            AllTiles = new Dictionary<int, HexTile>();
            AllResources = new Dictionary<int, Resource>();
            AllPlayers = new List<int>();
            MapState = MapState.None;
            MapLoader = new MapLoader();
        }

        public static int GenerateId()
        {
            _currentNumber++;
            return _currentNumber;
        }
    }
}
