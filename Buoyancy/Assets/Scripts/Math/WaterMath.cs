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

using System.Collections.Generic;
using UnityEngine;
using Demo.Ocean;
using System;

namespace Buoyancy.Math
{
    /// <summary>
    /// Calculations with water.
    /// </summary>
    public static class WaterMath 
    {
        /// <summary>
        /// Parameter for cache that shows interval of Y coordinate between two points
        /// in cache array.
        /// </summary>
        /// <example>
        /// If two points have less than <c>heightMapKeyInterval</c> Y - coordinate
        /// then 2nd point will not use outer function of depth and distance to water
        /// of 2nd point will be same with 1st
        /// </example>
        public static float heightMapKeyInterval = 1000f;
        /// <summary>
        /// Cache of calls for function that calculates depth of any point
        /// </summary>
        public static Dictionary<int, float> caсheHeightMap = new Dictionary<int, float>();
        /// <summary>
        /// Outer function of depth - get distance point in params to waterline
        /// </summary>
        public static Func<Vector3, float> GetWaterHeight;

        public static float DistanceToWater(Vector3 point)
        {
            int key = GetHeightMapKey(point);
            if (!caсheHeightMap.ContainsKey(key))
            {
                float waterHeight = GetWaterHeight(point);
                float distanceToWater = point.y - waterHeight;
                caсheHeightMap.Add(key, distanceToWater);
            }
            return caсheHeightMap[key];
        }

        private static int GetHeightMapKey(Vector3 point)
        {
            int key = (int)(point.y * heightMapKeyInterval);
            return key;
        }
    }
}

