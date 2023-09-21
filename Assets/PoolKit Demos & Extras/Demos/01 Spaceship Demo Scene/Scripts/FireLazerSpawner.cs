using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class FireLazerSpawner : MonoBehaviour {

	public string spawnerToFind = "Lazer Spawner";
	Spawner spawner;

	void Start(){
		spawner = PoolKit.GetSpawner(spawnerToFind);
	}

	// Update is called once per frame
	void Update () {

		// Don't run update if we haven't cached the spawner
		if(!spawner){ return; }

		// Start the lazer spawner when the mouse button is down
		if( Input.GetMouseButtonDown(0) ){ spawner.Play(); }

		// Stop the lazer spawner when we release the mouse button
		if( Input.GetMouseButtonUp(0) ){ spawner.Stop(); }

	}
}
