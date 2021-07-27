using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class SlicedMesh
{
    public static GameObject[] Slice(Plane plane, GameObject slicebleObj)
    {

        Mesh mesh = slicebleObj.GetComponent<MeshFilter>().mesh;
        ObjectData _editable = slicebleObj.GetComponent<ObjectData>();

        if (_editable == null)
        {
            Debug.LogWarning(slicebleObj.name + " is not editable. Please check for ObjectData script ref on the game object");
        }

        //Create pos/neg sliced obj
        MeshUtil _meshData = new MeshUtil(plane, mesh, _editable.IsConcaved, _editable.RenderInsideFaces);

        GameObject posNewObj = CreateCutObj(slicebleObj);
        posNewObj.name = "Positive_" + slicebleObj.name;
        posNewObj.transform.SetParent(GameObject.Find("CutObjs").transform);
        GameObject negNewObj = CreateCutObj(slicebleObj);
        negNewObj.name = "Negaive_" + slicebleObj.name;
        negNewObj.transform.SetParent(GameObject.Find("CutObjs").transform);

        var _PosMesh = _meshData.PosSideM;
        var _NegMesh = _meshData.NegSideM;

        posNewObj.GetComponent<MeshFilter>().mesh = _PosMesh;
        negNewObj.GetComponent<MeshFilter>().mesh = _NegMesh;

        //TODO setup colliderers and physics
        posNewObj.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        posNewObj.GetComponent<MeshFilter>().mesh.RecalculateTangents();

        negNewObj.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        negNewObj.GetComponent<MeshFilter>().mesh.RecalculateTangents();

        //Adds collider and RigidBody
        AddComponents(ref posNewObj, _PosMesh);
        AddComponents(ref negNewObj, _NegMesh);


        return new GameObject[] { posNewObj, negNewObj };
    }



    private static GameObject CreateCutObj(GameObject oldObj)
    {
        //gets all the materials attached to the cuttable object
        var _originalMat = oldObj.GetComponent<MeshRenderer>().materials;

        GameObject newMeshObj = new GameObject();
        ObjectData oldObjData = oldObj.GetComponent<ObjectData>();
        //Adds mesh filter and renderer to the new cut object
        newMeshObj.AddComponent<MeshFilter>();
        newMeshObj.AddComponent<MeshRenderer>();
        ObjectData newObjData = newMeshObj.AddComponent<ObjectData>();
        /* TODO get bool checks for:
         * is object solid or hollow
         * is inside faces need to be rendered
         * use of gravity
         */
        //Copies all data from old cut object to new

        newObjData.IsConcaved = oldObjData.IsConcaved;
        newObjData.RenderInsideFaces = oldObjData.RenderInsideFaces;

        newMeshObj.GetComponent<MeshRenderer>().materials = _originalMat;

        //Spawn the new mesh at the old object original position, rotation and scale 
        newMeshObj.transform.localScale = oldObj.transform.localScale;
        newMeshObj.transform.rotation = oldObj.transform.rotation;
        newMeshObj.transform.position = oldObj.transform.position;
        newMeshObj.tag = oldObj.tag;

        return newMeshObj;
    }

    private static void AddComponents(ref GameObject slicedObj, Mesh slicedMesh)
    {
        MeshCollider meshColl = slicedObj.AddComponent<MeshCollider>();
        var rigid = slicedObj.AddComponent<Rigidbody>();
        meshColl.sharedMesh = slicedMesh;
        meshColl.convex = true;
        rigid.useGravity = true;

    }
}
