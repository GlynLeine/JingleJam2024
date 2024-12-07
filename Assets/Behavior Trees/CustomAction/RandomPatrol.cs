using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RandomPatrol", story: "[Enemy] moves to random point", category: "Action/Navigation", id: "5970eee81802a2c4acdd1f03b92f573f")]
public partial class RandomPatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var navAgent = Enemy.Value.NavAgent;
        navAgent.enabled = true;
        navAgent.stoppingDistance = 1.0f;
        var enemyTransf = Enemy.Value.transform;
        if (navAgent.remainingDistance < 0.5f)
        {
            navAgent.SetDestination(enemyTransf.position + Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere * 5.0f, Vector3.up));
        }
        return Status.Success;
    }
}

