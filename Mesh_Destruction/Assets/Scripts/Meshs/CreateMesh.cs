using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMesh 
{
    /*Points of a cube
           V5 __________ V6
             /|        /|
            / |       / |
        V4 /__|____V7/  | 
           |  |V1___|___|V2
           |  /     |  /
           | /      | /
       V0  |/_______|/V3

       */
    public static Mesh GetMesh(Vector3 size)
    {
       
        var _os = 0.5f; //offset 

        Vector3 v0 = new Vector3(   -size.x * _os, -size.y * _os,  size.z * _os);
        Vector3 v1 = new Vector3(   size.x * _os, -size.y * _os,   size.z * _os);
        Vector3 v2 = new Vector3(   size.x * _os, -size.y * _os,  -size.z * _os);
        Vector3 v3 = new Vector3(   -size.x * _os, -size.y * _os, -size.z * _os);
        Vector3 v4 = new Vector3(   -size.x * _os, size.y * _os,   size.z * _os);
        Vector3 v5 = new Vector3(   size.x * _os, size.y * _os,    size.z * _os);
        Vector3 v6 = new Vector3(   size.x * _os, size.y * _os,   -size.z * _os);
        Vector3 v7 = new Vector3(   -size.x * _os, size.y * _os,  -size.z * _os);

        Vector3[] verts =
        {
            v0, v1, v2, v3, //down
            v7, v4, v0, v3, //left
            v4, v5, v1, v0, //forward
            v6, v7, v3, v2, //back
            v5, v6, v2, v1, //right
            v7, v6, v5, v4  //up

        };

        var up = Vector3.up;
        var down = Vector3.down;
        var forward = Vector3.forward;
        var back = Vector3.back;
        var left = Vector3.left;
        var right = Vector3.right;

        Vector3[] normals =
        {
            down, down, down, down,
            left, left, left, left,
            forward, forward, forward, forward,
            back, back, back, back,
            right, right, right, right,
            up, up, up, up

        };

        var cord_0_0 = new Vector2(0, 0);
        var cord_0_1 = new Vector2(0, 1);
        var cord_1_0 = new Vector2(1, 0);
        var cord_1_1 = new Vector2(1, 1);

        Vector2[] uv =
        {
            cord_1_1, cord_0_1, cord_0_0, cord_1_0,
            cord_1_1, cord_0_1, cord_0_0, cord_1_0,
            cord_1_1, cord_0_1, cord_0_0, cord_1_0,
            cord_1_1, cord_0_1, cord_0_0, cord_1_0,
            cord_1_1, cord_0_1, cord_0_0, cord_1_0,
            cord_1_1, cord_0_1, cord_0_0, cord_1_0
        };

        int[] tris =
        {
            3,1,0, //bottom side
            3,2,1,
            
            3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1, //Left side
            3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
            
            3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2, //Front side
            3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
            
            3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3, //Back side
            3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
            
            3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4, //Right side
            3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
            
            3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5, //Top side
            3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
        };


        var mesh = new Mesh
        {
            vertices = verts,
            triangles = tris,
            normals = normals,
            uv = uv
        };
        return mesh;
    }
}
