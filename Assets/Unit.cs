using System;
using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour, ITurnable
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

    public Dictionary<Terrain, int> MoveCost
    {
        get
        {
            var result = new Dictionary<Terrain, int> {{Terrain.Space, 1}};
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

    public void OnMouseDown()
    {
        Debug.Log("MOUSEDOWN");
        //CurrentTile.ColorSpriteByMovement(this,);
    }
}
