using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInRangeNode : DecisionTreeNode
{
    public Action TrueAction;
    public Action FalseAction;

    public override DecisionTreeNode MakeDecision()
    {
        throw new System.NotImplementedException();
    }
}