using UnityEngine;

[RequireComponent(typeof(Stats))]
public class AbilityManager : MonoBehaviour
{
    [SerializeField] private Ability[] m_Abilities;
    [SerializeField] private Animator m_Animator; 

    public void Start()
    {
        for (int i = 0; i < m_Abilities.Length; i++)
        {
            m_Abilities[i] = (Ability)ScriptableObject.Instantiate(m_Abilities[i]);     //Create instances of the referenced abilities. 
            m_Abilities[i].status = EAbilityStatus.Ready; 
        }
    }
    public void Activate(int index, Transform target = null)
    {
        if (index > m_Abilities.Length)
        {
            Debug.LogError("Invalid Index!");
            return;
        }
        //Trigger the requested ability
        m_Abilities[index]?.Activate(gameObject, target);

        if(m_Animator != null){
            m_Animator.Play(m_Abilities[index]?.animationName); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Tick all abilities 
        for (int i = 0; i < m_Abilities.Length; i++)
        {
            m_Abilities[i].Tick(Time.deltaTime);
        }
    }

    public int GetAbilityCount()
    {
        return m_Abilities.Length;
    }

    public Ability[] GetAbilities()
    {
        return m_Abilities;
    }
    public void SetAbility(int idx, Ability ability)
    {
        m_Abilities[idx] = ability;  
    }
}
