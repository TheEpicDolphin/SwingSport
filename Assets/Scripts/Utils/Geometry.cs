using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeometryUtils
{
    public class Geometry
    {
        internal static Vector3[] LineSegmentSphereIntersection(Vector3 s, Vector3 e, Vector3 center, float r)
        {
            Vector3 d = e - s;
            Vector3 f = s - center;
            /*
             * f(t) = s + t * (e - s) 
             * ||g(t) - center||^2 = r^2
             * t^2 * dot(d, d) + 2t * dot(f, d) + dot(f, f) - r^2 = 0
             */
            float a = Vector3.Dot(d, d);
            float b = 2 * Vector3.Dot(f, d);
            float c = Vector3.Dot(f, f) - r * r;

            float discriminant = b * b - 4 * a * c;
            if(discriminant >= 0)
            {
                discriminant = Mathf.Sqrt(discriminant);
                // t1 is smaller value
                float t1 = (-b - discriminant) / (2 * a);
                float t2 = (-b + discriminant) / (2 * a);
                List<Vector3> intersections = new List<Vector3>();
                if(t1 >= 0 && t1 <= 1)
                {
                    intersections.Add(s + t1 * d);
                }
                if (t2 >= 0 && t2 <= 1)
                {
                    intersections.Add(s + t2 * d);
                }
                return intersections.ToArray();
            }
            return new Vector3[0];
        }

        internal static bool RaySphereExitIntersection(Ray ray, Vector3 center, float r, out Vector3 intersection)
        {
            Vector3 f = ray.origin - center;
            /*
             * f(t) = origin + t * direction 
             * ||g(t) - center||^2 = r^2
             * t^2 * dot(direction, direction) + 2t * dot(f, direction) + dot(f, f) - r^2 = 0
             */
            float a = Vector3.Dot(ray.direction, ray.direction);
            float b = 2 * Vector3.Dot(f, ray.direction);
            float c = Vector3.Dot(f, f) - r * r;

            float discriminant = b * b - 4 * a * c;
            if (discriminant >= 0)
            {
                float t = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
                if(t >= 0)
                {
                    intersection = ray.GetPoint(t);
                    return true;
                }
            }
            intersection = Vector3.zero;
            return false;
        }

    }
}