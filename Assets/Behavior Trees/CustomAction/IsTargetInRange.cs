using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "IsTargetInRange", story: "Check if [TargetDetector] has a [Target]", category: "Action", id: "ce0910481e07ee35b1909accbfcad1a3")]
public partial class IsTargetInRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<TargetDetector> TargetDetector;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    protected override Status OnUpdate()
    {
        if(TargetDetector.Value.CurrentTarget != null)
        {
            Target.Value = TargetDetector.Value.CurrentTarget;
            return Status.Success;
        }
        return Status.Failure;
    }
}

