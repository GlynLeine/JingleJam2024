using UnityEngine;

public class Golem : Enemy
{

    protected override void Initialize() { }

    protected override void OnUpdate() { }

    protected override void OnDeath() { }
    public override void Attack()
    {
        if (m_abilityInstance == null)
            m_abilityInstance = Instantiate(m_abilityTemplate);

        (m_abilityInstance as Ability_Arc).Target = Target;
        m_abilityInstance.Activate(gameObject);
    }

    public override void TakeDamage()
    {
    }


}
