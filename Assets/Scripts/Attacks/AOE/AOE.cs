using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class AOE : MonoBehaviour
{
    public int Damage;
    public float Range;
    public VisualEffect Vfx;

    public void Initialize(Ability_AOE ability)
    {
        Vfx.enabled = true;
        Vfx.Play();
        Transform Target = transform;
        var enemy = Target.GetComponent<IDamageable>();
        if (enemy != null)
        {
            enemy.TakeDamage();
        }

        GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(WaitThenDestroy(1f));
    }

    private IEnumerator WaitThenDestroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
