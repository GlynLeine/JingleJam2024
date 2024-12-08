using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Can Attack", story: "[Enemy] CanAttack", category: "Conditions", id: "c10328a9655f19465936737e0ab44c72")]
public partial class CanAttackCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;

    public override bool IsTrue()
    {
        Enemy.Value.NavAgent.stoppingDistance = Enemy.Value.AttackRadius;
        Enemy.Value.NavAgent.enabled = !Enemy.Value.IsWithinAttackRadius;
        return Enemy.Value.IsWithinAttackRadius;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
