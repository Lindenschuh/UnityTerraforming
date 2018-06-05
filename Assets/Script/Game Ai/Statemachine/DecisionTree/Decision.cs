using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Decision : DecisionTreeNode
{
    public Action nodeTrue;
    public Action nodeFalse;

    public abstract Action GetBranch();
}