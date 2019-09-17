/* 
Copyright 2019 Maksim Petrov <grayen.job@gmail.com>

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;
using UnityEditor;

namespace Demo.BoatEngine
{
    public class BoatEngine : MonoBehaviour
    {
        public Transform engine;
        public Transform rudder;
        public float forceMax;
        public float forceIncrease;
        public float forceDamping;

        private float forceCurrent = 0f;
        private float ForceCurrent
        {
            get
            {
                return forceCurrent;
            }
            set
            {
                if (Mathf.Abs(value) <= forceMax)
                    forceCurrent = value;
            }
        }
        private float angle;
        private const float animationSpeed = 3000;
        private Rigidbody rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            AnimateEngine();
            AnimateRudder();
        }

        private void FixedUpdate()
        {
            rb.AddForceAtPosition(Quaternion.Euler(0, angle, 0) * engine.forward * ForceCurrent, engine.position);
            Damping();
        }

        public void MoveForward()
        {
            ForceCurrent += forceIncrease;
        }

        public void MoveBackward()
        {
            ForceCurrent -= forceIncrease;
        }

        public void TurnRight()
        {
            angle -= 0.9F;
            angle = Mathf.Clamp(angle, -90F, 90F);
        }

        public void TurnLeft()
        {
            angle += 0.9F;
            angle = Mathf.Clamp(angle, -90F, 90F);
        }

        private void AnimateEngine()
        {
            float rotateAngle = (Mathf.Abs(ForceCurrent) / forceMax) * animationSpeed * Time.deltaTime;
            engine.localRotation = Quaternion.Euler(engine.localRotation.eulerAngles + new Vector3(0, 0, -rotateAngle));
        }

        private void AnimateRudder()
        {
            angle = Mathf.Lerp(angle, 0.0F, 0.02F);
            rudder.localRotation = Quaternion.Euler(0, angle, 0);
        }

        private void Damping()
        {
            float preDampedForce = Mathf.Abs(ForceCurrent) - forceDamping;
            ForceCurrent = Mathf.Sign(ForceCurrent) * preDampedForce;
        }
    }
}
