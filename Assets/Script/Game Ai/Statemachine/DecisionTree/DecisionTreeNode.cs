using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecisionTreeNode : MonoBehaviour
{
    public abstract DecisionTreeNode MakeDecision();
}