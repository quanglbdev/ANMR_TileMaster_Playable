using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowJetRotation : MonoBehaviour {

	// Helpers
	float updateRotationCounter = 0f;
	public float minTime = 1.5f;
	public float maxTime = 3f;
	public float maxMotion = 2f;
	public float speed = 0.5f;
	Vector3 rotationTarget = Vector3.zero;

	// Update is called once per frame
	void Update () {
		
		// Countdown timer
		updateRotationCounter -= Time.deltaTime;

		// When the countdown runs out, setup a new rotation target
		if(updateRotationCounter < 0){
			updateRotationCounter = Random.Range( minTime, maxTime );

			rotationTarget.x = Random.Range(-maxMotion, maxMotion);
			rotationTarget.y = Random.Range(-maxMotion, maxMotion);
			rotationTarget.z = Random.Range(-maxMotion, maxMotion);
		}

		// Lerp the rotation
		transform.rotation = Quaternion.Lerp( transform.rotation, Quaternion.Euler(rotationTarget), Time.deltaTime * speed );
	}
}
