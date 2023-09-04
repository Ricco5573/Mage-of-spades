using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebolt : Spell
{
    // Start is called before the first frame update
    void Start()
    {
        SpellCost = 2;
    }
	public override void Cast()
	{
        Debug.Log("Firebolt!");
	}
}
