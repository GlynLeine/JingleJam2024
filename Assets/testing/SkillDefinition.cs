using System.Threading;
using UnityEngine;

public class SkillDefinition : MonoBehaviour
{
    public Skill skill;


    private void FixedUpdate()
    {
        //Set the appropriate skill state
        switch (skill.status)
        {
            case ESkillStatus.Ready:
                skill.executionTimer = skill.cast;
                skill.recastTimer = skill.recast;
                break;
            case ESkillStatus.In_Execution:
                if (skill.executionTimer <= 0)
                {
                    skill.status = ESkillStatus.In_Cooldown;
                    break; 
                }
                skill.executionTimer -= Time.fixedDeltaTime;
                skill.recastTimer -= Time.fixedDeltaTime;
                break;
            case ESkillStatus.In_Cooldown:
                if (skill.recastTimer <= 0.0f)
                {
                    skill.status = ESkillStatus.Ready;                skill.executionTimer = skill.cast;
                    skill.recastTimer = skill.recast;
                    break;
                }
                skill.recastTimer -= Time.fixedDeltaTime;
                break;
            default:
                break;
        }

    }
    public void TriggerSkill()
    {
        if (skill.status == ESkillStatus.Ready)
        {
            skill.status = ESkillStatus.In_Execution;
            Animator animator = this.GetComponent<Animator>();
            animator.Play(skill.animationName);
            animator.SetFloat("motionTime", 1.0f - skill.recastTimer); 

            skill.Activate();
        }
    }
}

