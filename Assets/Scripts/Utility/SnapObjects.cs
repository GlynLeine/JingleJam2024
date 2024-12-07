using UnityEngine;

public class SnapObjects : MonoBehaviour
{
    public bool snap;

    void OnValidate()
    {
        foreach (Transform child in transform)
        {
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(child.transform.position, -transform.up, out hit, Mathf.Infinity))
            {
                child.transform.position = hit.point;
            }
        }


        snap = false;
    }
}
