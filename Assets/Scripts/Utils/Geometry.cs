using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeometryUtils
{
    public class Geometry
    {
        internal static int LineSegmentCircleIntersection(Vector3 s, Vector3 e, Vector3 center, float r, out Vector3[] intersections)
        {
            intersections = new Vector3[2];
            /*
             * f(t) = s + t * (e - s) 
             * ||g(t) - center||^2 = r^2
             * t^2 * dot(d, d) + 2t * dot(f, d) + dot(f, f) - r^2 = 0
             */
            Vector3 d = e - s;
            Vector3 f = e - center;

            float a = Vector3.Dot(d, d);
            float b = 2 * Vector3.Dot(f, d);
            float c = Vector3.Dot(f, f) - r * r;

            float discriminant = b * b - 4 * a * c;
            if(discriminant >= 0)
            {
                discriminant = Mathf.Sqrt(discriminant);
                float t1 = (-b - discriminant) / (2 * a);
                float t2 = (-b + discriminant) / (2 * a);

                int count = 0;
                if(t1 >= 0 && t1 <= 1)
                {
                    intersections[count] = s + t1 * d;
                    count += 1;
                }
                if (t2 >= 0 && t2 <= 1)
                {
                    intersections[count] = s + t2 * d;
                    count += 1;
                }
                return count;
            }
            return 0;
        }
    }
}