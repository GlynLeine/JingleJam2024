using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CanOrbit", story: "[Self] can Orbit", category: "Conditions", id: "60017bd6115d002d24649c7a009ab16c")]
public partial class CanOrbitCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Wisp> Self;

    public override bool IsTrue()
    {
        return Self.Value.IsWithinOrbitRadius;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
