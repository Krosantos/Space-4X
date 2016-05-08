using UnityEngine;
using Assets.Scripts.MapGen;

public class MapGenTest : MonoBehaviour
{

    public bool UseSpiral;
    public MapGen MapGen;

    void Awake()
    {
        MapGen = new MapGen
        {
            XZones = 6,
            YZones = 4
        };
        MapGen.Launch(UseSpiral);
    }
}
