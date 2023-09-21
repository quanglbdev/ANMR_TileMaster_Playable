using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolKitBullet : MonoBehaviour {

	// Variables
	public bool useGravity = false;
	public float forwardForce = 250f;
	Rigidbody rb = null;
	Collider c = null;

	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody>();
		c = GetComponent<Collider>();
	}

	// Move the bullet forward every frame
	void FixedUpdate (){
		rb.AddForce( transform.forward.normalized * forwardForce );
	}

	// Turn on the collider when this is spawned (if there is one) and setup gravity settings
	void OnEnable(){
		if(c){ c.enabled = true; }
		if(rb){ rb.useGravity = useGravity; }
	}

	// Reset rigidbody velocities when this is despawned
	void OnDisable(){
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}

	// As soon as we get hit with the first bullet, turn off the collider (if there is one)
	void OnCollisionEnter(){
		if(c){ c.enabled = false; }
	}

}
