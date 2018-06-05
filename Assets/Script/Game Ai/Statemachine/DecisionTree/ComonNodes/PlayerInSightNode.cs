using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInSightNode : DecisionTreeNode
{
    public DecisionTreeNode TrueNode;
    public DecisionTreeNode FalseNode;

    public override DecisionTreeNode MakeDecision()
    {
        if (CheckPlayerInSight())
        {
            return TrueNode.MakeDecision();
        }
        return FalseNode.MakeDecision();
    }

    private bool CheckPlayerInSight()
    {
        throw new NotImplementedException();
    }
}