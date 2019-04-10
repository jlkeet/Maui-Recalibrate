using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCone : MonoBehaviour {

	public TikiAI tikiAI;

	void Awake() {

		tikiAI = gameObject.GetComponentInParent<TikiAI> ();

	}

	void OnTriggerStay2D(Collider2D col) {
		if (col.CompareTag("Player")) {
			tikiAI.Attack();
		}
	}

}
