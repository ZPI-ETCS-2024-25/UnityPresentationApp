using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(gameObject.transform.GetChild(1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
