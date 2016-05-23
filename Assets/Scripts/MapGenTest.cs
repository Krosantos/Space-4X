using UnityEngine;
using Assets.Scripts.MapGen;

public class MapGenTest : MonoBehaviour
{

    public bool UseSpiral;
    public int PlayerCount,XZone,YZone;
    public MapGen MapGen;

    void Awake()
    {
        var setting = new MapSetting
        {
            PlayerCount = PlayerCount,
            Spiral = UseSpiral,
            XZones = XZone,
            YZones = YZone
        };
        MapGen = new MapGen();

        MapGen.SpawnTiles(setting);
        MapGen.MapTiles();
        MapGen.AssignTypes(setting);
    }
}
