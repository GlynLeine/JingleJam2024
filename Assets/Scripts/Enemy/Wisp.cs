using NaughtyAttributes;
using UnityEngine;


public class Wisp : Enemy
{
    public float Timer = 0;
    public Vector3 PositionOnEnter => m_targetDetector.PositionOnEnter;
    protected override void Initialize() { }
    protected override void OnUpdate() { }
    protected override void OnDeath() { }

    public override void Attack()
    {
        m_abilityManager.Activate(Random.Range(0, m_abilityManager.GetAbilityCount()), Target);
    }

    public override void TakeDamage(float damage)
    {
        
    }
}
