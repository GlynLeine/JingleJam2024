using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField]
    private GameObject m_enemyPrefab;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            Instantiate(m_enemyPrefab, transform.position, Quaternion.identity);
        }
    }
}
