using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Utility;

namespace Assets.Scripts
{
    public static class UnitExtensions
    {
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
