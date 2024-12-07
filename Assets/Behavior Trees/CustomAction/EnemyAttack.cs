using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnemyAttack", story: "[Enemy] attacks", category: "Action", id: "6d8ec7e692fa310b843026cc4c06ac06")]
public partial class WispAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    protected override Status OnStart()
    {
        Enemy.Value.NavAgent.enabled = false;
        Enemy.Value.Attack();
        return Status.Success;
    }

}

