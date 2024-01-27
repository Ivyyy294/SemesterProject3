using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass_Rotation : MonoBehaviour
{
    public Transform objectToFollow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(objectToFollow);
    }
}
