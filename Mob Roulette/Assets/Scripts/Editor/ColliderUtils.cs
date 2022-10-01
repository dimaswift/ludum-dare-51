using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ColliderUtils 
{
    [MenuItem("CONTEXT/BoxCollider2D/Snap to mesh")]
    public static void SnapBoxCollider(MenuCommand command)
    {
        var collider = command.context as BoxCollider2D;
        var mesh = collider.GetComponentInChildren<MeshRenderer>();
        if (mesh == null)
        {
            Debug.LogWarning("No mesh found in children");
            return;
        }

        collider.size = mesh.bounds.size;
        collider.offset = mesh.transform.localPosition;
    }
}
