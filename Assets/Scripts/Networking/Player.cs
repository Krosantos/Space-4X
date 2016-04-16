using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class Player : MonoBehaviour, ITurnable
    {
        public int Id;
        public Dictionary<ResourceType, int> Resources;
        public int Credits, CreditRate;
        public int Science, ScienceRate;
        public int Diplo, DiploRate;

        //List of blueprints
        //Dictionary of player<-->agreement struct



        public void OnTurn() { }

        public void CheatCoroutine(IEnumerator<WaitForSeconds> routineToRun)
        {
            StartCoroutine(routineToRun);
        }
    }
}
