using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using UnityEngine.UIElements;



public enum Side
{
    //Which side of the mesh needed to be specified 
    Pos = 0,
    Neg = 1
}
public class MeshUtil
{
    //Ref to meshes on either side of the cutting plane
    private Mesh posMesh;
    private Mesh negMesh;

    /*--------------------------------------------*/
    //Positive side of cutting plane mesh data
    private List<Vector3> posSideVerts; //Verticies 
    private List<Vector3> posSideNorm; //Normals
    private List<Vector2> posSideUV01; //UVs
    private List<int> posSideTris; //Triangles


    //Negative side of cutting plane mesh data
    private List<Vector3> negSideVerts; //Verticies 
    private List<Vector3> negSideNorm; //Normals
    private List<Vector2> negSideUV01; //UVs
    private List<int> negSideTris; //Triangles
    /*--------------------------------------------*/

    //Cutting plane data
    private readonly List<Vector3> pointsOnPlane;
    private Plane cuttingPlane;
    /*--------------------------------------------*/

    //new mesh
    private Mesh mesh;
    /*--------------------------------------------*/

    //Object variables
    private bool isConcaveShape; //Is the object solid or hollow
    private bool renderInsideFaces = false; // To render inside faces of hollow objects

    /*--------------------------------------------*/
    //Getters & Setters

    public bool IsConcaveShape
    {
        get { return isConcaveShape; }
        set { isConcaveShape = value; }
    }


    FunctionUtils func;

    public Mesh PosSideM
    {
        get
        {
            if (posMesh == null)
            {
                posMesh = new Mesh();
            }

            //TODO SetMeshData
            SetMesh(Side.Pos);

            return posMesh;
        }
    }

    public Mesh NegSideM
    {
        get
        {
            if (negMesh == null)
            {
                negMesh = new Mesh();
            }

            //TODO SetMeshData
            SetMesh(Side.Neg);
            return negMesh;
        }
    }

    /*--------------------------------------------*/


    private void SetMesh(Side side)
    {
        switch (side)
        {
            case Side.Pos:
                {
                    //Positive
                    posMesh.vertices = posSideVerts.ToArray();
                    posMesh.normals = posSideNorm.ToArray();
                    posMesh.uv = posSideUV01.ToArray();
                    posMesh.triangles = posSideTris.ToArray();
                    break;
                }
            case Side.Neg:
                {
                    //Negative
                    negMesh.vertices = negSideVerts.ToArray();
                    negMesh.normals = negSideNorm.ToArray();
                    negMesh.uv = negSideUV01.ToArray();
                    negMesh.triangles = negSideTris.ToArray();
                    break;
                }
        }
    }

    public MeshUtil(Plane p, Mesh m, bool isConvex, bool renderInside)
    {
        //Mesh Data
        cuttingPlane = p;
        mesh = m;
        isConcaveShape = isConvex;
        renderInsideFaces = renderInside;

        //Positive side of cutting plane mesh data
        posSideVerts = new List<Vector3>(); //Verticies 
        posSideNorm = new List<Vector3>(); //Normals
        posSideUV01 = new List<Vector2>(); //UVs
        posSideTris = new List<int>(); //Triangles

        //Negative side of cutting plane mesh data
        negSideVerts = new List<Vector3>(); //Verticies 
        negSideNorm = new List<Vector3>(); //Normals
        negSideUV01 = new List<Vector2>();//UVs
        negSideTris = new List<int>(); //Triangles

        pointsOnPlane = new List<Vector3>();

        func = new FunctionUtils(); ;

        CreateNewMeshObj();
    }

    private void CreateNewMeshObj()
    {
        //Checks for new meshes based on the intersection of the cutting plane against editible object
        Vector3[] tempVerts = mesh.vertices;
        Vector3[] tempNorm = mesh.normals;
        Vector2[] tempUVS = mesh.uv;
        int[] tempTris = mesh.triangles;

        int _t3 = 3; //triangles have 3 points 

        for (int i = 0; i < tempTris.Length; i += _t3)
        {
            //Order of each vert on the triangle
            //Vert0
            var _vert0 = tempVerts[tempTris[i]];
            var _vert0Array = Array.IndexOf(tempVerts, _vert0);
            var _normals0 = tempNorm[_vert0Array];
            var _uvs0 = tempUVS[_vert0Array];
            bool _v0 = cuttingPlane.GetSide(_vert0);

            //Vert1
            var _vert1 = tempVerts[tempTris[i + 1]];
            var _vert1Array = Array.IndexOf(tempVerts, _vert1);
            var _normals1 = tempNorm[_vert1Array];
            var _uvs1 = tempUVS[_vert1Array];
            bool _v1 = cuttingPlane.GetSide(_vert1);

            //Vert2
            var _vert2 = tempVerts[tempTris[i + 2]];
            var _vert2Array = Array.IndexOf(tempVerts, _vert2);
            var _normals2 = tempNorm[_vert2Array];
            var _uvs2 = tempUVS[_vert2Array];
            bool _v2 = cuttingPlane.GetSide(_vert2);


            if (_v0 == _v1 && _v1 == _v2) //Points are all on the same side of the cutting plane
            {

                Side _side = (_v0) ? Side.Pos : Side.Neg;
                CreateTriNorUV(_side,
                    _vert0, _normals0, _uvs0,
                    _vert1, _normals1, _uvs1,
                    _vert2, _normals2, _uvs2, true, false);

            }
            else
            {
                Vector3 cross0;
                Vector3 cross1;

                Vector3 crossUV0;
                Vector3 crossUV1;

                Side _side0 = (_v0) ? Side.Pos : Side.Neg;
                Side _side1 = (_v0) ? Side.Neg : Side.Pos;


                if (_v0 == _v1) //Verts0 & 1 are on the same side
                {
                    //TODO make into function

                    cross0 = InterPoint(_vert1, _uvs1, _vert2, _uvs2, out crossUV0);
                    cross1 = InterPoint(_vert2, _uvs2, _vert0, _uvs0, out crossUV1);

                    CreateTriNorUV(_side0, _vert0, null, _uvs0, _vert1, null, _uvs1, cross0, null, crossUV1, false, false);
                    CreateTriNorUV(_side0, _vert0, null, _uvs0, cross0, null, crossUV0, cross1, null, crossUV1, false, false);
                    CreateTriNorUV(_side1, cross0, null, crossUV0, _vert2, null, _uvs2, cross1, null, crossUV1, false, false);
                }
                else if (_v0 == _v2) //verts0 & 3 are on the same side
                {
                    cross0 = InterPoint(_vert0, _uvs0, _vert1, _uvs1, out crossUV0);
                    cross1 = InterPoint(_vert1, _uvs1, _vert2, _uvs2, out crossUV1);

                    CreateTriNorUV(_side0, _vert0, null, _uvs0, cross0, null, crossUV1, _vert2, null, _uvs2, false, false);
                    CreateTriNorUV(_side0, cross0, null, crossUV0, cross1, null, crossUV1, _vert2, null, _uvs2, false, false);
                    CreateTriNorUV(_side1, cross0, null, crossUV0, _vert1, null, _uvs1, cross1, null, crossUV1, false, false);

                }
                else //Vert0 is on its own
                {
                    cross0 = InterPoint(_vert0, _uvs0, _vert1, _uvs1, out crossUV0);
                    cross1 = InterPoint(_vert0, _uvs0, _vert2, _uvs2, out crossUV1);

                    CreateTriNorUV(_side0, _vert0, null, _uvs0, cross0, null, crossUV0, cross1, null, crossUV1, false, false);
                    CreateTriNorUV(_side1, cross0, null, crossUV0, _vert1, null, _uvs2, _vert2, null, _uvs2, false, false);
                    CreateTriNorUV(_side1, cross0, null, crossUV0, _vert2, null, _uvs2, cross1, null, crossUV1, false, false);
                }

                pointsOnPlane.Add(cross0);
                pointsOnPlane.Add(cross1);

            }
        }

        if (!IsConcaveShape)
        {
            JoinPoints();
        }
        else if (renderInsideFaces)
        {

        }
    }

    private void JoinPoints()
    {
        Vector3 h = GetHalfway(out float dist);

        for (var i = 0; i < pointsOnPlane.Count; i += 2)
        {
            Vector3 firstV;
            Vector3 nextV;

            firstV = pointsOnPlane[i];
            nextV = pointsOnPlane[i + 1];

            Vector3 normal = func.Normal(h, nextV, firstV);
            normal.Normalize();

            var dir = Vector3.Dot(normal, cuttingPlane.normal);

            if (dir > 0)
            {
                CreateTriNorUV(Side.Pos, h,
                    -normal, Vector3.zero, firstV,
                    -normal, Vector3.zero, nextV,
                    -normal, Vector3.zero,
                    false, true);

                CreateTriNorUV(Side.Neg, h,
                    normal, Vector3.zero, nextV,
                    normal, Vector3.zero, firstV,
                    normal, Vector3.zero,
                    false, true);
            }
            else
            {
                CreateTriNorUV(Side.Pos, h,
                    normal, Vector3.zero, nextV,
                    normal, Vector3.zero, firstV,
                    normal, Vector3.zero,
                    false, true);

                CreateTriNorUV(Side.Neg, h,
                    -normal, Vector3.zero, firstV,
                    -normal, Vector3.zero, nextV,
                    -normal, Vector3.zero,
                    false, true);
            }
        }
    }

    private Vector3 GetHalfway(out float dist)
    {
        if (pointsOnPlane.Count > 0)
        {
            Vector3 first = pointsOnPlane[0];
            Vector3 last = Vector3.zero;
            dist = 0.0f;

            foreach (Vector3 p in pointsOnPlane)
            {
                float currentDist = 0.0f;
                currentDist = Vector3.Distance(first, p);

                if (currentDist > dist)
                {
                    dist = currentDist;
                    last = p;
                }
            }
            return Vector3.Lerp(first, last, 0.5f);
        }
        else
        {
            dist = 0.0f;
            return Vector3.zero;
        }
    }

    private Vector3 InterPoint(
        Vector3 _vert0, Vector2 _vertUV0,
        Vector3 _vert1, Vector2 _vertUV1,
        out Vector3 UV)
    {
        float dist = func.GetDistFromPlane(_vert0, _vert1, cuttingPlane, out Vector3 pointOfIntersect);
        UV = func.InterpUvs(_vertUV0, _vertUV1, dist);
        return pointOfIntersect;
    }

    private void CreateTriNorUV(Side _side,
        Vector3 _vert0, Vector3? _norm0, Vector2 _uvs0,
        Vector3 _vert1, Vector3? _norm1, Vector2 _uvs1,
        Vector3 _vert2, Vector3? _norm2, Vector2 _uvs2,
       bool shared, bool first)
    {
        //shared = true;
        if (_side == Side.Pos)
        {
            func.AddTrisNorUV(ref posSideVerts, ref posSideTris, ref posSideNorm, ref posSideUV01,
                _vert0, _norm0, _uvs0,
                _vert1, _norm1, _uvs1,
                _vert2, _norm2, _uvs2,
                shared, first);
        }
        else
        {
            func.AddTrisNorUV(ref negSideVerts, ref negSideTris, ref negSideNorm, ref negSideUV01,
               _vert0, _norm0, _uvs0,
               _vert1, _norm1, _uvs1,
               _vert2, _norm2, _uvs2,
               shared, first);
        }
    }
}


