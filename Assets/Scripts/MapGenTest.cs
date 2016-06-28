using UnityEngine;
using Assets.Scripts.MapGen;

public class MapGenTest : MonoBehaviour
{

    public bool UseSpiral;
    public int PlayerCount,XZone,YZone, AsteroidScore, IonScore, MixedScore, Richness;
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
            YZones = YZone,
            RichnessScore = Richness
        };
        MapGen = new MapGen();

        MapGen.Launch(setting);
        /*
        MapGen.SpawnTiles(setting);
        MapGen.MapTiles();
        MapGen.AssignRegions(setting);
        MapGen.AssignSectors();
        MapGen.AssignTiles();
        */
    }
}
