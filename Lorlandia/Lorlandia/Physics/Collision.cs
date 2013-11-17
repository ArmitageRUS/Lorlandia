using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lorlandia.Physics
{
    class Collision
    {
        public enum Intersection { Front, Inside, Behind, Error };

        public Intersection ClassifySphere(Vector3[] poligon, Vector3 sphere_centre, float radius)
        {
            Intersection intersectResult = Intersection.Error;
            if(poligon.Length>2)
            {
                Vector3 vA = poligon[2] - poligon[0];
                Vector3 vB = poligon[1] - poligon[0];
                Vector3 poligon_normal = Vector3.Cross(vA, vB);
                poligon_normal.Normalize();
                float distance = -(poligon_normal.X * poligon[0].X + poligon_normal.Y * poligon[0].Y + poligon_normal.Z * poligon[0].Z);
                float sphere_offset = poligon_normal.X * sphere_centre.X + poligon_normal.Y * sphere_centre.Y + poligon_normal.Z * sphere_centre.Z + distance;
                if (Math.Abs(sphere_offset) < radius) intersectResult = Intersection.Inside;
                else if (sphere_offset >= radius) intersectResult = Intersection.Front;
                else if (sphere_offset < radius) intersectResult = Intersection.Behind;
            }
            return intersectResult;
        }

        
    }
}
