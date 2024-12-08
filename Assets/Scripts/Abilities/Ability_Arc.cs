using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "Ability_Arc", menuName = "Abilities/Arc")]
public class Ability_Arc : Ability
{
    public override EAbilityType type => EAbilityType.Projectile;
    public int Damage;
    public float Range;

    private Arc arc;
    public override void Tick(float dt)
    {
        base.Tick(dt);
        if (status == EAbilityStatus.In_Execution && arc != null)
        {
            arc.Tick();
        }
    }

    public override void Activate(GameObject owner, Transform target = null)
    {
        base.Activate(owner);
        if (status == EAbilityStatus.Ready)
        {
            status = EAbilityStatus.In_Execution;

           var arcGO = new GameObject();
            arcGO.transform.parent = owner.transform;
            arcGO.transform.localPosition = Vector3.zero;
            arcGO.AddComponent<VisualEffect>().visualEffectAsset = vfx;

            arc = arcGO.AddComponent<Arc>();
            arc.Target = target;
            arc.Damage = Damage + owner.gameObject.GetComponent<Stats>().StrengthBonus;
            arc.Range = Range;
            arc.Initialize(this);

            arcGO.name = name + "_arc";
            arcGO.gameObject.layer = owner.layer;
        }
    }
}
