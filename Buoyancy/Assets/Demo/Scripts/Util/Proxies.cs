using UnityEngine;
using System.Collections;

namespace Demo.Util
{
    public class Proxies : MonoBehaviour
    {
        void Awake()
        {
            Destroy(GetComponent<MeshRenderer>());
            Destroy(GetComponent<MeshFilter>());
            Destroy(this);
        }
    }
}

