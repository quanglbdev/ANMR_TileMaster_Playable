//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	PoolKitExample_InstantiationTimer.cs
//
//	An example script that shows the performance difference between spawning and despawning with PoolKit and 
//	instantiating and destroying with Unity.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using HellTap.PoolKit;

public class PoolKitExample_InstantiationTimer : MonoBehaviour {

	public GameObject prefab = null;
	Pool pool = null;
	Stopwatch stopwatch;
	int objectsToInstantiate = 1000;
	List<Transform> instanceList = new List<Transform>();
	List<GameObject> instanceListGO = new List<GameObject>();

	public Text uiText = null;

	void Start(){
		pool = PoolKit.GetPoolContainingPrefab( prefab );
	}

	void UpdateResults( string optionUsed = "PoolKit" ){
		if(uiText!=null){
			uiText.text = 	"<b>RESULTS</b>\n" +
							optionUsed + " was used to process " + objectsToInstantiate.ToString() + " Unity Cubes! \n\n" +
							"<color=yellow>Instantiation / Spawn Time:</color><b> " + instantiationTimeMS.ToString() + " ms</b>\n" +
							"<color=yellow>Destroy / Despawn Time:</color><b> " + destroyTimeMS.ToString() + " ms</b>\n\n" +
							"<b>TOTAL TIME: " + (instantiationTimeMS+destroyTimeMS).ToString() + " ms</b>\n" +
							"\n<i><size=10>NOTE: Smaller Values Are Faster!</size></i>";
		}
	}

	public float instantiationTimeMS;
	public float destroyTimeMS;

	// USE POOL KIT TO CREATE A BUNCH OF OBJECTS
	public void TestWithPoolKit () {
		if( pool != null ){

			// Clear the instance list
			instanceList.Clear();

			// Reset Stopwatch
			stopwatch = Stopwatch.StartNew();

			// Spawn All Instances
			for( int i = 0; i < objectsToInstantiate; i++ ){
				pool.Spawn(prefab);
			}

			// Stop stopwatch
			stopwatch.Stop();
			instantiationTimeMS = stopwatch.ElapsedMilliseconds;

			// Reset Stopwatch
			stopwatch = Stopwatch.StartNew();

			// Despawn All Instances
			pool.DespawnAll( prefab );

			// Stop stopwatch
			stopwatch.Stop();
			destroyTimeMS = stopwatch.ElapsedMilliseconds;

			UpdateResults( "PoolKit (Spawn / Despawn)" );
		}
	}

	// USE INSTANTIATE AND DESTROY TO CREATE A BUNCH OF OBJECTS
	public void TestWithInstantiate () {
		if( pool != null ){

			// Clear the instance list
			instanceListGO.Clear();

			// Reset Stopwatch
			stopwatch = Stopwatch.StartNew();

			// Spawn All Instances
			for( int i = 0; i < objectsToInstantiate; i++ ){
				instanceListGO.Add( Instantiate(prefab) );
			}

			// Stop stopwatch
			stopwatch.Stop();
			instantiationTimeMS = stopwatch.ElapsedMilliseconds;

			// Reset Stopwatch
			stopwatch = Stopwatch.StartNew();

			// Despawn All Instances
			for( int i = 0; i < instanceListGO.Count; i++ ){
				Destroy( instanceListGO[i] );
			}

			// Stop stopwatch
			stopwatch.Stop();
			destroyTimeMS = stopwatch.ElapsedMilliseconds;

			UpdateResults( "Unity (Instantiation / Destroy)" );
		}
	}
	

}
