using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMode : MonoBehaviour {

    public Transform normalSquare;
    public Transform transparentSquare;
    public Transform map;
    public float maxRay;
    public float gridSize;

    private Vector3 position;
    private bool isBuildModeActive;
    private Transform square;

    protected vThirdPersonCamera tpCamera;
    // Use this for initialization
    void Start () {
        isBuildModeActive = false;
        tpCamera = FindObjectOfType<vThirdPersonCamera>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            isBuildModeActive = true;


            square = Instantiate(transparentSquare, position, Quaternion.identity);
        }
        if (isBuildModeActive)
        {
            RaycastHit hit;
            Ray ray = new Ray(tpCamera.transform.position, tpCamera.transform.forward);
            if (Physics.Raycast(ray, out hit, maxRay))
            {
                Vector3 finalPos;
                if (hit.transform.tag.Equals("BuildSquare"))
                {
                    finalPos = AlignAndGenerateSquare(hit.point);
                }
                else
                {
                     finalPos = AlignAndGenerateSquare(hit.point);
                    Debug.Log("hit: " + hit.point);
                }
                if(finalPos != position)
                {

                    position = finalPos;
                    square.position = finalPos;
                }
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Instantiate(normalSquare, position, Quaternion.identity);
            }
        }      
	}

    Vector3 AlignAndGenerateSquare(Vector3 hitPoint)
    {

        float snapPointX = hitPoint.x + ((gridSize - hitPoint.x) % gridSize);
        float snapPointY = hitPoint.y + 0.05f;
        float snapPointZ = hitPoint.z + ((gridSize - hitPoint.z) % gridSize);
        float halfGridSize = gridSize / 2;

        if (!(snapPointX + halfGridSize >= hitPoint.x)) snapPointX += gridSize;
        else if (!(snapPointX - halfGridSize <= hitPoint.x)) snapPointX -= gridSize;

        if (!(snapPointZ + halfGridSize >= hitPoint.z)) snapPointZ += gridSize;
        else if (!(snapPointZ - halfGridSize <= hitPoint.z)) snapPointZ -= gridSize;

        Debug.Log("hitpoint x:" + hitPoint.x + " hitpoint z: " + hitPoint.z + " snappoint x:" + snapPointX + " snappoint z:" + snapPointZ + " mod x:" + (gridSize - hitPoint.x) % gridSize);
        Vector3 gridPosition = new Vector3(snapPointX, snapPointY, snapPointZ);
        Debug.Log("grid position:" + gridPosition);
        return gridPosition;
            
        
    }

    Vector3Int GetNearestCell(Vector3 hitPoint)
    {
        int xPoint = (int)hitPoint.x +2 ;
        int yPoint = (int)(hitPoint.y);
        int zPoint = (int)hitPoint.z;
        
        Vector3Int cellPoint = new Vector3Int(xPoint, yPoint, zPoint);

        return cellPoint;
    }
}
