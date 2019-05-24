using UnityEngine;
using System.Collections;

public class Orbital : MonoBehaviour
{
  public Transform target;
  Vector3 lastPosition;
  Vector3 direction;
  float distance;

  Vector3 movement;
  Vector3 rotation;

	void Awake ()
  {
    direction = new Vector3(0, 0, (target.position - transform.position).magnitude);
    transform.SetParent(target);
    lastPosition = Input.mousePosition;

  }
	
	void Update ()
  {
    Vector3 mouseDelta = Input.mousePosition - lastPosition;
    if (Input.GetMouseButton(0))
      movement += new Vector3(mouseDelta.x * 0.1f, mouseDelta.y * 0.05f, 0F);
    movement.z += Input.GetAxis("Mouse ScrollWheel") * -2.5F;

    rotation += movement;
    rotation.x = rotation.x % 360.0f;
    rotation.y = Mathf.Clamp(rotation.y, -80F, -10F);

    //direction.z = Mathf.Clamp(direction.z - movement.z * 50F, 15F, 180F);
    direction.z = Mathf.Clamp(movement.z + direction.z, 15F, 100F);
    transform.position = target.position + Quaternion.Euler(180F - rotation.y, rotation.x, 0) * direction;
    transform.LookAt(target.position);

    lastPosition = Input.mousePosition;
    movement *= 0.9F;


  }
}
