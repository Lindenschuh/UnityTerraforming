using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Invector.vCharacterController
{
    public class v_SpriteHealth : MonoBehaviour
    {
        [Tooltip("UI to show on receive damage")]
        [SerializeField]
        protected GameObject healthBar;

        [Header("UI properties")]
        [SerializeField] protected Slider _healthSlider;
        [SerializeField] protected Slider _damageDelay;
        [SerializeField] protected float _smoothDamageDelay;
        [SerializeField] protected Text _damageCounter;
        [SerializeField] protected float _damageCounterTimer = 1.5f;
        [SerializeField] protected bool _showDamageType = true;

        private vHealthController healthControll;
        private bool inDelay;
        private float damage;
        private float currentSmoothDamage;

        void Start()
        {
            healthControll = transform.GetComponentInParent<vHealthController>();
            if (healthControll == null)
            {
                Debug.LogWarning("The character must have a ICharacter Interface");
                Destroy(this.gameObject);
            }
            healthControll.onReceiveDamage.AddListener(Damage);
            _healthSlider.maxValue = healthControll.maxHealth;
            _healthSlider.value = _healthSlider.maxValue;
            _damageDelay.maxValue = healthControll.maxHealth;
            _damageDelay.value = _healthSlider.maxValue;
            _damageCounter.text = string.Empty;
            if (healthBar) healthBar.SetActive(false);
        }

        void SpriteBehaviour()
        {
            if (Camera.main != null) transform.LookAt(Camera.main.transform.position, Vector3.up);

            if (healthControll == null || healthControll.currentHealth <= 0)
                Destroy(gameObject);

            _healthSlider.value = healthControll.currentHealth;
        }

        void Update()
        {
            if (!healthBar)
            {
                SpriteBehaviour();
            }
        }

        public void Damage(vDamage damage)
        {
            try
            {
                _healthSlider.value -= damage.damageValue;

                this.damage += damage.damageValue;
                _damageCounter.text = this.damage.ToString("00") + ((_showDamageType && !string.IsNullOrEmpty(damage.attackName)) ? (" : by " + damage.attackName) : "");
                if (!inDelay)
                    StartCoroutine(DamageDelay());
            }
            catch
            {
                Destroy(this);
            }
        }

        IEnumerator DamageDelay()
        {
            inDelay = true;
            if (healthBar) SpriteBehaviour();
            if (healthBar) healthBar.SetActive(true);

            while (_damageDelay.value > _healthSlider.value)
            {
                if (healthBar) SpriteBehaviour();
                _damageDelay.value -= _smoothDamageDelay;

                yield return null;
            }
            inDelay = false;
            

            yield return new WaitForSeconds(_damageCounterTimer);
            damage = 0;
            _damageCounter.text = string.Empty;
            if (healthBar) healthBar.SetActive(false);
        }
    }
}