using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "Ability_Projectile", menuName = "Abilities/Projectile")]
public class Ability_Projectile : Ability
{
    public Vector3 direction;
    public float speed;
    public float size;
    public int damage;
    public float range; 

    [SerializeField] private VisualEffect aoeIndicatorFX;

    public override void Tick(float dt)
    {
        base.Tick(dt); 
    }

    public override void Activate(GameObject owner)
    {
        base.Activate(owner);
        if (status == EAbilityStatus.Ready)
        {
            status = EAbilityStatus.In_Execution; 

            //Spawn a new projectile with this instance's parameters
            GameObject projectile = new GameObject();
            projectile.transform.IsChildOf(owner.transform);
            Projectile p = projectile.AddComponent<Projectile>();
            p.m_Origin = owner.transform.position;
            p.m_Direction = owner.GetComponent<PlayerController>().GetLookAtDirection();
            p.m_Damage = damage;
            p.m_Speed = speed;
            p.m_Size = size;
            p.m_Range = range;

            //(owner.transform.position, direction, speed, size, range, damage))

            projectile.name = this.name + "_projectile";
            projectile.gameObject.layer = owner.layer;

            VisualEffect.Instantiate(vfx, projectile.transform); 
        }
    }

}
