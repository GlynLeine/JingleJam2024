using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "HasTarget", story: "If [Target] is not Null", category: "Conditions", id: "eebe6061bcf2c058759c33a70f2fb0c5")]
public partial class HasTargetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Transform> Target;

    public override bool IsTrue()
    {
        return Target.Value != null;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
