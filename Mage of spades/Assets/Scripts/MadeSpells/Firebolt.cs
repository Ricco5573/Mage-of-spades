using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebolt : Spell
{
    [SerializeField] private GameObject fireboltPrefab;
    [SerializeField] private Transform castingPosition;
    private GameObject firebolt;
    private float fireboltSpeed = 20f;

    // Start is called before the first frame update
    void Start()
    {
        SpellCost = 2;
    }
    
	public override void Cast()
	{
        firebolt = Instantiate(fireboltPrefab, castingPosition.position, castingPosition.rotation);
        firebolt.GetComponent<Rigidbody>().velocity = castingPosition.forward * fireboltSpeed;
        Debug.Log("Firebolt!");
	}
}
