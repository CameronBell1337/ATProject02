using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectData : MonoBehaviour
{
    public bool isConcaved = false;
    public bool renderInsideFaces = false;
    // Start is called before the first frame update
  

    public bool IsConcaved
    { 
        get { return isConcaved; }
        set { isConcaved = value; }
    }

    public bool RenderInsideFaces
    {
        get { return renderInsideFaces; }
        set { renderInsideFaces = value; }
    }
}
