using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDestroyer:MonoBehaviour {

    private static BuildingDestroyer instance;

    private List<GameObject> plateList;
    private bool isOneAtGround;
    private GameObject mainPlate;
    // Use this for initialization
    private void Start()
    {
        instance = this;
    }
    private BuildingDestroyer() { }
	


    public static BuildingDestroyer Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new BuildingDestroyer();
            }
            return instance;
        }
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
            if (colAtGround.transform.gameObject.layer == LayerMask.NameToLayer("Default"))
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
            Destroy(building);           
            yield return new WaitForSeconds(0.5f);
        }
    }
}
