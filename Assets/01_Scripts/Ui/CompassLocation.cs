using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass_Location : MonoBehaviour
{
    public Transform objectToFollow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = objectToFollow.position;
        transform.rotation = objectToFollow.rotation;
    }
}
