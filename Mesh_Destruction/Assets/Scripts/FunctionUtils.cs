using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionUtils
{
    //Functions needed for mesh slicing
    public void MoveIndices(ref List<int> tris)
    {
        for (int j = 0; j < tris.Count; j += 3)
        {
            tris[j] += +3;
            tris[j + 1] += 3;
            tris[j + 2] += 3;
        }
    }

    public void AddTrisNorUV(ref List<Vector3> _verts, ref List<int> tris, ref List<Vector3> normals, ref List<Vector2> uvs,
        Vector3 _vert0, Vector3? _norm0, Vector2 _uvs0,
        Vector3 _vert1, Vector3? _norm1, Vector2 _uvs1,
        Vector3 _vert2, Vector3? _norm2, Vector2 _uvs2, bool shared, bool first)
    {
        //Forces fix for lighting to add extra tris before recalculating 
        first = true;
        shared = false;

        var _tri1Index = _verts.IndexOf(_vert0);
        if (first)
        {
            MoveIndices(ref tris);

        }

        if (_tri1Index > -1 && shared)
        {
            tris.Add(_tri1Index);
        }
        else
        {
            if (_norm0 == null)
            {
                //get vert perpendicular to the face mentioned
                _norm0 = Normal(_vert0, _vert1, _vert2);
            }

            int? i = null;
            if (first)
            {
                i = 0;
            }

            //Add vertnormalUV
            AddVerts(ref _verts, ref normals, ref uvs, ref tris, _vert0, (Vector3)_norm0, _uvs0, i);
        }

        var _tri2Index = _verts.IndexOf(_vert1);
        if (_tri2Index > -1 && shared)
        {
            tris.Add(_tri2Index);
        }
        else
        {
            if (_norm1 == null)
            {
                _norm1 = Normal(_vert1, _vert2, _vert0);
            }

            int? i = null;
            if (first)
            {
                i = 1;
            }

            //Add vertnormalUV
            AddVerts(ref _verts, ref normals, ref uvs, ref tris, _vert1, (Vector3)_norm1, _uvs1, i);
        }

        var _tri3Index = _verts.IndexOf(_vert2);
        if (_tri3Index > -1 && shared)
        {
            tris.Add(_tri3Index);
        }
        else
        {
            if (_norm2 == null)
            {
                _norm2 = Normal(_vert2, _vert0, _vert1);
            }

            int? i = null;
            if (first)
            {
                i = 2;
            }

            //Add vertnormalUV
            AddVerts(ref _verts, ref normals, ref uvs, ref tris, _vert2, (Vector3)_norm2, _uvs2, i);
        }
    }

    public void AddVerts(ref List<Vector3> _verts, ref List<Vector3> _normals, ref List<Vector2> _uv01, ref List<int> _tris,
        Vector3 vert, Vector3 normal, Vector2 uvs, int? index)
    {
        if (index != null)
        {
            var i = (int)index;
            _verts.Insert(i, vert);
            _uv01.Insert(i, uvs);
            _normals.Insert(i, normal);
            _tris.Insert(i, i);
        }
        else
        {
            _verts.Add(vert);
            _normals.Add(normal);
            _uv01.Add(uvs);
            _tris.Add(_verts.IndexOf(vert));
        }
    }

    public Vector3 Normal(Vector3 _vert0, Vector3 _vert1, Vector3 _vert2)
    {
        Vector3 _side0 = _vert1 - _vert0;
        Vector3 _side1 = _vert2 - _vert0;

        Vector3 _tempNormal = Vector3.Cross(_side0, _side1);

        return _tempNormal;
    }

    public Vector2 InterpUvs(Vector2 _uv1, Vector2 _uv2, float distance)
    {
        Vector2 uv = Vector2.Lerp(_uv1, _uv2, distance);
        return uv;
    }

    public float GetDistFromPlane(Vector3 _vert0, Vector3 _vert1, Plane _cuttingPlane, out Vector3 pointOfintersection)
    {
        //Gets distance from the cutting plane
        Ray cast = new Ray(_vert0, (_vert1 - _vert0));
        _cuttingPlane.Raycast(cast, out float dist);
        pointOfintersection = cast.GetPoint(dist);
        return dist;
    }


}
