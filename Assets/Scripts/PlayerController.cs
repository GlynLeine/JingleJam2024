using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public enum EInputActions
{
    None = 0,
    Move,
    Attack,
    Block,
    Skill_1,
    Skill_2,
    Skill_3,
    EInputActions_MAX
}


public enum EPlayerState
{
    None = 0,
    Alive,
    Dead,
    EPlayerState_MAX
}

[System.Serializable]
public struct PlayerInputAction
{
    public EInputActions mapping;
    public string inputAction;
    public bool isEnabled;

    public void Enable()
    {
        isEnabled = true;
    }
    public void Disable()
    {
        isEnabled = false;
    }

}


[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AbilityManager))]
public class PlayerController : MonoBehaviour, IDamageable
{
    private Stats m_Stats;
    public Stats Stats => m_Stats;
    private Rigidbody m_Rigidbody;
    private CapsuleCollider m_CapsuleCollider;

    [SerializeField] private GameObject m_Camera;
    [SerializeField] private GameObject m_SkeletalMeshParent;

    [SerializeField] private PlayerInputAction[] m_ActionMappings;

    [SerializeField] private bool m_bCanMove;   //Is the player allowed to move?
    //[SerializeField] private bool m_bIsBlocking;
    [SerializeField] public bool m_bCanInteract;
    [SerializeField] private EPlayerState m_StateThisFrame;
    [SerializeField] private EPlayerState m_StateLastFrame;
    [SerializeField] private uint m_AttackIndex;    //Which normal attack are we on?

    [SerializeField] private Vector3 m_LookDirection = Vector3.forward;   //Where are we oriented towards?
    private Dictionary<EInputActions, InputAction> m_InputActionMappings = new Dictionary<EInputActions, InputAction>();     //Store a map of action types to indices

    [SerializeField] private float m_MovementDeadzone; //Minimum magnitude of the movement vector
    [SerializeField] private bool m_bCanRotate;
    [SerializeField] private float m_RotationSpeed;

    public GameObject m_Interactable;   //This is public! Yes, it's bad code! no, I don't care!
    private AbilityManager m_AbilityManager;
    private Animator m_Animator;

    public Vector3 forward
    {
        get => m_LookDirection;
    }

    private void Start()
    {
        //Cache a reference to the player's stats
        m_Stats = GetComponent<Stats>();
        m_LookDirection = Vector3.zero;  //initially look "nowhere"

        //Cache references to required components / objects 
        m_Rigidbody = GetComponent<Rigidbody>();
        m_CapsuleCollider = GetComponent<CapsuleCollider>();

        m_Animator = GetComponentInChildren<Animator>();

        // m_IAMovement = InputSystem.actions.FindAction("Move");

        //TODO: Action Mappings for dynamic enable / disable!
        //Retrieve references to the action mappings we care about. 
        for (uint i = 0; i < m_ActionMappings.Length; i++)
        {
            if (!m_InputActionMappings.TryAdd(m_ActionMappings[i].mapping, InputSystem.actions.FindAction(m_ActionMappings[i].inputAction)))
            {
                Debug.LogWarning("Duplicate Action Mapping detected: This was probably unintended [" + i + "]");
            }

        }

        m_StateThisFrame = EPlayerState.Alive;
        m_StateLastFrame = EPlayerState.Alive;

        m_AbilityManager = GetComponent<AbilityManager>();
        if (m_AbilityManager.GetAbilityCount() < 3)
        {
            Debug.LogError("Player must have 3 abilities bound!");
        }
    }

    private void Update()
    {
        if (!CheckDeathState())
        {
            PlayerDeath();
        }

        UpdateAnimControllerState();
        ProcessMovement();

        //TODO: Refactor to use the ability bindings; This is bad code!!!
        if (InputSystem.actions.FindAction("Attack").ReadValue<float>() > 0.0f)
        {
            Debug.Log("Attack!");
        }
        else if (InputSystem.actions.FindAction("Interact").ReadValue<float>() > 0.0f)
        {
            Debug.Log("Interacting with " + m_Interactable.name);
            m_Interactable.GetComponent<IInteractable>().OnInteract(); 
        }

        else if (InputSystem.actions.FindAction("Skill_1").ReadValue<float>() > 0.0f)
        {
            m_AbilityManager.Activate(0);   //Activate the skill at the given index
        }
        else if (InputSystem.actions.FindAction("Skill_2").ReadValue<float>() > 0.0f)
        {
            m_AbilityManager.Activate(1);   //Activate the skill at the given index
        }
        else if (InputSystem.actions.FindAction("Skill_3").ReadValue<float>() > 0.0f)
        {
            m_AbilityManager.Activate(2);   //Activate the skill at the given index
        }
        else if (InputSystem.actions.FindAction("Skill_1").ReadValue<float>() > 0.0f)
        {
            //m_AbilityManager.Activate(0);   //Activate the skill at the given index
            Debug.Log("Dodge Roll!");
        }

    }

    private void LateUpdate()
    {
        m_StateLastFrame = m_StateThisFrame;
        m_StateThisFrame = EPlayerState.None;
    }

    public void Tick()
    {
        m_Stats.Tick();
    }

    private void OnDrawGizmos()
    {
        //Position / Orientation data    
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(this.transform.position, 0.25f);
        Gizmos.DrawRay(this.transform.position + new Vector3(0.0f, 1.5f, 0.0f), m_LookDirection);

        Gizmos.color = Color.red;
        InputAction movementAction;
        if (m_InputActionMappings.TryGetValue(EInputActions.Move, out movementAction))
        {
            Vector2 movementInput =
            movementAction.ReadValue<Vector2>();
            Vector3 velocity = (m_Camera.transform.forward * movementInput.y + m_Camera.transform.right * movementInput.x);
            Gizmos.DrawRay(this.transform.position + new Vector3(0.0f, 1.8f, 0.0f), velocity);
        }
        if (m_bCanMove == false)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(this.transform.position + new Vector3(0.0f, 2.2f, 0.0f), 0.3f);
        }
        if (m_bCanRotate == false)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(this.transform.position + new Vector3(0.0f, 2.5f, 0.0f), 0.3f);
        }

    }


    private bool CheckDeathState()
    {
        //Test to see whether the player died.
        if (m_StateLastFrame == EPlayerState.Alive && m_Stats.Health <= 0)
        {
            m_StateThisFrame = EPlayerState.Dead;
            return false;
        }
        if (m_Stats.Health > 0)
        {
            m_StateThisFrame = EPlayerState.Alive;
        }
        if (m_StateThisFrame == EPlayerState.None)
        {
            Debug.LogWarning("Player state was none!");
        }

        return true;
    }

    private void PlayerDeath()
    {
        Debug.Log("Player Death!\n");
        m_bCanMove = false;
        m_bCanRotate = false;
        //Todo: Trigger a Death animation etc...
        m_StateThisFrame = EPlayerState.Dead;
        m_Animator.SetTrigger("OnDeath");
        m_Animator.SetBool("isDead", true);
    }

    private void UpdateAnimControllerState()
    {

        m_Animator.SetFloat("movementSpeed", 0.0f);

    }
    private void ProcessMovement()
    {
        InputAction movementAction = m_InputActionMappings[EInputActions.Move];
        if (movementAction == null)
        {
            Debug.LogError("Movement action was Null!");
            return;
        }

        //If the player isn't allowed to submit movement input, return!
        /*
        if (m_Input.isEnabled == false)
        {
            return; 
        }
        */

        //Retrieve the input value
        Vector2 movementInput = movementAction.ReadValue<Vector2>();
        //Debug.Log(movementInput); 
        if (movementInput.magnitude > m_MovementDeadzone)   //Only process input when it's outside of the deadzone
        {
            //Compute desired velocity
            Vector3 velocity = Vector3.zero;
            velocity = (m_Camera.transform.forward * movementInput.y + m_Camera.transform.right * movementInput.x);
            //Cache the player's look-at direction

            {
                Vector3 n = Vector3.up;
                {

                    RaycastHit hit;
                    if (Physics.Raycast(this.transform.position + (m_LookDirection * 10.0f), Vector3.down, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Terrain")))
                    {
                        n = hit.normal;
                    }
                }

                float n_dot_up = Vector3.Dot(n, Vector3.up);
                float min = Mathf.Cos(20.0f);
                float max = Mathf.Cos(20.0f);
                if (n_dot_up < 0.9f)
                {
                    Debug.Log(n_dot_up);
                    return;
                }

                //Snap the player to the ground!
                // transform.position.Set(transform.position.x, hit.point.y, transform.position.z);
                float n_dot_v = Vector3.Dot(velocity, n);
                velocity = (velocity - n * n_dot_v);

                if (velocity.magnitude > 1.0f)
                {
                    velocity = velocity.normalized;
                }
            }
            m_LookDirection = new Vector3(velocity.x, /*velocity.y*/ 0.0f, velocity.z).normalized;
            m_Animator.SetFloat("movementSpeed", ((velocity.magnitude * m_Stats.MovementSpeed) / m_Stats.MaxMovementSpeed));

            /*
            //Snap the player to the navmesh!
            {
                RaycastHit hit; 
                if(!Physics.Raycast(m_Rigidbody.position + (velocity * m_Stats.m_MovementSpeed * Time.deltaTime), Vector3.down, out hit, Mathf.Infinity))
                {
                    return; 
                }

                if (hit.transform.gameObject.layer != LayerMask.NameToLayer("levelGeometry"))
                {
                    return;
                }
            }
             * */
            if (m_bCanMove)
            {
                m_Rigidbody.MovePosition(m_Rigidbody.position + (velocity * m_Stats.MovementSpeed * Time.deltaTime));     //Move the player's rigidbody
            }
            if (m_bCanRotate)
            {
                //Smoothly rotate the player's Skeletal Mesh towards their target velocity. 
                m_SkeletalMeshParent.transform.rotation = Quaternion.RotateTowards(m_SkeletalMeshParent.transform.rotation, Quaternion.LookRotation(m_LookDirection), m_RotationSpeed * Time.deltaTime);
            }
        }

    }

    public Vector3 GetLookAtDirection()
    {
        return m_LookDirection;
    }

    void IDamageable.TakeDamage(float damage)
    {
        m_Stats.Health -= damage;
    }
}
