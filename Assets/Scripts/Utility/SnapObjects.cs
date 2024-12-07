using System.Collections.Specialized;
using UnityEngine;

public class SnapObjects : MonoBehaviour
{
    public bool snap;

    void OnValidate()
    {
        foreach (Transform child in transform)
        {
            RaycastHit hit = new RaycastHit();
            child.transform.position = new Vector3(child.transform.position.x, 70f, child.transform.position.z);
            if (Physics.Raycast(child.transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {
                child.transform.position = hit.point;
            }
        }


        snap = false;
    }
}
