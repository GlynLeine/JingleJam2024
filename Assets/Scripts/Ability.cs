using NaughtyAttributes;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.VFX;

public enum EAbilityType
{
    None = 0,
    Passive,
    Target_Self,
    Projectile,
    Circle_AoE,
    Line_AoE,
    Cone_AoE,
    EAbilityType_MAX
}

public enum EAbilityStatus
{
    Ready = 0,
    In_Execution,
    In_Cooldown,
    EAbilityStatus_MAX
}

[CreateAssetMenu(fileName = "Ability", menuName = "Abilities/")]
public class Ability : ScriptableObject
{
    public virtual EAbilityType type => EAbilityType.None;
    [ReadOnly]
    public EAbilityStatus status;
    public string name;
    public string description;
    public string animationName;
    public VisualEffectAsset vfx;
    public float castTimer = 0.0f;
    public float castTime = 0.0f;
    public float recastTimer = 0.0f;
    public float recastTime = 0.0f;
    private float executionTimer = 0.0f;

    public virtual void Tick(float dt)
    {
        Debug.Log(name + " Tick");

        //Set the appropriate skill state
        switch (status)
        {
            case EAbilityStatus.Ready:
                executionTimer = castTime;
                recastTimer = recastTime;
                break;
            case EAbilityStatus.In_Execution:
                if (executionTimer <= 0)
                {
                    status = EAbilityStatus.In_Cooldown;
                    break;
                }
                executionTimer -= dt;
                recastTimer -= dt;
                break;
            case EAbilityStatus.In_Cooldown:
                if (recastTimer <= 0.0f)
                {
                    status = EAbilityStatus.Ready;
                    executionTimer = castTime;
                    recastTimer = recastTime;
                    break;
                }
                recastTimer -= dt;
                break;
            default:
                break;
        }
    }

    public virtual void Activate(GameObject owner)
    {

    }
}
