using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GPL
{
    public class WallGravity : MonoBehaviour
    {
        public Transform gravityDirection;

        public Vector3 GetGravityDirection
        {
            get { return gravityDirection.position.normalized; }
        }

        private void OnDrawGizmosSelected()
        {
            DrawArrow(gravityDirection.position, gravityDirection.position.normalized,Color.blue);
        }

        private void DrawArrow(Vector2 from, Vector2 to, Color color)
        {
            Handles.BeginGUI();
            Handles.color = color;
            Handles.DrawAAPolyLine(3, from, to);
            Vector2 v0 = from - to;
            v0 *= 10 / v0.magnitude;
            Vector2 v1 = new Vector2(v0.x * 0.866f - v0.y * 0.5f, v0.x * 0.5f + v0.y * 0.866f);
            Vector2 v2 = new Vector2(v0.x * 0.866f + v0.y * 0.5f, v0.x * -0.5f + v0.y * 0.866f); ;
            Handles.DrawAAPolyLine(3, to + v1, to, to + v2);
            Handles.EndGUI();
        }
    }
}
