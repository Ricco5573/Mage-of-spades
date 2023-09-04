using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spells : MonoBehaviour
{
    public int SpellSlots { get;  set; } = 8;
    [SerializeField] private Blink blink;
    [SerializeField] private Fireball fireball;
    [SerializeField] private Firebolt firebolt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.E))
			if(CheckForSpellSlots(blink.SpellCost))
                blink.Cast();
		
        if (Input.GetKeyDown(KeyCode.Q))
            if (CheckForSpellSlots(fireball.SpellCost))
                fireball.Cast();

        if (Input.GetKeyDown(KeyCode.F))
            if (CheckForSpellSlots(firebolt.SpellCost))
                firebolt.Cast();
    }

    bool CheckForSpellSlots(int costToCast)
	{
        bool isValidCast = false;
        if(SpellSlots >= costToCast)
		{
            SpellSlots -= costToCast;
            isValidCast = true;
		} else if (SpellSlots < costToCast)
            isValidCast = false;

        Debug.Log(isValidCast);
        Debug.Log(SpellSlots);
        return isValidCast;
	}
}
