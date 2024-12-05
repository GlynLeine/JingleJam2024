using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.Rendering.GPUSort;

[CreateAssetMenu(fileName = "Ability_Arc", menuName = "Abilities/Arc")]
public class Ability_Arc : Ability
{
    public override EAbilityType type => EAbilityType.Projectile;
    public Transform Target;
    public int Damage;
    public float Range;

    private GameObject arcGO;
    private Arc arc;
    public override void Tick(float dt)
    {
        base.Tick(dt);
        if (status == EAbilityStatus.In_Execution && arc != null)
        {
            arc.Tick();
        }

    }

    public override void Activate(GameObject owner)
    {
        base.Activate(owner);
        if (status == EAbilityStatus.Ready)
        {
            status = EAbilityStatus.In_Execution;

            arcGO = new GameObject();
            arcGO.transform.parent = owner.transform;
            arcGO.transform.localPosition = Vector3.zero;
            arcGO.AddComponent<VisualEffect>().visualEffectAsset = vfx;

            arc = arcGO.AddComponent<Arc>();
            arc.Target = Target;
            arc.Damage = Damage;
            arc.Range = Range;
            arc.Initialize(this);

            arcGO.name = name + "_arc";
            arcGO.gameObject.layer = owner.layer;
        }
    }
}
