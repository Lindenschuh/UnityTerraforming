using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour {

    public Canvas UICanvas;

    public GameObject wallIcon;
    public GameObject groundIcon;
    public GameObject rampIcon;


    private BuildMode buildScript;
    private GameObject woodCount;
    public Dictionary<BuildResources, int> resourceInfo;
    private ResourceControl resControl;
	// Use this for initialization
	void Start () {
        buildScript = GameObject.FindGameObjectWithTag("Player").GetComponent<BuildMode>();
        resControl = GetComponent<ResourceControl>();
        resourceInfo = new Dictionary<BuildResources, int>();
        resourceInfo.Add(BuildResources.Wood, 0);
        woodCount = GameObject.Find("WoodCount");
        woodCount.GetComponent<Text>().text = resControl.GetResourceInfo(BuildResources.Wood).ToString();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(buildScript.groundKey))
        {
            if(groundIcon.GetComponent<Image>().color == Color.green) groundIcon.GetComponent<Image>().color = Color.white;
            else groundIcon.GetComponent<Image>().color = Color.green;
            wallIcon.GetComponent<Image>().color = Color.white;
            rampIcon.GetComponent<Image>().color = Color.white;
        }

        if (Input.GetKeyUp(buildScript.wallKey))
        {
            if(wallIcon.GetComponent<Image>().color == Color.green) wallIcon.GetComponent<Image>().color = Color.white;
            else wallIcon.GetComponent<Image>().color = Color.green;
            groundIcon.GetComponent<Image>().color = Color.white;
            rampIcon.GetComponent<Image>().color = Color.white;
        }

        if (Input.GetKeyUp(buildScript.rampKey))
        {
            if(rampIcon.GetComponent<Image>().color == Color.green) rampIcon.GetComponent<Image>().color = Color.white;
            else rampIcon.GetComponent<Image>().color = Color.green;

            groundIcon.GetComponent<Image>().color = Color.white;
            wallIcon.GetComponent<Image>().color = Color.white;
        }
    }

    public void resetResourceCounter(BuildResources resourceType,int oldAmount, int newAmount)
    {
        switch (resourceType)
        {
            case BuildResources.Wood:
                if(oldAmount > newAmount)
                {
                    StartCoroutine(Wait(oldAmount, newAmount));
                }
                break;

        }
    }

    IEnumerator Wait(int oldAmount, int newAmount)
    {


        if (oldAmount > newAmount)
        {
            while (newAmount != oldAmount)
            {
                oldAmount--;
                yield return new WaitForSeconds(0.2f);
                woodCount.GetComponent<Text>().text = oldAmount.ToString();

            }
        }
    }
}
