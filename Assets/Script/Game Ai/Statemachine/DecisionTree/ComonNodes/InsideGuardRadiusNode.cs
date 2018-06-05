using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsideGuardRadiusNode : DecisionTreeNode
{
    public Action TrueAction;
    public Action FalseAction;

    public override DecisionTreeNode MakeDecision()
    {
        throw new System.NotImplementedException();
    }
}