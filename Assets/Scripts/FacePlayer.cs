using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour {

	Transform player;
	public float rotSpeed = 90f;
	public float timer = 7f;
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (player == null) {
			GameObject go = GameObject.FindGameObjectWithTag ("Player");

			if (go != null) {
				player = go.transform;
			}
		}

		if (player == null || timer <= 0) {
			return;
		}

		Vector3 dir = player.position - transform.position;
		dir.Normalize ();

		float zAngle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;

		Quaternion finalRotation = Quaternion.Euler (0, 0, zAngle); 

		transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation, rotSpeed* Time.deltaTime);
	}
}
