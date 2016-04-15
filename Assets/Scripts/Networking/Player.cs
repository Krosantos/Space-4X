using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class Player : MonoBehaviour, ITurnable
    {
        public int Id;
        public Dictionary<ResourceType, int> Resources;
        public int credits, creditRate;
        public int science, scienceRate;
        public int diplo, diploRate;

        //List of blueprints
        //Dictionary of player<-->agreement struct



        public void OnTurn() { }
        
    }
}
