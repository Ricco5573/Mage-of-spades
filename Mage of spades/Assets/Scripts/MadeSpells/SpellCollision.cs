using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCollision : MonoBehaviour
{
	[SerializeField] private float lifeTime = 1.5f;

	private void Start()
	{
		Destroy(gameObject, lifeTime);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.CompareTag("Enemy"))
			Destroy(collision.gameObject);
		else if (collision.gameObject.layer == 3)
			Destroy(gameObject);
		else if (collision.collider.CompareTag("EliteEnemy")){
			collision.collider.GetComponent<TestEnemyHealth>().TakeDamage();
			Destroy(gameObject);
		}
	}
}
