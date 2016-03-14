using UnityEngine;
using System.Collections.Generic;

public class HexTile : MonoBehaviour, ISelectable {

    public static HexMap ParentMap;

    public HexSector ParentSector;

    public HexRegion ParentRegion
    {
        get { return ParentSector.ParentRegion; }
    }

    public static HexTile[,] TileMap
    {
        get { return ParentMap.allTiles; }
        set { ParentMap.allTiles = value; }   
    }
    //Neighbours are properties: makes calculation a bit more CPU intensive, saves a ton of memory.
    public HexTile UL
    {
        get
        {
            if (x%2 == 0 && x > 0)
            {
                if (TileMap[x - 1, y] != null) return TileMap[x - 1, y];
            }
            else if (x > 0 && y+1 < TileMap.GetLength(1))
            {
                if (TileMap[x - 1, y + 1] != null) return TileMap[x - 1, y + 1];
            }
            return null;
        }
    }
    public HexTile UU
    {
        get
        {
            if (y +1< TileMap.GetLength(1))
            {
                if (TileMap[x, y + 1] != null) return TileMap[x, y + 1];
            }
            return null;
        }
    }
    public HexTile UR
    {
        get
        {
            if (x%2 == 0 && x + 1 < TileMap.GetLength(0))
            {
                if (TileMap[x + 1, y] != null) return TileMap[x + 1, y];
            }
            else if (x % 2 != 0 && x + 1< TileMap.GetLength(0) && y+1 < TileMap.GetLength(1))
            {
                if (TileMap[x + 1, y+1] != null) return TileMap[x + 1, y+1];
            }
            return null;
        }
    }
    public HexTile DL
    {
        get
        {
            if (x%2 == 0 && x > 0 && y > 0)
            {
                if (TileMap[x - 1, y - 1] != null) return TileMap[x - 1, y - 1];
            }
            else if (x % 2 != 0 && x > 0)
            {
                if (TileMap[x - 1, y] != null) return TileMap[x - 1, y];
            }
            return null;
        }
    }
    public HexTile DD
    {
        get
        {
            if (y>0)
            {
                if (TileMap[x, y - 1] != null) return TileMap[x, y - 1];
            }
            return null;
        }
    }
    public HexTile DR
    {
        get
        {
            if (x%2 == 0 && x + 1 < TileMap.GetLength(0) && y>0)
            {
                if (TileMap[x + 1, y-1] != null) return TileMap[x + 1, y-1];
            }
            else if (x % 2 != 0 && x + 1 < TileMap.GetLength(0))
            {
                if (TileMap[x + 1, y] != null) return TileMap[x + 1, y];
            }
            return null;
        }
    }
    public Unit OccupyUnit;
    public Terrain Terrain;
    public int x, y,Id;
    public Resource Resource;
    public List<HexTile> Neighbours;
    public GameObject Tint;
    public Color Color {
        get { return gameObject.GetComponent<SpriteRenderer>().color; }
        set { gameObject.GetComponent<SpriteRenderer>().color = value; }
    }
    public Color TintColor
    {
        get { return Tint.GetComponent<SpriteRenderer>().color; }
        set { Tint.GetComponent<SpriteRenderer>().color = value; }
    }

    public void MapNeighbours()
    {
        var neighbours = new List<HexTile>();
        if (UL != null) neighbours.Add(UL);
        if (UU != null) neighbours.Add(UU);
        if (UR != null) neighbours.Add(UR);
        if (DL != null) neighbours.Add(DL);
        if (DD != null) neighbours.Add(DD);
        if (DR != null) neighbours.Add(DR);
        this.Neighbours = neighbours;
    }

    void Awake() {
        Neighbours = new List<HexTile>();
        Resource = new Resource(0,0);
    }

    public void OnMouseDown()
    {
        UiSelect.Select(this);
    }

    public void OnSelect()
    {
        Debug.Log("I am a tile!");
        if (UiSelect.Type == SelectType.Unit)
        {
            Debug.Log("Previously, a unit was selected!");
        }
        UiSelect.Type = SelectType.Tile;
    }

    public void OnDeselect()
    {
    }
}

public class HexMap {
    int xDim, yDim;
    public HexTile[,] allTiles;
    public List<HexTile> tileList;

    public HexMap() {
        tileList = new List<HexTile>();
    }

    public HexMap(int x, int y) {
        xDim = x;
        yDim = y;
        allTiles = new HexTile[x, y];
        tileList = new List<HexTile>();
    }

    public int[] SerializeMap()
    {
        var result = new int[2+tileList.Count*6];
        result[0] = xDim;
        result[1] = yDim;
        var index = 2;
        foreach (var tile in tileList)
        {
            result[index] = tile.x;
            result[index + 1] = tile.y;
            result[index + 2] = tile.Id;
            result[index + 3] = (int)tile.Terrain;
            result[index + 4] = (int)tile.Resource.Type;
            result[index + 5] = tile.Resource.Quantity;
            index += 6;
        }
        return result;
    }
}