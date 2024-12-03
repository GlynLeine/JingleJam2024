using Unity.Hierarchy;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Stats : MonoBehaviour
{
    #region Health
    [Header("Health Statistics")]
    public int m_Health;
    public int m_MaxHealth;
    public int m_HealthRegenPerTick;
    #endregion

    #region Stamina
    [Header("Stamina Statistics")]
    public int m_Stamina;
    public int m_MaxStamina;
    public int m_StaminaRegenPerTick;
    #endregion

    #region Movement
    [Header("Movement Statistics")]
    public float m_MovementSpeed;
    public float m_MaxMovementSpeed;
    #endregion

    #region Combat
    [Header("Combat Statistics")]
    public int m_AttackStrength;    //Add to each attack's base strength
    public float m_AttackSpeedScale;  //1.0 for normal! 
    public int m_Defence;   //Subtract from incoming damage
    #endregion


    //Move to Player / Enemy Controller!
    //public bool m_bCanMove;
    //public bool m_bIsSprinting; 
}
