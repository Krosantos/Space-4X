using Assets.Scripts.Networking;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class EndTurnButton : MonoBehaviour {

        public void EndTurn()
        {
            Player.Me.EndTurn();
        }
    }
}
