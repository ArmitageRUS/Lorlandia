using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lorlandia.Physics
{
    public enum Intersection { Front, Inside, Behind, None };
    class Collision
    {
        

        const float inaccuracy = 0.999f;

        public Intersection ClassifySphere(Vector3[] poligon, Vector3 sphere_centre, float radius)
        {
            Intersection intersectResult = Intersection.None;
            if (poligon!=null && poligon.Length > 2)
            {
                Vector3 vA = poligon[2] - poligon[0];
                Vector3 vB = poligon[1] - poligon[0];
                Vector3 poligon_normal = Vector3.Cross(vA, vB);
                poligon_normal.Normalize();
                float distance = -(poligon_normal.X * poligon[0].X + poligon_normal.Y * poligon[0].Y + poligon_normal.Z * poligon[0].Z);
                float sphere_offset = poligon_normal.X * sphere_centre.X + poligon_normal.Y * sphere_centre.Y + poligon_normal.Z * sphere_centre.Z + distance;
                if (Math.Abs(sphere_offset) < radius)
                {
                    intersectResult = Intersection.Inside;
                    Vector3 inspectedPoint = sphere_centre - poligon_normal * sphere_offset;
                    intersectResult = InsidePoligon(inspectedPoint, poligon) ? Intersection.Inside : Intersection.None;
                    if (intersectResult == Intersection.None) intersectResult = EdgePoligonCollision(sphere_centre, poligon, radius) ? Intersection.Inside : Intersection.None;
                }
                else if (sphere_offset >= radius) intersectResult = Intersection.Front;
                else if (sphere_offset < radius) intersectResult = Intersection.Behind;
            }
            return intersectResult;
        }

        private bool InsidePoligon(Vector3 inspectedPoint, Vector3[] poligon)
        {
            int vertex_count = poligon.Length;
            double angle = 0.0f;
            for (int i = 0; i < vertex_count; i++)
            {
                Vector3 vA = poligon[i] - inspectedPoint;
                Vector3 vB = poligon[(i + 1) % vertex_count] - inspectedPoint;
                vA.Normalize();
                vB.Normalize();
                float dot_product = Vector3.Dot(vA, vB);
                double i_angle = Math.Acos(dot_product);
                angle += i_angle;
            }
            if (angle >= MathHelper.TwoPi * inaccuracy) return true;
            return false;
        }

        private bool EdgePoligonCollision(Vector3 center, Vector3[] poligon, float radius)
        {
            bool return_value = false;
            int vertex_count = poligon.Length;
            for (int i = 0; i < vertex_count; i++)
            {
                Vector3 vPoint = ClosestPointOnEdge(poligon[i], poligon[(i + 1) % vertex_count], center);
                if ((center - vPoint).Length() < radius)
                {
                    return_value = true;
                    break;
                }
            }
            return return_value;
        }

        private Vector3 ClosestPointOnEdge(Vector3 vA, Vector3 vB, Vector3 center)
        {
            Vector3 vAB = vB - vA;
            float edge_length =vAB.Length();
            vAB.Normalize();
            Vector3 vAC = center - vA;
            float projection_length = Vector3.Dot(vAB, vAC);
            if (projection_length < 0) return vA;
            else if (projection_length > edge_length) return vB;
            else
            {
                vAB *= projection_length;
                return vA + vAB;
            }
        }
    }
}
