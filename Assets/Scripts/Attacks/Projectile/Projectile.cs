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

        Tick();
    }

    // Update is called once per frame
    public void Tick()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + (m_Direction * m_Speed * Time.deltaTime));
        m_DistanceTravelled += m_Speed * Time.deltaTime; 
        if(m_DistanceTravelled > m_Range)
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
        Debug.Log("Projectile Hit!");
    }
}
