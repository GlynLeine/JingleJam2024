using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToRandomPositionAwayFromTarget", story: "[Enemy] moves to random point away from [Target]", category: "Action", id: "d921fce4f986ddd08d409d08a58ff232")]
public partial class MoveToRandomPositionAwayFromTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<Transform> Target;

    protected override Status OnStart()
    {
        return Status.Running;
    }
    private Vector3 dif;
    private Vector3 direction;
    private Vector3 randomPos;
    protected override Status OnUpdate()
    {
        var targetTransf = Target.Value;
        var navAgent = Enemy.Value.NavAgent;
        navAgent.enabled = true;
        navAgent.stoppingDistance = 1.0f;
        if (navAgent.remainingDistance <= 1.0f)
        {
            navAgent.ResetPath();
            dif = (navAgent.transform.position - targetTransf.position);
            direction = -dif.normalized;
            randomPos = (direction * UnityEngine.Random.Range(dif.magnitude * .5f, dif.magnitude * 2.0f)) + new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), 0, 0);
        }
        navAgent.SetDestination(randomPos);
        Debug.DrawLine(navAgent.transform.position, randomPos, Color.red);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

