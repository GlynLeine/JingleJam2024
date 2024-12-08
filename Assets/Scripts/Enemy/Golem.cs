using UnityEditor.Animations;
using UnityEngine;

public class Golem : Enemy
{
    private Animator m_AnimController;

    protected override void Initialize() { 
        m_AnimController = GetComponentInChildren<Animator>();
    }

    protected override void OnUpdate() {
        m_AnimController.SetFloat("movementSpeed", m_navAgent.desiredVelocity.magnitude / m_navAgent.speed);
    }

    protected override void OnDeath() { }
    public override void Attack()
    {
        if(m_abilityManager == null)
            m_abilityManager = GetComponent<AbilityManager>();

        m_abilityManager.Activate(Random.Range(0, m_abilityManager.GetAbilityCount()), Target);
    }

    public override void TakeDamage(float damage)
    {
        m_stats.Health -= damage; 
    }


}
