using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    [Tooltip("Located at the BASE of the blade")]
    private GameObject swordVert0 = null;
    [SerializeField]
    [Tooltip("Located at the TIP of the blade")]
    private GameObject swordVert1 = null;
    [SerializeField]
    [Tooltip("Cutting force ")]
    private float cuttingForce = 2.0f;

    [SerializeField]
    [Tooltip("Obj with collider ")]
    private GameObject bladeCollider = null;

    private MeshCollider bladeCol;
    //these are used after the katana has entered into the cuttable object
    private Vector3 triggerTipStatPos;
    private Vector3 triggerTipEndPos;
    private Vector3 triggerBaseStatPos;
    //private Vector3 

    private bool attack = false;

    private void Start()
    {
        bladeCol = bladeCollider.GetComponent<MeshCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
            if (other.gameObject.tag == "Editable")
            {
                //Sets the pos for the cutting plane attack angle
                triggerTipStatPos = swordVert1.transform.position;
                triggerBaseStatPos = swordVert0.transform.position;
            }
        
    }


    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.Mouse0))
        {
            attack = true;
            bladeCol.isTrigger = true;
        }
        else
        {
            StartCoroutine(Delay());
        }
        
    }


    private void OnTriggerExit(Collider other)
    {
        
            if (other.gameObject.tag == "Editable")
            {

                triggerTipEndPos = swordVert1.transform.position;

                //gets difference of enter and exti positions between tip and base of sweord to create a triangle
                var side0 = triggerTipEndPos - triggerTipStatPos;
                var side1 = triggerTipEndPos - triggerBaseStatPos;

                //creates a point perpendicular to complete triangle and create cutting plane attack angle
                var normal = Vector3.Cross(side0, side1).normalized;
                Vector3 tN = ((Vector3)(other.gameObject.transform.localToWorldMatrix.transpose * normal)).normalized;
                //Get the enter position defined from the cuttable object local pos
                Vector3 tStart = other.gameObject.transform.InverseTransformPoint(triggerTipStatPos);

                Plane cuttingPlane = new Plane();

                cuttingPlane.SetNormalAndPosition(tN, tStart);

                var dir = Vector3.Dot(Vector3.up, normal);


                //keep facing towards the positive side
                if (dir < 0)
                    cuttingPlane = cuttingPlane.flipped;

                GameObject[] cuts = SlicedMesh.Slice(cuttingPlane, other.gameObject);
                Destroy(other.gameObject);

                //Adds a cutting force to the object succesfully cut
                Rigidbody rigid = cuts[1].GetComponent<Rigidbody>();
                Vector3 f = tN + Vector3.up * cuttingForce;
                rigid.AddForce(f, ForceMode.Impulse);

            }
        
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(3f);
        attack = false;
        bladeCol.isTrigger = false;
    }
}
