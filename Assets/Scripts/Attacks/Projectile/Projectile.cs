using System.Collections;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Projectile : MonoBehaviour
{
    [HideInInspector]
    public Vector3 m_Origin;
    [HideInInspector]
    public Vector3 m_Direction;
    [HideInInspector]
    public float m_Speed;
    [HideInInspector]
    public float m_Size;
    [HideInInspector]
    public int m_Damage;
    public VisualEffect vfx;
    [Tooltip("This is the health of the projectile. Each enemy a projectile hits will take off a health from the porjectile. If the health is 0 it will destroy itself")]
    public int m_Health = 3;

    private Rigidbody m_Rigidbody;
    private CapsuleCollider m_CapsuleCollider;

    public LayerMask m_LayerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.useGravity = false;
        m_Rigidbody.position = m_Origin;

        m_CapsuleCollider = GetComponent<CapsuleCollider>();
        m_CapsuleCollider.radius = m_Size;
        m_CapsuleCollider.isTrigger = true;
        m_Rigidbody.AddForce(m_Direction * m_Speed * 100f);
        m_Rigidbody.AddTorque(m_Direction * m_Speed * 100f);
        StartCoroutine(WaitForSecondsThenDie());

    }

    //// Update is called once per frame
    //public void Tick()
    //{
    //    m_Rigidbody.MovePosition(m_Rigidbody.position + (m_Direction * m_Speed * Time.deltaTime));
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, m_Size);

        Gizmos.DrawRay(transform.position + new Vector3(0.0f, 1.0f, 0.0f), m_Direction);
    }
    private void OnTriggerEnter(Collider other)
    {
        int l = (m_LayerMask & (1 << other.transform.gameObject.layer));
        //Only deal damage if we're on supported layers; Set this up for no friendly fire!
        if (l != 0)
        {
            Debug.Log("Layer Intersection from " + LayerMask.LayerToName(gameObject.layer) + "with " + LayerMask.LayerToName(other.gameObject.layer));
            IDamageable dmg = other.GetComponentInParent<IDamageable>();//GetComponentInParent checks the current object as well
            if (dmg == null) return;

            Debug.Log("Projectile Hit!" + other.name);
            dmg.TakeDamage(m_Damage);
            if (m_Health == 0)
                Destroy(gameObject);
            else 
                m_Health--;
        }
    }

    private IEnumerator WaitForSecondsThenDie()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}
