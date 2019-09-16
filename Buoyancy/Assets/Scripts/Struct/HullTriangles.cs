using System.Collections.Generic;
using Buoyancy.Struct;

namespace Buoyancy.Struct
{
    /// <summary>
    /// Type that contains triangles prepared for physics calculations. 
    /// "Prepared" means that the triangles were divided (also were cutted) 
    /// by waterline on under water triangles and abovewater.
    /// There are different forces apply to them. 
    /// </summary>
    /// <see cref="Math.HullMath"/>
    public class HullTriangles
    {
        public List<Triangle> underwater = new List<Triangle>();
        public List<Triangle> abovewater = new List<Triangle>();
    }
}
