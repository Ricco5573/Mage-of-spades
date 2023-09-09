using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetection : MonoBehaviour
{

    private FirstPersonCharacterController player;
    public bool leftSide;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<FirstPersonCharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Wall")
        {
            player.SetWalls(true, leftSide);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Wall")
        {
            player.SetWalls(false,leftSide);
        }
    }
}
