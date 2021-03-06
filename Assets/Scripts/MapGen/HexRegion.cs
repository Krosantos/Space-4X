﻿using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Assets.Scripts.MapGen
{
    public class HexRegion
    {
        public RegionType Type;
        public HexSector CenterSector;
        public int X, Y;
        public List<HexSector> ChildSectors;
        List<Vector2> ZoneCoords
        {
            get
            {
                var zoneCoords = new List<Vector2>
                {
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(2, 1),
                    new Vector2(0, 2),
                    new Vector2(1, 2),
                    new Vector2(2, 2),
                    new Vector2(1, 3)
                };
                return zoneCoords;
            }
        }

        public HexRegion(int x, int y)
        {
            X = x;
            Y = y;
            ChildSectors = new List<HexSector>();
            foreach (var coord in ZoneCoords)
            {
                var xCoord = (int)coord.x + (x * 3) + y;
                var yCoord = (int)coord.y + (y * 3) + x;
                if (x % 2 != 0 && y % 2 != 0)
                {
                    if ((int)coord.x % 2 == 0) yCoord--;
                }
                if (x % 2 != 0 || y % 2 != 0)
                {
                   if ((int)coord.x % 2 != 0) yCoord--;
                }
                yCoord -= x / 2;
                yCoord -= y / 2;

                var hexSector = new HexSector(xCoord, yCoord, this);
                ChildSectors.Add(hexSector);
                if (coord.x == 1 && coord.y == 2)
                {
                    CenterSector = hexSector;
                }
            }
        }
    }
}
