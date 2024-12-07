using UnityEngine;

[RequireComponent(typeof(Stats))]
public class AbilityManager : MonoBehaviour
{
    [SerializeField] private Ability[] m_Abilities;

    public void Start()
    {
        for (int i = 0; i < m_Abilities.Length; i++)
        {
            m_Abilities[i] = (Ability)ScriptableObject.Instantiate(m_Abilities[i]);     //Create instances of the referenced abilities. 
        }
    }
    public void Activate(int index)
    {
        if (index > m_Abilities.Length)
        {
            Debug.LogError("Invalid Index!");
            return;
        }

        //Trigger the requested ability
        m_Abilities[index].Activate(this.gameObject);
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
}
