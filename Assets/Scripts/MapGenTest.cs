using UnityEngine;
using Assets.Scripts.MapGen;

public class MapGenTest : MonoBehaviour
{

    public bool UseSpiral;
    public int PlayerCount,XZone,YZone, AsteroidScore, IonScore, MixedScore;
    public MapGen MapGen;

    void Awake()
    {
        var setting = new MapSetting
        {
            AsteroidScore = AsteroidScore,
            IonScore = IonScore,
            MixedScore = MixedScore,
            PlayerCount = PlayerCount,
            Spiral = UseSpiral,
            XZones = XZone,
            YZones = YZone
        };
        MapGen = new MapGen();

        MapGen.SpawnTiles(setting);
        MapGen.MapTiles();
        MapGen.AssignRegions(setting);
        MapGen.AssignSectors();
        MapGen.AssignTiles();
    }
}
