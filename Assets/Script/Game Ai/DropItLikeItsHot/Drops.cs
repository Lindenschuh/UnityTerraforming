using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming
{
    [System.Serializable]
    public struct Drop
    {
        public GameObject Prefab;

        [Range(0f, 1f)]
        public float Chance;
    }

    public class Drops : MonoBehaviour
    {
        public List<Drop> drops;

        public void Drop()
        {
            foreach (Drop d in drops)
            {
                var random = Random.Range(0f, 1f);
                if (d.Chance >= random)
                {
                    Instantiate(d.Prefab, transform.position, Quaternion.identity);
                }
            }
        }
    }
}