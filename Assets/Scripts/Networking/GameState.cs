using System.Collections.Generic;

namespace Assets.Networking
{
    public class GameState
    {
        //This stores all the various things that make a up a snapshot of a game.
        public string GameName;
        public int GameId;

        public Dictionary<int, Unit> AllUnits;
        public Dictionary<int, HexTile> AllTiles;
        public Dictionary<int, Resource> AllResources;
        public List<int> AllPlayers;

        private int _currentNumber;

        public GameState()
        {
            _currentNumber = 0;
            AllUnits = new Dictionary<int, Unit>();
            AllTiles = new Dictionary<int, HexTile>();
            AllResources = new Dictionary<int, Resource>();
        }

        public int GenerateId()
        {
            _currentNumber++;
            return _currentNumber;
        }
    }
}
