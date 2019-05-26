using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Buoyancy.Struct;
using Buoyancy.Math;

namespace Buoyancy.Physics
{
    class ResistanceForces
    {
        //public static float r = 1000f;
        //public static float g = 9.8f;

        //private Rigidbody rb;
        //private Triangle triangle;
        //private Vector3 center;

        //public ResistanceForces(Triangle triangle, Rigidbody rb)
        //{
        //    this.triangle = triangle;
        //    this.rb = rb;
        //}

        //public static void ApplyForce(Triangle triangle, Rigidbody rb)
        //{
        //    new ResistanceForces(triangle, rb).ApplyForce();
        //}

        //private void ApplyForce()
        //{
        //    var force = MakeForce();
        //    this.center = TriangleMath.GetCenter(triangle);
        //    rb.AddForceAtPosition(force, center);
        //    //Debug.DrawRay(center, TriangleMath.GetNormal(triangle), Color.white);
        //}

        //private Vector3 MakeForce()
        //{
        //    var velocity = rb.velocity;
        //    var normal = TriangleMath.GetNormal(triangle);
        //    Vector3 direction = -Vector3.ProjectOnPlane(velocity.normalized, normal);
        //    float dS = TriangleMath.GetSquare(triangle);
        //    float V = velocity.magnitude;

        //    float magnitude = (r * V * V * dS * Cf) / 2;
        //    return direction * Mathf.Abs(magnitude);
        //}
    }
}
