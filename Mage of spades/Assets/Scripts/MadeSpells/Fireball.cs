using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Spell
{
	[SerializeField] private GameObject fireballPrefab;
	[SerializeField] private Transform castingPosition;
	private GameObject fireball;
	private float fireballSpeed = 4.5f;
	private Rigidbody fireballRb;
	// Start is called before the first frame update
	void Start()
    {
		SpellCost = 3;
    }

	public override void Cast()
	{
		fireball = Instantiate(fireballPrefab, castingPosition.position, castingPosition.rotation);
		fireballRb = fireball.GetComponent<Rigidbody>();
		fireballRb.velocity = castingPosition.forward * fireballSpeed;
		fireballRb.AddForce(0, 75, 0);
		Debug.Log("Fireball!");
	}

}
