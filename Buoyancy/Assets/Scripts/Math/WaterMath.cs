using System.Collections.Generic;
using UnityEngine;
using Demo.Ocean;

namespace Buoyancy.Math
{
    public static class WaterMath 
    {
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
            int key = (int)(point.y * heightMapKeyInterval);
            return key;
        }
    }
}

