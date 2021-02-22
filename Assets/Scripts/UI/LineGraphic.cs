using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AORTechTreeItem))]
public class LineGraphic : MaskableGraphic
{
    public float thickness;

    public List<dependedGraph> corners;
    public List<Vector2> points;

    [System.Serializable]
    public struct dependedGraph
    {
        public RectTransform root;

        public RectTransform selfInput;
        public RectTransform dependency;
        public List<Vector2> points;


    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (corners != null)
        {
            points = new List<Vector2>();
            for (int i1 = 0; i1 < corners.Count; i1++)
            {
                var iV2 = (Vector2)corners[i1].selfInput.localPosition;
                var oV2 = (Vector2)corners[i1].root.InverseTransformPoint(corners[i1].dependency.TransformPoint(corners[i1].dependency.anchoredPosition));
                //oV2.y -= (i1 * (thickness + 50f));
                var vv = new Vector2(iV2.x, oV2.y- ((thickness + 100f)));
                points.Add(iV2);
                points.Add(vv);
                points.Add(oV2);
            }
            float angle = 0;
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 point = points[i];

                if (i < points.Count - 1)
                {
                    angle = GetAngle(point, points[i + 1]) + 45f;



                }
                if (i+2<points.Count &&(i+1)%3 == 0 &&points[i].x < points[i+2].x)
                {
                    angle += 90f;
                    if (i == 0)
                    {
                        angle -= 45f;
                    }
                }
                else
                {
                    angle -= 180f;
                    if (i == 0)


                        angle += 45f;
                }

                if (i == points.Count - 1) angle -= 135f;


                DrawVertsForPoint(point, vh, angle);
            }
            for (int i = 0; i < points.Count - 1; i++)
            {
                int index = i * 2;
                if ((i+1) % 3 == 0) index += 4;
                vh.AddTriangle(index + 0, index + 1, index + 3);
                vh.AddTriangle(index + 3, index + 2, index + 0);
            }
        }
    }

    public float GetAngle(Vector2 me, Vector2 target)
    {
        return (float)(Mathf.Atan2(target.y - me.y, target.x - me.x) * (180 / Mathf.PI));
    }

    void DrawVertsForPoint(Vector2 point, VertexHelper vh, float angle)
    {
        UIVertex vert = UIVertex.simpleVert;

        vert.color = color;


        vert.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vert.position += (Vector3)point;
        vh.AddVert(vert);
        vert.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vert.position += (Vector3)point;

        vh.AddVert(vert);
    }
}
