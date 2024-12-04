using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RandomPatrol", story: "[Wisp] moves to random point", category: "Action/Navigation", id: "5970eee81802a2c4acdd1f03b92f573f")]
public partial class RandomPatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<Wisp> Wisp;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var navAgent = Wisp.Value.NavAgent;
        var wispTransf = Wisp.Value.transform;
        if (navAgent.isActiveAndEnabled && navAgent.remainingDistance < 0.5f)
        {
            navAgent.SetDestination(wispTransf.position + Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere * 5.0f, Vector3.up));
        }
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

