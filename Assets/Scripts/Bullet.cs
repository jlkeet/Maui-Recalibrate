using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	Player player;

	void Start() {
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
	}

	void OnTriggerEnter2D(Collider2D col) {

		if (col.isTrigger != true) {
			
			if (col.CompareTag("Player")) {
				player.grounded = false;
				col.GetComponent<Player>().DamageTaken(1);
				StartCoroutine (player.Knockback (0.05f, 200.0f, player.transform.position));
			}
			Destroy (gameObject);
		}
	}
}
