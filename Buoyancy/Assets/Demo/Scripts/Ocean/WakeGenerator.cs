using UnityEngine;
using System.Collections;
using Demo.Ocean;

namespace Demo.Ocean
{
    public class WakeGenerator : MonoBehaviour
    {
        public Vector3 offset;

        private OceanAdvanced ocean;
        private Vector3 last_position;
        private float speed;

        void Awake()
        {
            ocean = FindObjectOfType<OceanAdvanced>();
            speed = 0.0F;
            last_position = transform.position;
        }

        void Update()
        {
            speed = (transform.position - last_position).magnitude / Time.deltaTime;
            last_position = transform.position;
            if (Time.time % 0.2F < 0.01F)
            {
                Vector3 p = transform.position + transform.rotation * offset;
                if (OceanAdvanced.GetWaterHeight(p) > p.y)
                    ocean.RegisterInteraction(p, Mathf.Clamp01(speed / 15.0F) * 0.5F);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + transform.rotation * offset, 0.5F);
        }
    }
}

