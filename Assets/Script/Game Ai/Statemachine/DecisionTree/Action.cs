using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : DecisionTreeNode
{
    [HideInInspector]
    public bool Activated = false;

    private StateManager _stateManager;

    private void Awake()
    {
        _stateManager = GetComponentInParent<StateManager>();
    }

    public override DecisionTreeNode MakeDecision()
    {
        return this;
    }

    private void LateUpdate()
    {
        if (!Activated) return;

        // Here comes the Behaviour!!
        MakeAction();
    }

    public abstract void MakeAction();
}