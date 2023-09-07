using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spells : MonoBehaviour
{
    public int SpellSlots { get;  set; } = 15;
    bool canBlink = false;
    float blinkCooldown = 0;
    [SerializeField] private Blink blink;
    [SerializeField] private Fireball fireball;
    [SerializeField] private Firebolt firebolt;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            if (blinkCooldown == 0 && CheckForSpellSlots(blink.SpellCost))
			{
                blinkCooldown = 2.5f;
                canBlink = true;
                blink.Preview();
			}

          if(Input.GetKeyUp(KeyCode.LeftShift))
			if(canBlink)
			{
                StartCoroutine(RefreshBlinkCooldown());
                blink.Cast();
                canBlink = false;
			}

        if (Input.GetKeyDown(KeyCode.Alpha2))
            if (CheckForSpellSlots(firebolt.SpellCost))
                firebolt.Cast();
		
        if (Input.GetKeyDown(KeyCode.Alpha3))
            if (CheckForSpellSlots(fireball.SpellCost))
                fireball.Cast();

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

        return isValidCast;
	}
    IEnumerator RefreshBlinkCooldown()
	{
        for (float i = 0; i < 5; i ++)
		{
            blinkCooldown -= 0.5f;
            yield return new WaitForSeconds(0.5f);
		}
	}
}
