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
    [HideInInspector]
    public float m_Range;
    private float m_DistanceTravelled;
    public VisualEffect vfx;

    private Rigidbody m_Rigidbody;
    private CapsuleCollider m_CapsuleCollider;

    public LayerMask m_LayerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.isKinematic = true;
        m_Rigidbody.useGravity = false;
        m_Rigidbody.position = m_Origin;

        m_CapsuleCollider = GetComponent<CapsuleCollider>();
        m_CapsuleCollider.radius = m_Size;
        m_CapsuleCollider.isTrigger = true;

    }

    // Update is called once per frame
    public void Tick()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + (m_Direction * m_Speed * Time.deltaTime));
        m_DistanceTravelled += m_Speed * Time.deltaTime;
        if (m_DistanceTravelled >= m_Range)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position, m_Size);

        Gizmos.DrawRay(this.transform.position + new Vector3(0.0f, 1.0f, 0.0f), m_Direction);
    }
    private void OnTriggerEnter(Collider other)
    {
        int l = (m_LayerMask & (1 << other.transform.gameObject.layer));
        //Only deal damage if we're on supported layers; Set this up for no friendly fire!
        if (l != 0)
        {
            Debug.Log("Layer Intersection from " + LayerMask.LayerToName(this.gameObject.layer) + "with " + LayerMask.LayerToName(other.gameObject.layer));
            IDamageable dmg = other.GetComponent<IDamageable>();
            if (dmg == null)
            {
                //Search the Parent object...
                dmg = other.GetComponentInParent<IDamageable>();
                if(dmg == null)
                {
                    return;
                }
            }
            {
                Debug.Log("Projectile Hit!" + other.name);
                dmg.TakeDamage(m_Damage);
            }
        }
    }
}
