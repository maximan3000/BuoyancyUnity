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
