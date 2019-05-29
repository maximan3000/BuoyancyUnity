using System.Collections.Generic;
using Buoyancy.Struct;

namespace Buoyancy.Struct
{
    public class HullTriangles
    {
        public List<Triangle> underwater = new List<Triangle>();
        public List<Triangle> abovewater = new List<Triangle>();
    }
}
