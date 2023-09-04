using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Spell
{

	// Start is called before the first frame update
	void Start()
    {
		SpellCost = 3;
    }

	public override void Cast()
	{
		Debug.Log("Fireball!");
	}

}
