using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMode : Photon.PunBehaviour {
    private enum buildMode
    {
            buildModeOff,
            buildModeGround,
            buildModeWall,
            buildModeRamp
    }
    public Transform normalSquare;
    public Transform transparentSquare;
    public Transform normalWall;
    public Transform transparentWall;
    public Transform normalRamp;
    public Transform transparentRamp;
    public float gridSize;
    public LayerMask buildLayer;
    private Vector3 position;
    private Transform square;



    protected vThirdPersonCamera tpCamera;
    private buildMode presentBuildMode;

    // Use this for initialization
    void Start () {
        tpCamera = FindObjectOfType<vThirdPersonCamera>();
        presentBuildMode = buildMode.buildModeOff;

        //adjust the scale of the ramp to fit in grid.
        float scaleX = normalWall.localScale.x;
        float scaleZ = normalWall.localScale.z;
        float scaleY = Mathf.Sqrt(normalWall.localScale.y * normalWall.localScale.y + normalWall.localScale.z * normalWall.localScale.z);
        normalRamp.localScale = new Vector3(scaleX, scaleY, scaleZ);
        transparentRamp.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }

    // Update is called once per frame
    void Update() {

        if (Input.GetKeyUp(KeyCode.Q))
        {
            InitBuilding(transparentSquare, buildMode.buildModeGround);
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            InitBuilding(transparentWall, buildMode.buildModeWall);

        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            InitBuilding(transparentRamp, buildMode.buildModeRamp);
        }

        if (presentBuildMode != buildMode.buildModeOff)
        {
            Vector3 pos = tpCamera.transform.position + tpCamera.transform.forward * (gridSize + 2);

            position = SnapToNearestGridcell(pos);
            square.position = position;

        }
    
            if (Input.GetKeyUp(KeyCode.Mouse0) && position != null)
            {
            BuildComponent();
            }
        }      

    void InitBuilding(Transform component, buildMode mode)
    {
        if (square != null) Destroy(square.gameObject);
        if (presentBuildMode == mode)
        {
            presentBuildMode = buildMode.buildModeOff;
            return;
        }else
        {
            presentBuildMode = mode;
            square = Instantiate(component, position, Quaternion.identity);
        }
    }
	
    /*
     *Calculate the position of the nearest cell
     * of the given grid and adjusted the position
     * for different building components 
     */
    Vector3 SnapToNearestGridcell(Vector3 hitPoint)
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

        //Get the nearest 90° rotation of y-Achsis to rotate the component into this position
        var rotation = tpCamera.transform.rotation.eulerAngles;
        rotation.x = 0;
        rotation.y = Mathf.Round(rotation.y / 90) * 90 - 90;
        rotation.z = 0;
        
        if (presentBuildMode == buildMode.buildModeWall)
        {
            
            if (Mathf.Abs(rotation.y) == 90 || Mathf.Abs(rotation.y) == 270 )
            {
                snapPointZ += halfGridSize;
            }else snapPointX += halfGridSize;

            snapPointY += halfGridSize;
        }

        if(presentBuildMode == buildMode.buildModeRamp)
        {
            if (Mathf.Abs(rotation.y) == 90 || Mathf.Abs(rotation.y) == 270)
            {
                rotation.z = -45;
            }
            else
            {
                rotation.z = -45;
            }
            snapPointY += halfGridSize;
        }
        square.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        Vector3 gridPosition = new Vector3(snapPointX, snapPointY, snapPointZ);

        return gridPosition;
            
        
    }

    /*
     * Check if component can be build and instantiate at the given point
     * */
    void BuildComponent()
    {
        //Check if there are colliders arount the component or if there is already build something
        bool canBuild = true;
        Collider[] boxColliders = Physics.OverlapBox(position, square.transform.localScale / 2, square.transform.rotation, buildLayer);
        if (boxColliders.Length == 0) canBuild = false;
        else
        {
            foreach (Collider collider in boxColliders)
            {
                if (collider.transform.position == position) canBuild = false;
            }
        }
        if (canBuild)
        {
            switch (presentBuildMode)
            {
                case buildMode.buildModeGround:
                    photonView.RPC("RPCBuildGround", PhotonTargets.All, position, square.rotation);
                    return;
                case buildMode.buildModeWall:
                    photonView.RPC("RPCBuildWall", PhotonTargets.All, position, square.rotation);
                    return;
                case buildMode.buildModeRamp:
                    photonView.RPC("RPCBuildRamp", PhotonTargets.All, position, square.rotation);
                    return;
                default:
                    return;

            }
        }
    }


    #region PunRPC

    [PunRPC]
    private void RPCBuildGround(Vector3 position, Quaternion rotation)
    {
        Instantiate(normalSquare, position, rotation);
    }

    [PunRPC]
    private void RPCBuildWall(Vector3 position, Quaternion rotation)
    {
        Instantiate(normalWall, position, rotation);
    }

    [PunRPC]
    private void RPCBuildRamp(Vector3 position, Quaternion rotation)
    {
        Instantiate(normalRamp, position, rotation);
    }

    #endregion

}
