using Assets.Scripts.MapGen;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts
{
    public class MapGenTest : MonoBehaviour
    {

        public bool UseSpiral;
        public int PlayerCount,XZone,YZone, AsteroidScore, IonScore, MixedScore, Richness;
        public MapGen.MapGen MapGen;
        public GameObject ThisBlueShip, ThisRedShip;
        public static GameObject BlueShip, RedShip;

        [UsedImplicitly]
        void Awake()
        {
            BlueShip = ThisBlueShip;
            RedShip = ThisRedShip;
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
            MapGen = new MapGen.MapGen();

            MapGen.Launch(setting,() => {Debug.Log("Map Done!");});
        }
    }
}
