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

        [UsedImplicitly]
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
            MapGen = new MapGen.MapGen();

            MapGen.Launch(setting,() => {Debug.Log("Map Done!");});
        }
    }
}
