using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class GameEntity : MonoBehaviour
{
    protected Rigidbody rb;

    public Vector3 centerOfMassOffset = new Vector3(0F, 0F, 0F);
    Vector3 S_centerOfMass;

    public Vector3 speed { get; private set; }
    public float absSpeed { get; private set; }
    public float sqrtSpeed { get; private set; }
    Vector3 last_position;

    public float totalMass { get; private set; }


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();

        S_centerOfMass = rb.centerOfMass;
        last_position = transform.position;

        totalMass = GetTotalMass(transform);
    }

    private static float GetTotalMass(Transform t)
    {
        float mass = 0F;
        Rigidbody rb = t.GetComponent<Rigidbody>();
        if(rb != null)
            mass += rb.mass;
        for(int i = 0 ; i < t.childCount ; i++)
            mass += GetTotalMass(t.GetChild(i));
        return mass;
        
    }


    protected virtual void FixedUpdate()
    {
#if UNITY_EDITOR

        if (rb.centerOfMass != S_centerOfMass + centerOfMassOffset)
            rb.centerOfMass = S_centerOfMass + centerOfMassOffset;
#endif

        speed = (transform.position - last_position) / Time.deltaTime;
        last_position = transform.position;
        absSpeed = speed.x < 0F ? -speed.x : speed.x
                 + speed.y < 0F ? -speed.y : speed.y
                 + speed.z < 0F ? -speed.z : speed.z;
        if(absSpeed < 0)
            absSpeed = -absSpeed;
        sqrtSpeed = Mathf.Sqrt(absSpeed);
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        if(rb == null)
            return;
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(rb.worldCenterOfMass, 0.25F);
    }
#endif
}
