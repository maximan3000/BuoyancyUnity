using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Demo.Ocean;
using Buoyancy.Struct;

namespace Buoyancy.Math
{
    public class WaterMath 
    {
        /* Ключ к словарю берется из координаты Y точки, 
         * поэтому нужен коэффициент, который какие точки (какие Y координаты) будут иметь одинаковый ключ*/
        public static float heightMapKeyInterval = 1000f;
        public static Dictionary<int, float> casheHeightMap = new Dictionary<int, float>();

        public static float DistanceToWater(Vector3 point)
        {
            int key = GetHeightMapKey(point);
            if (!casheHeightMap.ContainsKey(key))
            {
                float waterHeight = OceanAdvanced.GetWaterHeight(point);
                float distanceToWater = point.y - waterHeight;
                casheHeightMap.Add(key, distanceToWater);
            }

            return casheHeightMap[key];
        }

        private static int GetHeightMapKey(Vector3 point)
        {
            return (int)(point.y * heightMapKeyInterval);
        }
    }
}

