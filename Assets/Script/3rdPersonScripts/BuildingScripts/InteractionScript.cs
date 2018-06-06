using Invector.vCamera;
using Invector.vCharacterController;
using Invector.vCharacterController.vActions;
using Invector.vItemManager;
using Invector.vShooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionScript: Photon.PunBehaviour
{

    
    public GameObject woodResource;
    private static InteractionScript instance;

    private GameObject player;
    private List<GameObject> plateList;
    private bool isOneAtGround;
    private GameObject mainPlate;
    // Use this for initialization
    private void Start()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        GameObject priest = GameObject.Find("PriestNew");
        priest.GetComponent<vThirdPersonController>().enabled = true;
        priest.transform.GetChild(0).gameObject.SetActive(true);
        priest.GetComponent<vShooterMeleeInput>().enabled = true;
        priest.GetComponent<vShooterManager>().enabled = true;
        priest.GetComponent<vAmmoManager>().enabled = true;
        priest.GetComponent<vHeadTrack>().enabled = true;
        priest.GetComponent<vGenericAction>().enabled = true;
        priest.GetComponent<BuildMode>().enabled = true;
        priest.GetComponent<vItemManager>().enabled = true;
        priest.GetComponent<ResourceControl>().enabled = true;
        priest.GetComponent<UIControl>().enabled = true;
        priest.GetComponentInChildren<vThirdPersonCamera>().enabled = true;
        priest.GetComponentInChildren<Camera>().enabled = true;
    }
    private InteractionScript() { }
	


    public static InteractionScript Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new InteractionScript();
            }
            return instance;
        }
    }

    public void DropInventoryObject()
    {

    }
    public void CheckBuildings(GameObject plate)
    {
        mainPlate = plate;
        isOneAtGround = false;
        plateList = new List<GameObject>();
        List<GameObject> firstList = new List<GameObject>();
        
        List<Vector3> scaleList = GetScaleList(plate.transform);
        foreach (Vector3 scale in scaleList)
        {
            Collider[] colArray = Physics.OverlapBox(plate.transform.position, scale / 2, plate.transform.rotation);
            foreach (Collider col in colArray)
            {

                if (col.transform.gameObject != plate && col.transform.gameObject.layer == LayerMask.NameToLayer("BuildComponent") && !firstList.Contains(col.transform.gameObject))
                {
                    if (!CheckIfGrounded(col.transform))
                    {
                        firstList.Add(col.transform.gameObject);
                    }
                }
            }

        }
        Destroy(plate);
        foreach (GameObject buildPlate in firstList)
        {
            plateList = new List<GameObject>();
            CheckNeighbours(buildPlate);
            if (!isOneAtGround)
            {

                    StartCoroutine(DestroyBuilds());
                   

                
            }
            isOneAtGround = false;
        }   
    }

    private bool CheckIfGrounded(Transform objectToCheck)
    {
        bool isAtGround = false;
        Collider[] checkCollider = Physics.OverlapBox(objectToCheck.position, objectToCheck.localScale / 2, objectToCheck.rotation);
        foreach (Collider colAtGround in checkCollider)
        {
            if (colAtGround.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                isAtGround = true;
            }

        }
        return isAtGround;
    }

    private List<Vector3> GetScaleList(Transform plate)
    {
        List<Vector3> scaleList = new List<Vector3>();
        if (plate.localScale.x >= 4)
        {
            scaleList.Add(new Vector3(plate.localScale.x - 2, plate.localScale.y, plate.localScale.z));
        }
        if (plate.localScale.y >= 4)
        {
            scaleList.Add(new Vector3(plate.localScale.x, plate.localScale.y - 2, plate.localScale.z));
        }
        if (plate.localScale.z >= 4)
        {
            scaleList.Add(new Vector3(plate.localScale.x, plate.localScale.y, plate.localScale.z - 2));
        }
        return scaleList;
    }

    private void CheckNeighbours(GameObject plate)
    {
        if (!plateList.Contains(plate)) plateList.Add(plate);

        if (CheckIfGrounded(plate.transform))
        {
            isOneAtGround = true;
            return;
        }
        List<Vector3> scaleList = GetScaleList(plate.transform);
        foreach (Vector3 scale in scaleList)
        {
            Collider[] colArray = Physics.OverlapBox(plate.transform.position, scale / 2, plate.transform.rotation);
            foreach (Collider collider in colArray)
            {
                if (collider.gameObject != mainPlate && collider.gameObject.layer == LayerMask.NameToLayer("BuildComponent") && !plateList.Contains(collider.transform.gameObject))
                {
                    plateList.Add(collider.transform.gameObject);
                    CheckNeighbours(collider.gameObject);
                }
            }
        }

    }

    IEnumerator DestroyBuilds()
    {
        foreach (GameObject building in plateList)
        {
            photonView.RPC("RPCDestroyBuilding", PhotonTargets.All, building.transform.position, building.transform.localScale, building.transform.rotation);
            yield return new WaitForSeconds(0.5f);
        }
    }

    #region PunRPC

    [PunRPC]
    private void RPCDestroyBuilding(Vector3 position, Vector3 scale, Quaternion rotation)
    {
        Collider[] colArray = Physics.OverlapBox(position, new Vector3(scale.x/2, scale.y/2, scale.z/2) / 2, rotation);
        foreach(Collider collider in colArray)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("BuildComponent"))
            {
                Destroy(collider.gameObject);
            }
        }
    }

    #endregion
}
