using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] public Vector3 m_Origin;
    [SerializeField] public Vector3 m_Direction;
    [SerializeField] public float m_Speed;
    [SerializeField] public float m_Size;
    [SerializeField] public int m_Damage;
    [SerializeField] public float m_Range;
    private float m_DistanceTravelled; 

    private Rigidbody m_Rigidbody;
    private CapsuleCollider m_CapsuleCollider; 

    public Projectile(Vector3 origin, Vector3 direction, float speed, float size, float range, int damage)
    {
        m_Origin = origin;
        m_Direction = direction; 
        m_Speed = speed;
        m_Size = size;
        m_Range = range;
        m_Damage = damage;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Rigidbody = this.GetComponent<Rigidbody>();
        m_Rigidbody.isKinematic = true;
        m_Rigidbody.useGravity = false;
        m_Rigidbody.position = m_Origin; 
        
        m_CapsuleCollider = this.GetComponent<CapsuleCollider>();
        m_CapsuleCollider.radius = m_Size;
        m_CapsuleCollider.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + (m_Direction * m_Speed * Time.deltaTime));
        m_DistanceTravelled += m_Speed * Time.deltaTime; 
        if(m_DistanceTravelled > m_Range)
        {
            GameObject.Destroy(this.gameObject);
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
        //Destroy(this.gameObject); 
    }
}
