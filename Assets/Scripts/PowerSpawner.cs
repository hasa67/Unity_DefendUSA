using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSpawner : MonoBehaviour {
	
	public GameObject firePower;
	public float firePowerCooldown = 5f;
	private float firePowerTimer;
	private int firePowerCount = 2;
	public int maxFirePower = 3;

	private float heightOrtho;
	private float offset = 0.9f;

	private PlayerControls playerControls;
	private int playerFirePower;

	void Start () {
		heightOrtho = offset * Camera.main.orthographicSize;
		firePowerTimer = firePowerCooldown;
	}
	
	// Update is called once per frame
	void Update () {
		if (GameObject.FindWithTag ("Player") != null) {
			playerControls = GameObject.FindWithTag ("Player").GetComponent<PlayerControls>();

			if (playerControls != null) {
				playerFirePower = playerControls.firePower;
			}
		}

		firePowerTimer -= Time.deltaTime;

		if (firePowerTimer <= 0 && playerFirePower < maxFirePower) {
			float randFireY = Random.Range (-heightOrtho, heightOrtho);
			Instantiate (firePower, new Vector3 (transform.position.x, randFireY, 0), Quaternion.identity);
			firePowerCount--;
			firePowerTimer = firePowerCooldown;
		}
	}
}
