using UnityEngine;

public class Golem : Enemy
{
    protected override void Initialize() { }
    protected override void OnUpdate() { }
    protected override void OnDeath() { }
    public override void Attack()
    {
        m_abilityManager.Activate(Random.Range(0, m_abilityManager.GetAbilityCount()), Target);
    }

    public override void TakeDamage(float damage)
    {
        m_stats.Health -= damage;
    }
}
