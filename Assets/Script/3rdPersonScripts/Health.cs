using Invector;
using UnityEngine;
using UnityEngine.UI;
using UnityTerraforming.GameAi;

[RequireComponent(typeof(vHealthController))]
public class Health : Photon.PunBehaviour
{
    private vHealthController healthController;

    // Use this for initialization
    private void Start()
    {
        healthController = gameObject.GetComponent<vHealthController>();
        healthController.onDead.AddListener(OnDeath);
    }

    public void OnDeath(GameObject gameobjct)
    {
        if (gameObject.layer == LayerMask.NameToLayer("BuildComponent"))
        {
            InteractionScript.Instance.CheckBuildings(gameObject);
        }
        else if (gameobjct.layer == LayerMask.NameToLayer("Enemy"))
        {
            gameobjct.GetComponent<BasicAi>().InstanceDied();
        }
        else
        {
            Destroy(gameObject);
        }
        if (gameObject.tag == "Player")
        {
            GameObject.Find("HealthSlider").GetComponent<Slider>().value = healthController.currentHealth;
        }
    }

    public void AddDamage(int damage)
    {
        healthController.TakeDamage(new vDamage(damage));
    }

    public void EnemyAddDamage(int damage)
    {
        photonView.RPC("RPCEnemyAddDamage", PhotonTargets.All, damage);
    }

    [PunRPC]
    private void RPCEnemyAddDamage(int damage)
    {
        AddDamage(damage);
    }
}