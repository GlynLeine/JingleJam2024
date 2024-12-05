using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Arc : MonoBehaviour
{

    public Transform Target;
    public int Damage;
    public float Range;
    public VisualEffect Vfx;

    private Vector3 Origin;
    private Vector3 Position1;
    private Vector3 Position2;
    private Vector3 TargetPosition;

    public void Initialize(Ability_Arc ability)
    {
        Vfx = GetComponent<VisualEffect>();
        Tick();

        var enemy = Target.GetComponent<IDamageable>();
        if (enemy != null)
        {
            enemy.TakeDamage();
        }

        StartCoroutine(WaitThenDestroy(ability.castTime));
    }

    public void Tick()
    {
        Origin = Vector3.zero;
        TargetPosition = Target.position;
        TargetPosition -= transform.position;
        var dir = TargetPosition - Origin;
        Position1 = Vector3.Lerp(Origin, Origin + dir, .33f);
        Position2 = Vector3.Lerp(Origin, Origin + dir, .66f);

        Vfx.SetVector3("Position 0", Origin);
        Vfx.SetVector3("Position 1", Position1);
        Vfx.SetVector3("Position 2", Position2);
        Vfx.SetVector3("Position 3", TargetPosition);
    }

    private IEnumerator WaitThenDestroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        var radius = 0.3f; 
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Origin, radius);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(Position1, radius);
        Gizmos.DrawSphere(Position2, radius);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(TargetPosition, radius);
    }
}
