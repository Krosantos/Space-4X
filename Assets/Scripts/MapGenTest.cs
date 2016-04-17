using UnityEngine;
using Assets.Scripts.MapGen;

public class MapGenTest : MonoBehaviour
{

    public MapGen MapGen;

    void Awake()
    {
        MapGen = new MapGen
        {
            XZones = 1,
            YZones = 1
        };
        MapGen.Launch(false);
    }
}
