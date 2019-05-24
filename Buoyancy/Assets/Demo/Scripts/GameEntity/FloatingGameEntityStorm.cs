using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ArchimedsLab;


[RequireComponent(typeof(MeshFilter))]
public class FloatingGameEntityStorm : GameEntity
{
    public Mesh buoyancyMesh;

    /* These 4 arrays are cache array, preventing some operations to be done each frame. */
    tri[] _triangles;
    tri[] worldBuffer;
    tri[] wetTris;
    tri[] dryTris;
    //These two variables will store the number of valid triangles in each cache arrays. They are different from array.Length !
    uint nbrWet, nbrDry;

    protected override void Awake()
    {
        base.Awake();

        //By default, this script will take the render mesh to compute forces. You can override it, using a simpler mesh.
        Mesh m = buoyancyMesh == null ? GetComponent<MeshFilter>().mesh : buoyancyMesh;

        //Setting up the cache for the game. Here we use variables with a game-long lifetime.
        WaterCutter.CookCache(m, ref _triangles, ref worldBuffer, ref wetTris, ref dryTris);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(rb.IsSleeping())
            return;
        /* It's strongly advised to call these in the FixedUpdate function to prevent some weird behaviors */

        //This will prepare static cache, modifying vertices using rotation and position offset.
        WaterCutter.CookMesh(transform.position, transform.rotation, ref _triangles, ref worldBuffer);
        
        /*
            Now mesh ae reprensented in World position, we can split the mesh, and split tris that are partially submerged.
            Here I use a very simple water model, already implemented in the DLL.
            You can give your own. See the example in Examples/CustomWater.
        */
        WaterCutter.SplitMesh(worldBuffer, ref wetTris, ref dryTris, out nbrWet, out nbrDry, WaterSurface.simpleWater);
        //This function will compute the forces depending on the triangles generated before.
        Archimeds.ComputeAllForces(wetTris, dryTris, nbrWet, nbrDry, speed, rb);
    }
   
#if UNITY_EDITOR
    //Some visualizations for this buyoancy script.
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if(!Application.isPlaying)
            return;

        Gizmos.color = Color.blue;
        for(uint i = 0 ; i < nbrWet ; i++)
        {
            Gizmos.DrawLine(wetTris[i].a, wetTris[i].b);
            Gizmos.DrawLine(wetTris[i].b, wetTris[i].c);
            Gizmos.DrawLine(wetTris[i].a, wetTris[i].c);
        }
        
        Gizmos.color = Color.yellow;
        for (uint i = 0; i < nbrDry; i++)
        {
            Gizmos.DrawLine(dryTris[i].a, dryTris[i].b);
            Gizmos.DrawLine(dryTris[i].b, dryTris[i].c);
            Gizmos.DrawLine(dryTris[i].a, dryTris[i].c);
        }
    }
#endif
}
