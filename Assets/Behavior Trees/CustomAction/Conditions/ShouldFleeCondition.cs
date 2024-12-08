using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ShouldFlee", story: "[Enemy] should flee", category: "Conditions", id: "708269a3732f69bd3185ec58858d071b")]
public partial class ShouldFleeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;

    public override bool IsTrue()
    {
        return Enemy.Value.IsWithinFleeRadius;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
