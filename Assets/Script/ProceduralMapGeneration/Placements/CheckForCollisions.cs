using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForCollisions : MonoBehaviour {

    private PositionCheck managerPositionChecks;
    void Awake()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.isTrigger = true;
        }

        Rigidbody rb;
        if (GetComponent<Rigidbody>() == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (managerPositionChecks == null)
            managerPositionChecks.ObjectIsColliding();
    }

    public IEnumerator IsColliding(PositionCheck positionChecks)
    {
        managerPositionChecks = positionChecks;

        transform.SetPositionAndRotation(managerPositionChecks.randomPosition, managerPositionChecks.normalizedRotation);

        yield return new WaitForFixedUpdate();
        managerPositionChecks = null;
    }
}
