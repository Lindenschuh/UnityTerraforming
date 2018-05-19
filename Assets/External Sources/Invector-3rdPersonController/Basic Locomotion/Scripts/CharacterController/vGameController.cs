using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace Invector
{
    using vCharacterController;
    public class vGameController : MonoBehaviour
    {
        [System.Serializable]
        public class OnRealoadGame : UnityEngine.Events.UnityEvent { }
        [Tooltip("Assign here the locomotion (empty transform) to spawn the Player")]
        public Transform spawnPoint;
        [Tooltip("Assign the Character Prefab to instantiate at the SpawnPoint, leave unassign to Restart the Scene")]
        public GameObject playerPrefab;
        [Tooltip("Time to wait until the scene restart or the player will be spawned again")]
        public float respawnTimer = 4f;
        [Tooltip("Check if you want to leave your dead body at the place you died")]
        public bool destroyBodyAfterDead;
        [HideInInspector]
        public OnRealoadGame OnReloadGame = new OnRealoadGame();
        [HideInInspector]
        public GameObject currentPlayer;
        private vThirdPersonController currentController;
        public static vGameController instance;
        private GameObject oldPlayer;
        public bool displayInfoInFadeText = true;

        void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                this.gameObject.name = gameObject.name + " Instance";
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }

            SceneManager.sceneLoaded += OnLevelFinishedLoading;
            if (displayInfoInFadeText && vHUDController.instance)
                vHUDController.instance.ShowText("Init Scene");
            FindPlayer();
        }

        public void OnCharacterDead(GameObject _gameObject)
        {
            oldPlayer = _gameObject;

            if (playerPrefab != null)
                StartCoroutine(Spawn());
            else
            {
                if (displayInfoInFadeText && vHUDController.instance)
                    vHUDController.instance.ShowText("Restarting Scene...");
                Invoke("ResetScene", respawnTimer);
            }
        }

        public void Spawn(Transform _spawnPoint)
        {
            if (playerPrefab != null)
            {
                if (oldPlayer != null && destroyBodyAfterDead)
                {
                    if (displayInfoInFadeText && vHUDController.instance)
                        vHUDController.instance.ShowText("Player destroyed: " + oldPlayer.name.Replace("(Clone)", "").Replace("Instance", ""));

                    Destroy(oldPlayer);
                }

                else if (oldPlayer != null)
                {
                    if (displayInfoInFadeText && vHUDController.instance)
                        vHUDController.instance.ShowText("Remove Player Components: " + oldPlayer.name.Replace("(Clone)", "").Replace("Instance", ""));
                    DestroyPlayerComponents(oldPlayer);
                }

                currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation) as GameObject;
                currentController = currentPlayer.GetComponent<vThirdPersonController>();
                currentController.onDead.AddListener(OnCharacterDead);
                OnReloadGame.Invoke();
                if (displayInfoInFadeText && vHUDController.instance)
                    vHUDController.instance.ShowText("Spawn player: " + currentPlayer.name.Replace("(Clone)", ""));

            }
        }

        public IEnumerator Spawn()
        {
            yield return new WaitForSeconds(respawnTimer);

            if (playerPrefab != null && spawnPoint != null)
            {
                if (oldPlayer != null && destroyBodyAfterDead)
                {
                    if (displayInfoInFadeText && vHUDController.instance)
                        vHUDController.instance.ShowText("Player destroyed: " + oldPlayer.name.Replace("(Clone)", "").Replace("Instance", ""));
                    Destroy(oldPlayer);
                }

                else
                {
                    if (displayInfoInFadeText && vHUDController.instance)
                        vHUDController.instance.ShowText("Remove Player Components: " + oldPlayer.name.Replace("(Clone)", "").Replace("Instance", ""));
                    DestroyPlayerComponents(oldPlayer);
                }

                yield return new WaitForEndOfFrame();

                currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation) as GameObject;
                currentController = currentPlayer.GetComponent<vThirdPersonController>();
                currentController.onDead.AddListener(OnCharacterDead);

                if (displayInfoInFadeText && vHUDController.instance)
                {
                    vHUDController.instance.ShowText("Respawn player: " + currentPlayer.name.Replace("(Clone)", ""));
                }
                OnReloadGame.Invoke();
            }
        }

        void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            if (currentController.currentHealth > 0)
            {
                if (displayInfoInFadeText && vHUDController.instance)
                    vHUDController.instance.ShowText("Load Scene: " + scene.name);
                return;
            }
            if (displayInfoInFadeText && vHUDController.instance)
                vHUDController.instance.ShowText("Reload Scene");
            OnReloadGame.Invoke();
            FindPlayer();

        }

        void FindPlayer()
        {
            var player = GameObject.FindObjectOfType<vThirdPersonController>();

            if (player)
            {
                currentPlayer = player.gameObject;
                currentController = player;
                player.onDead.AddListener(OnCharacterDead);
                if (displayInfoInFadeText && vHUDController.instance)
                    vHUDController.instance.ShowText("Found player: " + currentPlayer.name.Replace("(Clone)", "").Replace("Instance", ""));
            }
            else if (currentPlayer == null && playerPrefab != null && spawnPoint != null)
                Spawn(spawnPoint);
        }

        public void ResetScene()
        {
            if (oldPlayer)
                DestroyPlayerComponents(oldPlayer);


#if UNITY_5_3_OR_NEWER
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
#else
            	Application.LoadLevel(Application.loadedLevel);
#endif

            if (oldPlayer && destroyBodyAfterDead)
            {
                Destroy(oldPlayer);
            }
        }

        private void DestroyPlayerComponents(GameObject target)
        {
            if (!target) return;
            var comps = target.GetComponentsInChildren<MonoBehaviour>();
            for (int i = 0; i < comps.Length; i++)
            {
                Destroy(comps[i]);
            }
            var coll = target.GetComponent<Collider>();
            if (coll != null) Destroy(coll);
            var rigidbody = target.GetComponent<Rigidbody>();
            if (rigidbody != null) Destroy(rigidbody);
            var animator = target.GetComponent<Animator>();
            if (animator != null) Destroy(animator);
        }
    }
}