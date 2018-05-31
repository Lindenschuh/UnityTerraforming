using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UIComponentControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isEntered;
    private GameObject movableUIComp;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Name: " + eventData.pointerCurrentRaycast.gameObject.name);
        Debug.Log("Tag: " + eventData.pointerCurrentRaycast.gameObject.tag);
        Debug.Log("GameObject: " + eventData.pointerCurrentRaycast.gameObject);
        isEntered = true;
        movableUIComp = eventData.pointerCurrentRaycast.gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEntered = false;
        movableUIComp = null;
    }


    // Use this for initialization
    void Start () {
        isEntered = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.Mouse0) && movableUIComp != null)
        {
            movableUIComp.transform.position = Input.mousePosition;
        }
    }
}
