using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class SpawnerEye : Photon.PunBehaviour
    {
        public int MaxTries;

        private float _currentXValue;
        private float _currentZValue;
        private int _terrainLayer;

        private void Start()
        {
            _terrainLayer = LayerMask.NameToLayer("Terrain");
            _currentXValue = 0;
            _currentZValue = 0;
        }

        public Vector3 GetRandomSpawnPoint(float range, int counter = 0)
        {
            if (counter >= MaxTries) return Vector3.zero;

            if (PhotonNetwork.isMasterClient)
            {
                photonView.RPC("RPCCreateRandomRangePair", PhotonTargets.All, Random.Range(-range, +range), Random.Range(-range, +range));
            }

            var wantedPoint = new Vector3(_currentXValue, 0, _currentZValue);

            RaycastHit hit;
            Ray ray = new Ray(transform.position, wantedPoint);

            if (Physics.Raycast(ray, out hit, range * 3))
            {
                if (hit.collider.gameObject.layer == _terrainLayer)
                {
                    return hit.point;
                }
            }

            return GetRandomSpawnPoint(range, counter + 1);
        }

        [PunRPC]
        public void RPCCreateRandomRangePair(float randomXValue, float randomZValue)
        {
            _currentXValue = randomXValue;
            _currentZValue = randomZValue;
        }
    }
}