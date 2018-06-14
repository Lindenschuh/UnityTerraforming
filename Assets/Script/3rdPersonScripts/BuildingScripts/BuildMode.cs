using Invector.vCamera;
using Invector.vCharacterController;
using Invector.vCharacterController.vActions;
using Invector.vItemManager;
using Invector.vShooter;
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
    public KeyCode groundKey;
    public KeyCode wallKey;
    public KeyCode rampKey;
    private Vector3 position;
    private Transform square;
    private bool canBuild;
    private UIControl uiController;
    private BuildResources selectedMaterial;
    private ResourceControl resourceControl;
    protected GameObject tpCamera;
    private buildMode presentBuildMode;
    private vShooterMeleeInput shooterInput;
    // Use this for initialization
    void Start () {
        //tpCamera = Camera.main.gameObject;
        presentBuildMode = buildMode.buildModeOff;
        resourceControl = GetComponent<ResourceControl>();
        //adjust the scale of the ramp to fit in grid.
        float scaleX = normalWall.localScale.x;
        float scaleZ = normalWall.localScale.z;
        float scaleY = Mathf.Sqrt(normalWall.localScale.y * normalWall.localScale.y + normalWall.localScale.z * normalWall.localScale.z);
        normalRamp.localScale = new Vector3(scaleX, scaleY, scaleZ);
        transparentRamp.localScale = new Vector3(scaleX, scaleY, scaleZ);
        uiController = GetComponentInParent<UIControl>();
        selectedMaterial = BuildResources.Wood;
        shooterInput = gameObject.GetComponent<vShooterMeleeInput>();
    }

    // Update is called once per frame
    void Update() {

        if (Input.GetKeyUp(groundKey))
        {
            InitBuilding(transparentSquare, buildMode.buildModeGround);            
        }

        if (Input.GetKeyUp(wallKey))
        {
            InitBuilding(transparentWall, buildMode.buildModeWall);
        }

        if (Input.GetKeyUp(rampKey))
        {
            InitBuilding(transparentRamp, buildMode.buildModeRamp);
        }

        if (presentBuildMode != buildMode.buildModeOff)
        {
            Vector3 pos = Camera.main.transform.position + Camera.main.transform.forward * (gridSize + 2);

            position = SnapToNearestGridcell(pos);

                
                square.position = position;
                //Check if there are colliders arount the component or if there is already build something
                canBuild = true;
                List<Vector3> scaleList = new List<Vector3>();
                if (square.transform.localScale.x >= 4)
                {
                    scaleList.Add(new Vector3(square.transform.localScale.x - gridSize / 2, square.transform.localScale.y, square.transform.localScale.z));
                }
                if (square.transform.localScale.y >= 4)
                {
                    scaleList.Add(new Vector3(square.transform.localScale.x, square.transform.localScale.y - gridSize / 2, square.transform.localScale.z));
                }
                if (square.transform.localScale.z >= 4)
                {
                    scaleList.Add(new Vector3(square.transform.localScale.x, square.transform.localScale.y, square.transform.localScale.z - gridSize / 2));
                }
                List<Collider[]> colliderList = new List<Collider[]>();
                int arrayLength = 0;
                foreach (Vector3 scale in scaleList)
                {
                    Collider[] colArray = Physics.OverlapBox(position, scale / 2, square.transform.rotation, buildLayer);
                    if (colArray.Length > 0)
                    {
                        colliderList.Add(colArray);
                        arrayLength += colArray.Length;
                    }
                }

                Collider[] boxColliders = new Collider[arrayLength];
                int copyInt = 0;
                foreach (Collider[] colArray in colliderList)
                {

                    colArray.CopyTo(boxColliders, copyInt);
                    copyInt += colArray.Length;
                }
                if (boxColliders.Length == 0) canBuild = false;
                else
                {
                    foreach (Collider collider in boxColliders)
                    {
                        if (collider.transform.position == position) canBuild = false;


                    }
                }

                if (!canBuild || resourceControl.GetResourceInfo(selectedMaterial) < 10)
                {
                    //square.GetComponent<Renderer>().material.color = Color.red;
                    square.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
                }
                else
                {
                    square.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                }

            }

            if (Input.GetKeyUp(KeyCode.Mouse0) && position != null && resourceControl.GetResourceInfo(selectedMaterial) >= 10)
            {
                BuildComponent();
            
            }
        }
              

    void InitBuilding(Transform component, buildMode mode)
    {
        if (square != null) Destroy(square.gameObject);
        if (presentBuildMode == mode)
        {
            shooterInput.lockMeleeInput = false;
            shooterInput.lockShooterInput = false;
            presentBuildMode = buildMode.buildModeOff;
            return;
        }else
        {
            shooterInput.lockMeleeInput = true;
            shooterInput.lockShooterInput = true;
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
        var rotation = Camera.main.transform.rotation.eulerAngles;
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

        if (canBuild)
        {
           
            switch (presentBuildMode)
            {
                
                case buildMode.buildModeGround:
                    photonView.RPC("RPCBuildGround", PhotonTargets.All, position, square.rotation);
                    break;
                case buildMode.buildModeWall:
                    photonView.RPC("RPCBuildWall", PhotonTargets.All, position, square.rotation);
                    break;
                case buildMode.buildModeRamp:
                    photonView.RPC("RPCBuildRamp", PhotonTargets.All, position, square.rotation);
                    break;
                default:
                    return;
                    
            }
            canBuild = false;
            resourceControl.UseResource(selectedMaterial, 10);
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
