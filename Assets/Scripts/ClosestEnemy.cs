using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestEnemy : MonoBehaviour {

	private GameObject targetEnemy;
	public float rotSpeed = 90f;
	private float timer = 7f;

	void Start () {
		
	}

	void Update () {
		timer -= Time.deltaTime;
		targetEnemy = FindClosestEnemy ();

		if (targetEnemy != null && timer >= 0) {
			Vector3 dir = targetEnemy.transform.position - transform.position;
			dir.Normalize ();

			float zAngle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;

			Quaternion finalRotation = Quaternion.Euler (0, 0, zAngle); 

			transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation, rotSpeed* Time.deltaTime);
		}
	}

	public GameObject FindClosestEnemy(){
		float distanceToClosestEnemy = Mathf.Infinity;
		GameObject closestEnemy = null;
		GameObject[] allEnemies = GameObject.FindGameObjectsWithTag ("Enemy");

		if (allEnemies != null) {
			foreach (GameObject currentEnemy in allEnemies) {
				float distanceToEnemy = (currentEnemy.transform.position - transform.position).sqrMagnitude;

				if (distanceToEnemy < distanceToClosestEnemy) {
					distanceToClosestEnemy = distanceToEnemy;
					closestEnemy = currentEnemy;
				}
			}

			if (closestEnemy != null) {
				// Debug.DrawLine (this.transform.position, closestEnemy.transform.position);
			}
		}
		return closestEnemy;
	}
}
