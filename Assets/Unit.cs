using System;
using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour, ITurnable, ISelectable
{
    public HexTile CurrentTile;
    public int PlayerId;
    public int UnitId;
    public int MaxMoves, MovesLeft;
    public Delegate Ability01;
    public Delegate Ability02;
    public Delegate Ability03;
    public Delegate Ability04;
    public Delegate Ability05;
    public ShipScale Scale;
    public float MaxHealth, CurrentHealth;
    public List<HexTile> TilesInRange
    {
        get
        {
            return CurrentTile.GetMovableTiles(this);
        }
    }

    public Dictionary<Terrain, int> MoveCost
    {
        get
        {
            var result = new Dictionary<Terrain, int> {{Terrain.AsteroidS, 2}, {Terrain.Space, 1}};
            return result;
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

    public void Move(HexTile tile)
    {
        //This is for after a unit has been told to move by the server.
        CurrentTile.OccupyUnit = null;
        CurrentTile = tile;
        CurrentTile.OccupyUnit = this;
        MovesLeft -= MoveCost[tile.Terrain];
        //Animate stuff later when you don't suck.
        transform.position = tile.transform.position;        
    }

    public void OnTurn()
    {
        MovesLeft = MaxMoves;
    }

    public void OnSelect()
    {
        Debug.Log("I AM SELECTED.");
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
        UiSelect.Type = SelectType.Unit;
    }

    public void OnDeselect()
    {
        foreach (var tile in TilesInRange)
        {
            tile.TintColor = TileExtensions.ClearSelect;
        }
    }

    public void OnMouseDown()
    {
        UiSelect.Select(this);
    }
}
