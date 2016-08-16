﻿using System.Collections.Generic;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Networking
{
    public class Player : ITurnable
    {
        public int Id;
        public Client Client;
        public Dictionary<ResourceType, int> Resources;
        public List<ITurnable> Turnables; 
        public int Credits, CreditRate;
        public int Science, ScienceRate;
        public int Diplo, DiploRate;
        
        //List of blueprints
        //Dictionary of player<-->agreement struct

        public void RequestToMove(int unitId, int tileId)
        {
            
        }

        public void EndTurn()
        {
            Client.Send(Messages.EndTurn, new EndTurnMsg());
        }

        public void OnTurn()
        {
            foreach (var turnable in Turnables)
            {
                turnable.OnTurn();
            }
        }
    }
}
