//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	RealtimePoolProtectionExample.cs
//
//	An example script trying to randomly destroy instances in the pool to test Pool Protection.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HellTap.PoolKit;

public class RealtimePoolProtectionExample : MonoBehaviour {

	[Header("Find Your Spawner By GameObject Name")]
	public string poolToFind = "TestPool";
	public Pool pool = null;
	Transform[] instances;
	int randomIndex = 0;

	[Header("Update Text")]
	public Text theText = null;

	// ==================================================================================================================
	//	START
	//	We cache the pool at start
	// ==================================================================================================================
	
	void Start(){
		pool = PoolKit.GetPool( poolToFind );
	}

	// ==================================================================================================================
	//	UPDATE
	//	Show status of the instances
	// ==================================================================================================================
	
	void Update(){
		
		// Make sure the Pool and Text references are set ...
		if(!theText || !pool){ return; }

		// Update the text stats
		theText.text = 	"Total Number Of Instances: " + pool.GetInstanceCount().ToString() +
						"\nSpawned Instances: " + pool.GetActiveInstanceCount().ToString() +
						"\nDespawned Instances: " + pool.GetInactiveInstanceCount().ToString();
	}

	// ==================================================================================================================
	//	DELETE RANDOM INSTANCE
	//	Delete a single instance at random
	// ==================================================================================================================

	public void DeleteRandomInstance(){
		if( pool != null ){
			Debug.Log("POOLKIT - RealTime Pool Protection Example: Deleting Instance At Random!");
			instances = pool.GetInstances();
			if( instances.Length > 0 ){
				randomIndex = Random.Range(0, instances.Length-1 );
				if( instances[randomIndex] != null ){ Destroy(instances[randomIndex].gameObject); }
			}
		}
	}

	// ==================================================================================================================
	//	DELETE ALL INSTANCES
	//	Deletes all instances in the pool
	// ==================================================================================================================

	public void DeleteAllInstances(){
		if( pool != null ){
			Debug.Log("POOLKIT - RealTime Pool Protection Example: Deleting ALL instances!");
			instances = pool.GetInstances();
			for( int i = 0; i < instances.Length; i++ ){
				if(instances[i]!=null){ Destroy(instances[i].gameObject); }
			}
		}
	}

}		
