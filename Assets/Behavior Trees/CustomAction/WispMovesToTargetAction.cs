using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WispMovesToTarget", story: "[Wisp] move to [Target]", category: "Action", id: "cd2eef329ba52283fabf5bebc991ec1a")]
public partial class WispMovesToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Wisp> Wisp;
    [SerializeReference] public BlackboardVariable<Transform> Target;

    protected override Status OnStart()
    {
        Wisp.Value.NavAgent.SetDestination(Target.Value.position);
        return Status.Success;
    }
}

