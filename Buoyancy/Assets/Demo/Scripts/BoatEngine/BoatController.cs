using UnityEngine;
using UnityEditor;

namespace Demo.BoatEngine
{
    public class BoatController : MonoBehaviour
    {
        private BoatEngine engine = null;

        void Awake()
        {
            engine = GetComponent<BoatEngine>();
        }

        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.W))
                engine.MoveForward();
            if (Input.GetKey(KeyCode.S))
                engine.MoveBackward();
            if (Input.GetKey(KeyCode.A))
                engine.TurnLeft();
            if (Input.GetKey(KeyCode.D))
                engine.TurnRight();
        }
    }
}

