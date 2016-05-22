using UnityEngine;
using Assets.Scripts.MapGen;

public class MapGenTest : MonoBehaviour
{

    public bool UseSpiral;
    public int PlayerCount,xZone,yZone;
    public MapGen MapGen;

    void Awake()
    {
        var setting = new MapSetting();
        setting.PlayerCount = 1;

        MapGen = new MapGen
        {
            XZones = xZone,
            YZones = yZone
        };
        MapGen.Launch(setting);
    }
}
