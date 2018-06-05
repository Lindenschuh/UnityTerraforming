using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTerraforming.GameAi;
using System.Linq;

public enum GuardianStates
{
    WANDER,
    OUTSIDE,
    CHASING,
    ATTACK
}

[System.Serializable]
public class State
{
    public GuardianStates StateIdentifier;
    public List<AgentBehaviour> NeededBehaviours;
}

public class StateManager : MonoBehaviour
{
    private State CurrentState;
    public List<State> States;

    public bool ChangeState(GuardianStates switchTo)
    {
        State state = States.First(s => s.StateIdentifier == switchTo);

        if (state != null)
        {
            CurrentState.NeededBehaviours.ForEach(b => b.enabled = false);
            state.NeededBehaviours.ForEach(b => b.enabled = true);
            CurrentState = state;
            return true;
        }
        return false;
    }
}