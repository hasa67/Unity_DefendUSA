using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour {

	public float maxSpeed = -3;

	public bool isVertical = false;
	public float vertSpeed = 3;
	public float vertRange = 3;
	private float direction;


	void Start () {
		if (isVertical) {
			direction = Mathf.Pow (-1, (int)Random.Range (1, 3));
		}
	}


	void Update () {
		float positionX = maxSpeed * Time.deltaTime;
		float positionY = 0;

		if (isVertical)
			positionY = Mathf.Sin (Time.time * vertSpeed) * Time.deltaTime * vertRange * direction;

		Vector3 position = new Vector3 (positionX, positionY, 0f);
		transform.Translate (position);
	}
}
