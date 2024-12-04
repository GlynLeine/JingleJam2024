using UnityEngine;


public class Wisp : Enemy
{
    public float MoveSpeed => m_enemyData.MoveSpeed;
    public float Timer = 0;
    public Vector3 PositionOnEnter => m_targetDetector.PositionOnEnter;
    public bool IsWithinOrbitRadius => m_targetDetector.IsWithinOrbitRadius;
    public bool IsWithinRadius => m_targetDetector.IsWithinRadius;
    protected override void Initialize()
    {

    }

    protected override void OnUpdate()
    {

    }

    protected override void OnDeath()
    {

    }

    public Transform GetTarget()
    {
        return m_targetDetector.FindTarget(transform);
    }

    public bool CanOrbit()
    {
        return m_targetDetector.IsWithinOrbitRadius;
    }

    public void Attack()
    {

    }
}
