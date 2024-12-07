using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToTarget", story: "[Enemy] move to [Target]", category: "Action", id: "cd2eef329ba52283fabf5bebc991ec1a")]
public partial class WispMovesToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    protected override Status OnStart()
    {
        var navAgent = Enemy.Value.NavAgent;
        navAgent.enabled = true;
        navAgent.stoppingDistance = 1.0f;
        navAgent.SetDestination(Target.Value.position);
        return Status.Success;
    }
}

