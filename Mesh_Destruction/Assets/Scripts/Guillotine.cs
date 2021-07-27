using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guillotine : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject blade;

    private Vector3 pos;

    private Rigidbody rigid;
    void Start()
    {
        rigid = blade.GetComponent<Rigidbody>();
        pos = blade.transform.position;
        
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(Input.GetKey(KeyCode.Space))
            {
                rigid.constraints = ~RigidbodyConstraints.FreezePositionY;

                rigid.useGravity = true;
            }


            if (Input.GetKey(KeyCode.R))
            {
                rigid.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                rigid.useGravity = false;
                blade.transform.position = pos;
            }
        }

    }
}
