using System;
using UnityEngine;

[CreateAssetMenu]
public class Skill_DEBUG : Skill
{
    public override void Activate()
    {
        Debug.Log("Skill Triggered!"); 
    }
}
