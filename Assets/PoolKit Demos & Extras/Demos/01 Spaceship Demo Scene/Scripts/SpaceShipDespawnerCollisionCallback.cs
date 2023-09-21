using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class SpaceShipDespawnerCollisionCallback : MonoBehaviour {

	public Despawner despawner = null;

	void Awake() {

		// If this despawner is not set and a Despawner is attached on this gameobject, use that.
		if(despawner==null && GetComponent<Despawner>() != null ){ despawner = GetComponent<Despawner>(); }
		
		// Subscribe to despawner callbacks when this object is instantiated
		if(despawner!=null){ despawner.onDespawnerCollided += onDespawnerCollided; }
	}

	void OnDestroy(){

		// Unsubscribe to despawner callbacks when this object is about to be destroyed
		if(despawner!=null){ despawner.onDespawnerCollided -= onDespawnerCollided; }
	}
	
	// If the bullet collides with an enemy, apply rigidbody force
	public float explosionForce = 200f;
	Rigidbody _enemyRB = null;
	Vector3 _randomV3 = Vector3.zero;

	// This is the callback from the despawner (when it detects a physics collision with the enemy plane)
	void onDespawnerCollided ( GameObject go ) {
		if( go != null ){

			// Apply cheap explosion physics to the enemy plane
			if( go.GetComponent<Rigidbody>() != null ){

				_enemyRB = go.GetComponent<Rigidbody>();
				_enemyRB.useGravity = true;

				_randomV3.x = Random.Range(-explosionForce, explosionForce);
				_randomV3.y = Random.Range(-explosionForce, explosionForce);
				_randomV3.z = Random.Range(-explosionForce, explosionForce);
				_enemyRB.angularVelocity = _randomV3;
			}

			// Turn off the enemy plane's collider so it doesn't keep triggering events
			if( go.GetComponent<Collider>() != null ){
				go.GetComponent<Collider>().enabled = false;
			}
		}
	}
}
