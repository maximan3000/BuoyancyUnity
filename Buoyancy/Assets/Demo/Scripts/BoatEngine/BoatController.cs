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

