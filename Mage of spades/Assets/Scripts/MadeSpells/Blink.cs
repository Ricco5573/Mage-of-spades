using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : Spell
{
    // Start is called before the first frame update
    void Start()
    {
         SpellCost = 1;
    }

    public override void Cast()
	{
        Debug.Log("Blink!");
	}
}
