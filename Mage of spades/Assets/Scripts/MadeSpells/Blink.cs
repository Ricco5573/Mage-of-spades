using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : Spell
{
    [SerializeField] private GameObject blinkPosition;
    [SerializeField]private TestLook look;
    MeshRenderer blinkMesh;
    private TestMovement movement;
    // Start is called before the first frame update
    void Start()
    {
        movement = gameObject.GetComponent<TestMovement>();
        blinkMesh = blinkPosition.GetComponent<MeshRenderer>();
         SpellCost = 1;
    }

    public override void Cast()
	{
        StartCoroutine(Blinking());
        Debug.Log("Blink!");
	}

    public void Preview()
	{
        movement.isDisabled = true;
        blinkMesh.enabled = true;
	}

    IEnumerator Blinking()
	{
        look.isDisabled = true;
        blinkMesh.enabled = false;
        yield return new WaitForSeconds(0.01f);

        gameObject.transform.position = blinkPosition.transform.position;

        yield return new WaitForSeconds(0.01f);
        movement.isDisabled = false;
        look.isDisabled = false;
	}
}
