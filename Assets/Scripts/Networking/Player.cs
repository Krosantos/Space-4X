using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    public class Player : ITurnable
    {
        public static Player Me;
        public int Id;
        public NetworkClient Client;
        public Dictionary<ResourceType, int> Resources;
        public List<ITurnable> Turnables; 
        public int Credits, CreditRate;
        public int Science, ScienceRate;
        public int Diplo, DiploRate;
        
        //List of blueprints
        //Dictionary of player<-->agreement struct

        public Player()
        {
            Resources = new Dictionary<ResourceType, int>();
            Turnables = new List<ITurnable>();
        }

        public void RequestToMove(int unitId, int tileId)
        {
            Client.Send(Messages.MoveUnit, new MoveUnitMsg(unitId, tileId, 0));
        }

        public void EndTurn()
        {
            Client.Send(Messages.EndTurn, new EndTurnMsg());
        }

        public void OnTurn()
        {
            Credits += CreditRate;
            Science += ScienceRate;
            Diplo += DiploRate;
            foreach (var turnable in Turnables)
            {
                turnable.OnTurn();
            }
        }
    }
}
