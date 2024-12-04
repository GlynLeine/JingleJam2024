using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    public float Health = 100f;
    public float Damage = 10f;
    public float MoveSpeed = 5f;
}
