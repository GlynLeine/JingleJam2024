using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    GameObject owner;
    Rigidbody rb;
    GameObject camera;
    Stats m_Stats; 

    [SerializeField] private GameObject spline;

    [SerializeField] private GameObject mesh;

    InputAction moveAction;
    InputAction attackAction;
    InputAction skillAction;

    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private Vector3 m_Velocity;
    [SerializeField] private float m_GroundCheckDist = 5.0f;
    [SerializeField] private float m_SmoothTime = 560.0f;
    [SerializeField] private float m_CameraSplineDist = 10.0f;
    [SerializeField] private float m_MaxSpeedPercent = 0.00f;

    [SerializeField] private Vector3 m_CameraOffset;
    [SerializeField] public Skill[] m_Skills;
    [SerializeField] private bool m_bIsDeadThisFrame;
    [SerializeField] private bool m_bWasDeadLastFrame;

    int m_AttackIndex;
    float m_AttackDelay; 
    private Vector3 nearestSplinePoint;
    private Vector3 splineTangent;

    [SerializeField] public SkillDefinition[] skills;
    [SerializeField] private SkillDefinition attackSkill;
    [SerializeField] private SkillDefinition defendSkill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        owner = GameObject.FindGameObjectWithTag("Player");
        rb = owner.GetComponent<Rigidbody>();

        camera = GameObject.FindGameObjectWithTag("Camera");
        //Cache Input system action references
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        skillAction = InputSystem.actions.FindAction("Skill");
        m_Stats = GetComponent<Stats>();

        m_bWasDeadLastFrame = true;
        m_bIsDeadThisFrame = false;

        //skills[0] = owner.GetComponent<SkillDefinition>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        m_MaxSpeedPercent = moveValue.magnitude;
        if (moveValue.magnitude > 0.2f)
        {
            m_Velocity = (camera.transform.forward * moveValue.y + camera.transform.right * moveValue.x);
            if (m_Velocity.magnitude > 1.0f)
            {
                m_Velocity = m_Velocity.normalized;
            }
            rb.MovePosition(rb.position + (m_Velocity * movementSpeed * Time.deltaTime));
            //rb.linearVelocity = (m_Velocity * movementSpeed * Time.deltaTime);
            //rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, velocity, ref m_Velocity, m_SmoothTime); 
            //owner.transform.rotation = Quaternion.LookRotation(m_Velocity);
            Debug.DrawRay(owner.transform.position + new Vector3(0.0f, 2.0f, 0.0f), m_Velocity * 3.0f, Color.green);
            mesh.transform.rotation = Quaternion.RotateTowards(mesh.transform.rotation, Quaternion.LookRotation(m_Velocity), m_SmoothTime * Time.deltaTime);
        }

        SplineContainer splineContainer = spline.GetComponent<SplineContainer>();
        if (splineContainer != null)
        {
            Spline s = splineContainer.Spline;

            float t;
            float3 nearest;

            SplineUtility.GetNearestPoint<Spline>(s, owner.transform.position, out nearest, out t);
            nearestSplinePoint = SplineUtility.EvaluatePosition<Spline>(s, t);
            nearestSplinePoint += spline.transform.position;
            splineTangent = SplineUtility.EvaluateTangent<Spline>(s, t);

        }


        float distToNearestSplinePoint = Vector3.Distance(owner.transform.position, nearestSplinePoint);
        if (distToNearestSplinePoint < m_CameraSplineDist)
        {
            Vector3 camTgtPos = nearestSplinePoint + m_CameraOffset;
            camera.transform.position = Vector3.Slerp(owner.transform.position, camTgtPos, distToNearestSplinePoint / m_CameraSplineDist);
            camera.transform.rotation = Quaternion.FromToRotation(Vector3.forward, splineTangent.normalized);
        }
        else
        {
            camera.transform.position = new Vector3(m_CameraOffset.x + owner.transform.position.x, m_CameraOffset.y + owner.transform.position.y, m_CameraOffset.z + owner.transform.position.z);

        }


        Animator animator = this.GetComponent<Animator>();

        if (attackAction.ReadValue<float>() > 0.0f)
        {
            m_AttackIndex++; 

            switch (m_AttackIndex)
            {
                case 1:
                    m_AttackDelay = 0.5f; 
                    break;
                default:
                    break; 
            }
            animator.SetTrigger("OnAttack");
        }
        if(m_AttackDelay > 0.0f)    //TODO: Attack Recast - use Skill interface???
        {
            m_AttackDelay -= Time.deltaTime;
            m_AttackIndex = 0; 
        }
        else
        {
            animator.ResetTrigger("OnAttack"); 
        }
        
        if(skillAction.ReadValue<float>() > 0.0f)
        {
            skills[0].TriggerSkill();
        }

        animator.SetFloat("maxSpeedPercent", m_MaxSpeedPercent);

        //If player just died
        if (m_bIsDeadThisFrame == true && m_bWasDeadLastFrame == false)
        {
            animator.SetTrigger("OnDeath");
            m_bWasDeadLastFrame = true;
        }
        //Death loop
        else if (m_bIsDeadThisFrame == true && m_bWasDeadLastFrame == true)
        {
            animator.ResetTrigger("OnDeath");
            animator.SetBool("isDead", true);
        }
        //Revival
        else if (m_bIsDeadThisFrame == false && m_bWasDeadLastFrame == true)
        {
            animator.SetBool("isDead", false);
            m_bIsDeadThisFrame = false;
            m_bWasDeadLastFrame = false;
        }

        /*
        Ray ray = new Ray(owner.transform.position, Vector3.down);
        Debug.DrawRay(owner.transform.position, Vector3.down * m_GroundCheckDist, Color.green);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(owner.transform.position, Vector3.down, out hit, m_GroundCheckDist))
        {
            m_bIsGrounded = true;
        }
        else
        {
            m_bIsGrounded = false;
        }

        if (attackAction.ReadValue<float>() > 0.0f && m_bIsGrounded == true && m_bIsJumping == false)
        {
            m_bIsJumping = true;
            rb.AddForce(0, 100.0f, 0);
        }
        if (m_bIsJumping == true && m_bIsGrounded == false)
        {
            Debug.Log("Hit!");
            m_bIsJumping = false;
        }

        float3 tgtPos; 
        float3 nearest = new float3();
        float t;
        SplineUtility.GetNearestPoint<Spline>(spline.GetComponent<SplineContainer>().Spline, owner.transform.position, out nearest, out t);
        tgtPos = spline.GetComponent<SplineContainer>().EvaluatePosition(t); 
        Debug.DrawRay(owner.transform.position + new Vector3(0.0f, 1.5f, 0.0f), tgtPos * t, Color.cyan);
        float dst = Vector3.Distance(owner.transform.position, tgtPos);
        if (dst < m_CameraSplineDist)
        {
            //camera.transform.position = nearest; 
            camera.transform.position = new Vector3(m_CameraOffset.x + tgtPos.x, m_CameraOffset.y + owner.transform.position.y, m_CameraOffset.z + tgtPos.z);
        }
        else
        {

            camera.transform.position = new Vector3(m_CameraOffset.x + owner.transform.position.x, m_CameraOffset.y + owner.transform.position.y, m_CameraOffset.z + owner.transform.position.z);
        }



        Debug.DrawRay(owner.transform.position + new Vector3(0.0f, 1.5f, 0.0f), nearest * dst, Color.cyan);
*/
    }
    private void OnDrawGizmos()
    {

        //Player Forwards Vector
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(this.transform.position + new Vector3(0.0f, 1.5f, 0.0f), m_Velocity * 5.0f);

        Gizmos.DrawSphere(this.transform.position, 0.5f); //Player Position


        Gizmos.color = Color.red;
        if (spline != null)
        {
            Vector3 toNearestSplinePoint = (nearestSplinePoint - this.transform.position).normalized;
            float distToNearestSplinePoint = Vector3.Distance(this.transform.position, nearestSplinePoint);
            Gizmos.DrawRay(this.transform.position + new Vector3(0.0f, 1.8f, 0.0f), toNearestSplinePoint * distToNearestSplinePoint);
            Gizmos.DrawSphere(nearestSplinePoint, 1.5f);

            Gizmos.DrawRay(nearestSplinePoint + new Vector3(0.0f, 1.0f, 0.0f), splineTangent.normalized * 10.0f);
        }

        /*
        float3 nearest;
        float t; 
        SplineUtility.GetNearestPoint<Spline>(spline.GetComponent<SplineContainer>().Spline, this.transform.position, out nearest, out t); 
        SplineContainer splineContainer = spline.GetComponent<SplineContainer>();
        float3 newPos = splineContainer.EvaluatePosition(t);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPos, 3.0f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(nearest, 3.0f);
        */
    }



    /*
public void DebugAnimCallback()
{
     * 
    Debug.Log("Hello, from Anim!");
    Skill sk = Object.Instantiate(m_Skills[0], owner.transform);
    sk.gameObject.SetActive(true); 
    VisualEffect vfx = sk.gameObject.GetComponent<VisualEffect>();

    StartCoroutine(WaitForVFXCompletion(vfx));
}

IEnumerator WaitForVFXCompletion(VisualEffect vfx)
{
    yield return new WaitUntil(() => vfx.HasAnySystemAwake() == false);
    Debug.Log("Destroying VFX!");
    vfx.gameObject.SetActive(false);
    Object.Destroy(vfx.gameObject); 
}

IEnumerator Example()
{
    Debug.Log("Waiting for princess to be rescued...");
    yield return new WaitUntil(() => 1 >= 10);
    Debug.Log("Princess was rescued!");
}
    */


}
