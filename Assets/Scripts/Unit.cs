using System.Collections.Generic;
using Assets.Scripts.MapGen;
using Assets.Scripts.UI;
using Assets.Scripts.Utility;
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

        public Dictionary<Terrain, int> MoveCost;

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
            get {return Resources.Load("BaseUnit") as GameObject; }
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
            Debug.Log("There are "+TilesInRange.Count+" tiles in range.");
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
