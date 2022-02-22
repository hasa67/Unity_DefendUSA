using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {

	public float offset = 1.5f;
	private Vector2 screenSize;

	float widthOrtho;
	float heightOrtho;

	void Start () {
		screenSize = GameController.instance.CameraBoundary () * offset;
	}

	void Update () {
		if (transform.position.x > screenSize.x || transform.position.x < -screenSize.x) {
			Destroy (gameObject);
		}

		if (transform.position.y > screenSize.y || transform.position.y < -screenSize.y) {
			Destroy (gameObject);
		}
	}

}
