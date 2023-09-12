using System.Collections;
using UnityEngine;

public class SpellcastingUI : MonoBehaviour
{
	Spellcasting spellcasting;

	[SerializeField] private HealthBar blinkCooldownBar;
	[SerializeField] private GrayOutUI blinkUI;
	[SerializeField] GrayOutUI fireboltUI;
	[SerializeField] GrayOutUI fireballUI;
	// Start is called before the first frame update
	void Awake()
	{
		spellcasting = gameObject.GetComponent<Spellcasting>();
	} 

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.LeftShift) && spellcasting.SpellSlots >= 1)
		{
			blinkCooldownBar.SetHealth(0f);
			StartCoroutine(DisplayBlinkCooldown());
		}

		if (Input.GetKeyDown(KeyCode.Alpha2) && spellcasting.SpellSlots >= 2)
		{
			StartCoroutine(fireboltUI.TimedColoredFill(0.10f, Color.white));
		}

		if (Input.GetKeyDown(KeyCode.Alpha3) && spellcasting.SpellSlots >= 3)
		{
			StartCoroutine(fireballUI.TimedColoredFill(0.10f, Color.white));
		}
		UpdateSpellsOnHUD();
	}

	private IEnumerator DisplayBlinkCooldown()
	{
		for (int i = 0; i < 5; i++)
		{
			blinkCooldownBar.SetHealth(((i * 0.5f) + 0.5f));
			yield return new WaitForSeconds(0.5f);
		}
	}

	public void UpdateSpellsOnHUD()
	{
		if (spellcasting.SpellSlots < 3)
			fireballUI.GrayOutFill();
		if (spellcasting.SpellSlots < 2)
			fireboltUI.GrayOutFill();
		if (spellcasting.SpellSlots < 1)
			blinkUI.GrayOutFill();
	}

}
