using System.Collections.Generic;
using UnityEngine;

public class CameraOcclusion : MonoBehaviour
{
    public Transform target;

    public List<Renderer> renderers = new List<Renderer>();

    void Update()
    {
        var direction = target.position - transform.position;
        var distance = direction.magnitude;
        var hits = Physics.RaycastAll(transform.position, direction, distance);

        foreach (var rend in renderers)
            rend.material.SetFloat("_Alpha", 1f);
        renderers.Clear();

        foreach (var hit in hits)
        {
            var rend = hit.collider.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                rend.material.SetFloat("_Alpha", 0.5f);
                renderers.Add(rend);
            }
        }
    }
}
