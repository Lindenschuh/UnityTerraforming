using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMode : MonoBehaviour {
    private enum buildMode
    {
            buildModeOff,
            buildModeGround,
            buildModeWall
    }
    public Transform normalSquare;
    public Transform transparentSquare;
    public Transform normalWall;
    public Transform transparentWall;
    public Transform map;
    public float maxRay;
    public float gridSize;

    private Vector3 position;
    private bool isBuildModeActive;
    private bool canBuild;
    private Transform square;



    protected vThirdPersonCamera tpCamera;
    private buildMode presentBuildMode;

    // Use this for initialization
    void Start () {
        isBuildModeActive = false;
        tpCamera = FindObjectOfType<vThirdPersonCamera>();
        presentBuildMode = buildMode.buildModeOff;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            isBuildModeActive = true;

            presentBuildMode = buildMode.buildModeGround;
            if(square != null)
            {
                Destroy(square.gameObject);
            }
            square = Instantiate(transparentSquare, position, Quaternion.identity);
        }
        if(Input.GetKeyUp(KeyCode.R))
        {
            isBuildModeActive = true;
            presentBuildMode = buildMode.buildModeWall;
            if (square != null)
            {
                Destroy(square.gameObject);
            }
            square = Instantiate(transparentWall, position, Quaternion.identity);
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
                switch (presentBuildMode)
                {
                    case buildMode.buildModeGround:
                        Instantiate(normalSquare, position, square.rotation);
                        return;
                    case buildMode.buildModeWall:
                        Instantiate(normalWall, position, square.rotation);
                        return;
                    default:
                        return;

                }
                
                    

                        
                
                
            }
        }      
	}

    Vector3 AlignAndGenerateSquare(Vector3 hitPoint)
    {

        float snapPointX = hitPoint.x + ((gridSize - hitPoint.x) % gridSize);
        float snapPointY = hitPoint.y + ((gridSize - hitPoint.y) % gridSize);
        float snapPointZ = hitPoint.z + ((gridSize - hitPoint.z) % gridSize);
        float halfGridSize = gridSize / 2;

        if (!(snapPointX + halfGridSize >= hitPoint.x)) snapPointX += gridSize;
        else if (!(snapPointX - halfGridSize <= hitPoint.x)) snapPointX -= gridSize;

        if (!(snapPointY + halfGridSize >= hitPoint.y)) snapPointY += gridSize;
        else if (!(snapPointY - halfGridSize <= hitPoint.y)) snapPointY -= gridSize;

        if (!(snapPointZ + halfGridSize >= hitPoint.z)) snapPointZ += gridSize;
        else if (!(snapPointZ - halfGridSize <= hitPoint.z)) snapPointZ -= gridSize;

        var rotation = tpCamera.transform.rotation.eulerAngles;
        rotation.x = 0;
        rotation.y = Mathf.Round(rotation.y / 90) * 90;
        rotation.z = 0;
        square.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        if (presentBuildMode == buildMode.buildModeWall)
        {
            snapPointX += halfGridSize;
            snapPointY += halfGridSize;
            //snapPointZ += halfGridSize;
        }
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
