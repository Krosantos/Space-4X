using System.Collections.Generic;
using Assets.Scripts.Networking;
using UnityEngine;
using Terrain = Assets.Scripts.Utility.Terrain;

namespace Assets.Scripts.Units
{
    public static class UnitExtensions
    {
        //The server uses this to create a unit without sprites and junk.
        public static Unit InstantiateUnitFromMsg(CreateUnitMsg msg)
        {
            var newUnitObject = Object.Instantiate(Unit.BaseUnit, Vector3.zero, Quaternion.identity);
            var newUnit = newUnitObject.GetComponent<Unit>();
            newUnit.PlayerId = msg.PlayerId;
            newUnit.UnitId = msg.UnitId;
            newUnit.MaxHealth = msg.MaxHealth;
            newUnit.CurrentHealth = msg.MaxHealth;
            newUnit.MaxMoves = msg.MaxMoves;
            newUnit.MovesLeft = msg.MaxMoves;
            newUnit.Scale = msg.Scale;
            newUnit.CreateMoveCostDictFromArray(msg.MoveCost);
            //It'd be weird to have too much reference to the tiles here, so we'll do all that jazz elsewhere.
            return newUnit;
        }

        public static void CreateMoveCostDictFromArray(this Unit unit, int[] input)
        {
            var dict = new Dictionary<Terrain, int>
            {
                {Terrain.Space, input[0]},
                {Terrain.AsteroidS, input[1]},
                {Terrain.AsteroidM, input[2]},
                {Terrain.AsteroidL, input[3]},
                {Terrain.AsteroidX, input[4]},
                {Terrain.Star, input[5]},
                {Terrain.Planet, input[6]},
                {Terrain.Blackhole, input[7]},
                {Terrain.Hyperspace, input[8]},
                {Terrain.Deadspace, input[9]},
                {Terrain.IonCloud, input[10]},
                {Terrain.Station, input[11]}
            };
            unit.MoveCost = dict;
        } 
    }
}
