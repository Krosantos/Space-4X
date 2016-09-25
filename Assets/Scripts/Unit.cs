using System;
using System.Collections.Generic;
using Assets.Scripts.MapGen;
using Assets.Scripts.Networking;
using Assets.Scripts.UI;
using Assets.Scripts.Utility;
using JetBrains.Annotations;
using UnityEngine;
using Terrain = Assets.Scripts.Utility.Terrain;

namespace Assets.Scripts
{
    public class Unit : MonoBehaviour, ITurnable, ISelectable
    {
        public HexTile CurrentTile;
        public int PlayerId;
        public int UnitId;
        public int MaxMoves, MovesLeft;
        public delegate void Ability01(MonoBehaviour target);
        public delegate void Ability02(MonoBehaviour target);
        public delegate void Ability03(MonoBehaviour target);
        public delegate void Ability04(MonoBehaviour target);
        public delegate void Ability05(MonoBehaviour target);
        public delegate void OnDeath();
        public ShipScale Scale;
        public int MaxHealth, CurrentHealth;
        public List<HexTile> TilesInRange
        {
            get
            {
                return CurrentTile.GetMovableTiles(this);
            }
        }

        [NotNull]
        public Dictionary<Terrain, int> MoveCost
        {
            get
            {
                var result = new Dictionary<Terrain, int> { {Terrain.AsteroidL, 99}, {Terrain.AsteroidX, 99}, {Terrain.AsteroidM, 99}, {Terrain.IonCloud, 2}, {Terrain.Deadspace, 3},{Terrain.AsteroidS, 2}, {Terrain.Space, 1}};
                return result;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                MoveCost = value;
            }
        }

        public Sprite Sprite
        {
            get
            {
                return gameObject.GetComponent<SpriteRenderer>().sprite;
            }
            set { gameObject.GetComponent<SpriteRenderer>().sprite = value; }
        }

        public static GameObject BaseUnit
        {
            get {return Resources.Load("BasUnit") as GameObject; }
        }

        public Unit() { }

        //This is for the no-sprite version that lives in server memory.
        public Unit(CreateUnitMsg input)
        {
            PlayerId = input.PlayerId;
            UnitId = input.UnitId;
            MaxHealth = input.MaxHealth;
            CurrentHealth = input.MaxHealth;
            Scale = input.Scale;

            //Do something with lookups to assign ability delegates.

            //Assign CurrentTile elsewhere (when you have a reference point to the GameState)

        }

        public void Move(HexTile tile, int totalMoveCost)
        {
            //This is for after a unit has been told to move by the server.
            CurrentTile.OccupyUnit = null;
            CurrentTile = tile;
            CurrentTile.OccupyUnit = this;
            MovesLeft -= totalMoveCost;
            //Animate stuff later when you don't suck.
            transform.position = tile.transform.position;        
        }

        public void OnTurn()
        {
            MovesLeft = MaxMoves;
        }

        public void OnSelect()
        {
            Debug.Log("Select Unit!");
            Debug.Log("Current Type: " + UiSelect.CurrentType + ". Prev Type: " + UiSelect.LastType + ".");
            foreach (var tile in TilesInRange)
            {
                if (tile.OccupyUnit != null)
                {
                    if (tile.OccupyUnit.PlayerId != PlayerId)
                    {
                        tile.TintColor = TileExtensions.RedSelect;
                    }
                }
                else tile.TintColor = TileExtensions.BlueSelect;
            }
        }

        public void OnDeselect()
        {
            Debug.Log("Deselect Unit!");
            Debug.Log("Current Type: " + UiSelect.CurrentType + ". Prev Type: " + UiSelect.LastType + ".");
            foreach (var tile in TilesInRange)
            {
                tile.TintColor = TileExtensions.ClearSelect;
            }
        }
    }
}
