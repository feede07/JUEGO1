using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class InvisibleBoundary : MonoBehaviour
{
    [SerializeField] private Color sceneColor = new(1f, 0.15f, 0.1f, 0.85f);

    private void Reset()
    {
        BoxCollider boundaryCollider = GetComponent<BoxCollider>();
        boundaryCollider.isTrigger = false;
        gameObject.layer = 0;
    }

    private void OnDrawGizmosSelected()
    {
        BoxCollider boundaryCollider = GetComponent<BoxCollider>();
        if (boundaryCollider == null)
        {
            return;
        }

        Matrix4x4 previousMatrix = Gizmos.matrix;
        Color previousColor = Gizmos.color;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = sceneColor;
        Gizmos.DrawWireCube(boundaryCollider.center, boundaryCollider.size);

        Gizmos.matrix = previousMatrix;
        Gizmos.color = previousColor;
    }
}
