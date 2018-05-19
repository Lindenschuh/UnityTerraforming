using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Invector.vCharacterController
{
    public class vThrowUI : MonoBehaviour
    {
        public vThrowObject throwManager;
        public Text maxThrowCount;
        public Text currentThrowCount;

        private void Start()
        {
            throwManager = FindObjectOfType<vThrowObject>();
            throwManager.onCollectObject.AddListener(UpdateCount);
            throwManager.onThrowObject.AddListener(UpdateCount);
            UpdateCount();
        }

        void UpdateCount()
        {
            currentThrowCount.text = throwManager.currentThrowObject.ToString();
            maxThrowCount.text = throwManager.maxThrowObjects.ToString();
        }
    }
}
