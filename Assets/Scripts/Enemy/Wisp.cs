using UnityEngine;
using UnityEngine.AI;
using Bool = Unity.Behavior.BlackboardVariable<bool>;

public class Wisp : Enemy
{
    public Vector3 PositionOnEnter => m_targetDetector.PositionOnEnter;
    public Transform CurrentTarget => m_targetDetector.CurrentTarget;
    public bool IsWithinOrbitRadius => m_targetDetector.IsWithinOrbitRadius;
    protected override void Initialize()
    {
        //if(!m_graphAgent.GetVariable("IsOrbiting", out _isOrbiting))
        //{
        //    Debug.Log("\"IsOrbiting\" variable was not found");
        //}
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnDeath()
    {

    }

    public bool CanOrbit()
    {
        return m_targetDetector.IsWithinOrbitRadius;
    }

    public void Attack()
    {

    }
}
