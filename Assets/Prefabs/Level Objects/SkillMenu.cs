using UnityEngine;

public class SkillMenu : MonoBehaviour
{
    [SerializeField] public Ability[] abilities; 
    [SerializeField] public bool m_bIsSacrifice = false;  //By selecting this, are we sacrificing a skill? 
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //If the menu is intended for a skill gain, ensure it has enough abilities set
        if (!m_bIsSacrifice)
        {
            if (abilities.Length < 3)
            {
                Debug.LogError("Skill menu must have 3 abilities!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //If this is for a sacrifice, grab the player's current skills. 
        //TODO: Move this out of Update! ...Somehow!
        if(m_bIsSacrifice)
        {
            for(int i = 0; i < 3; i++)
            {
                abilities[i] = GameObject.FindWithTag("Player").GetComponent<AbilityManager>().GetAbilities()[i]; 
            }
        }
        
    }

    public void OnSelectSkill(int idx)
    {
        Debug.Log("Skill " + idx + " Selected");
        if (m_bIsSacrifice) //Remove a skill
        {
            GameObject.FindWithTag("Player").GetComponent<AbilityManager>().SetAbility(idx, null); 
        }
        else //Add a skill
        {
            GameObject.FindWithTag("Player").GetComponent<AbilityManager>().SetAbility(idx, abilities[idx] ); 
        }

        //"Close" the skill menu
        this.gameObject.SetActive(false);
        GameObject.Destroy(this.gameObject); 
    }
}
