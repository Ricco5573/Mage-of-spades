using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 dir;
    private GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void Instantiate(Vector3 direction, GameObject prnt)
    {
        dir = direction; 
        parent = prnt;
        var directions = Quaternion.FromToRotation(-parent.transform.forward, -dir);
        transform.rotation *= directions;
    }

}
