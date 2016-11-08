using Assets.Scripts.Networking;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class EndTurnButton : MonoBehaviour {

        public void EndTurn()
        {
            Debug.Log("END");
            Player.Me.EndTurn();
        }

        public void Boop()
        {
            Debug.Log("boop");
        }
    }
}
