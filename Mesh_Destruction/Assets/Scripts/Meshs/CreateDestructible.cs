using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDestructible : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Vector3 size;
    [SerializeField] private Material mat;
    private GameObject obj;
    void Start()
    {

        

     
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            Regen();
        }

        
    }

    public void Regen()
    {
        
        create();
        if (gameObject.transform.childCount > 1)
        {
            Destroy(gameObject.transform.GetChild(0).gameObject);
        }
    }

    private void create()
    {
        var createMesh = CreateMesh.GetMesh(size);
       
        obj = new GameObject();

        obj.AddComponent<MeshRenderer>();
        obj.AddComponent<MeshFilter>();
        obj.AddComponent<MeshCollider>();
        obj.AddComponent<Rigidbody>();
        obj.AddComponent<ObjectData>();


        obj.name = "Cube";
        obj.tag = "Editable";
        obj.transform.SetParent(transform);
        obj.transform.position = Vector3.up * size.y / 2 + Vector3.forward * transform.position.z;

        var _mesh = obj.GetComponent<MeshFilter>();
        var _meshRenderer = obj.GetComponent<MeshRenderer>();
        var _mc = obj.GetComponent<MeshCollider>();

        _mesh.mesh = createMesh;
        _meshRenderer.sharedMaterial = mat;
        _mc.convex = true;
        _mc.sharedMesh = createMesh;
    }
        



}


