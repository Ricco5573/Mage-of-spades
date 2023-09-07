using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyHealth : MonoBehaviour
{
	private int Health = 2;

	public void TakeDamage()
	{
		Health -= 1;

		if (Health == 0)
			Destroy(gameObject);
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.CompareTag("Fireball"))
			Destroy(gameObject);
	}
}
