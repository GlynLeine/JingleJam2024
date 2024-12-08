using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class AOE : MonoBehaviour
{
    public int Damage;
    public float Range;
    public VisualEffect Vfx;

    private Rigidbody m_Rigidbody;
    private CapsuleCollider m_CapsuleCollider;

    public LayerMask m_LayerMask;


    public void Initialize(Ability_AOE ability)
    {
        Vfx.enabled = true;
        Vfx.Play();
        /*
        Transform Target = transform; //Note: This won't work; Use RigidBodies / Overlap triggers instead??
        var enemy = Target.GetComponent<IDamageable>(); 
        if (enemy != null)
        {
            enemy.TakeDamage(ability.Damage);
        }

        GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(WaitThenDestroy(1f));
        */

        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.useGravity = false;
        m_Rigidbody.position = transform.position;

        m_CapsuleCollider = GetComponent<CapsuleCollider>();
        m_CapsuleCollider.radius = Range;
        m_CapsuleCollider.isTrigger = true;
        StartCoroutine(WaitThenDestroy(0.5f));
    }

    private IEnumerator WaitThenDestroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position, Range);
    }

    private void OnTriggerEnter(Collider other)
    {
        int l = (m_LayerMask & (1 << other.transform.gameObject.layer));
        //Only deal damage if we're on supported layers; Set this up for no friendly fire!
        if (l != 0)
        {
            Debug.Log("Layer Intersection from " + LayerMask.LayerToName(this.gameObject.layer) + "with " + LayerMask.LayerToName(other.gameObject.layer));
            IDamageable dmg = other.GetComponentInParent<IDamageable>();//GetComponentInParent checks the current object as well
            if (dmg == null) return;

            Debug.Log("Projectile Hit!" + other.name);
            dmg.TakeDamage(Damage);
        }
    }

}
