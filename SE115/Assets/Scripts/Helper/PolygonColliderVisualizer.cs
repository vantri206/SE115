using UnityEngine;

public class PolygonColliderVisualizer : MonoBehaviour
{
    public Color gizmoColor = Color.red;
    public bool showOnlyWhenSelected = false;

    private PolygonCollider2D poly;

    private void OnDrawGizmos()
    {
        if (showOnlyWhenSelected) return;
        DrawPoly();
    }

    private void OnDrawGizmosSelected()
    {
        if (!showOnlyWhenSelected) return;
        DrawPoly();
    }

    private void DrawPoly()
    {
        if (poly == null) poly = GetComponent<PolygonCollider2D>();
        if (poly == null) return;

        Gizmos.color = gizmoColor;

        Vector2[] points = poly.points;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 p1 = transform.TransformPoint(points[i]);
            Vector3 p2 = transform.TransformPoint(points[(i + 1) % points.Length]);

            Gizmos.DrawLine(p1, p2);
        }
    }
}