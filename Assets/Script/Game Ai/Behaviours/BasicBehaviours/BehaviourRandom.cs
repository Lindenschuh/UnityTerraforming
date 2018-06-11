using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class BehaviourRandom : Photon.PunBehaviour
    {
        public float CurrentRandom;

        public void GetRandomRange(float min, float max)
        {
            if (PhotonNetwork.isMasterClient)
            {
                photonView.RPC("RPCSendRandom", PhotonTargets.All, Random.Range(min, max));
            }
        }

        [PunRPC]
        public void RPCSendRandom(float randomValue)
        {
            CurrentRandom = randomValue;
        }
    }
}