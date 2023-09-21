using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class SpawnerChangeSelectionMode : MonoBehaviour {

	public string spawnerToFind = "PoolKit Spawner";
	Spawner spawner = null;

	// Use this for initialization
	void Start () {
		spawner = PoolKit.GetSpawner(spawnerToFind);
	}
	
	// Change Selection Mode
	public void SetSequenceAscending() {
		if(spawner!=null){ spawner.spawnPointSelection = Spawner.SpawnPointSelection.SequenceAscending; }
	}

	public void SetSequenceDescending() {
		if(spawner!=null){ spawner.spawnPointSelection = Spawner.SpawnPointSelection.SequenceDescending; }
	}

	public void SetPingPongAscending() {
		if(spawner!=null){ spawner.spawnPointSelection = Spawner.SpawnPointSelection.PingPongAscending; }
	}

	public void SetPingPongDescending() {
		if(spawner!=null){ spawner.spawnPointSelection = Spawner.SpawnPointSelection.PingPongDescending; }
	}

	public void SetRandom() {
		if(spawner!=null){ spawner.spawnPointSelection = Spawner.SpawnPointSelection.Random; }
	}

	public void SetRandomWithWeights() {
		if(spawner!=null){ spawner.spawnPointSelection = Spawner.SpawnPointSelection.RandomWithWeights; }
	}

	// Set Random Weight - Point 1
	public void SetRandomWeightPoint1( float value ) {
		if(spawner!=null){ spawner.SetRandomWeightOfTransformPosition( 0, value * 100f ); }
	}

	public void SetRandomWeightPoint2( float value ) {
		if(spawner!=null){ spawner.SetRandomWeightOfTransformPosition( 1, value * 100f ); }
	}

	public void SetRandomWeightPoint3( float value ) {
		if(spawner!=null){ spawner.SetRandomWeightOfTransformPosition( 2, value * 100f ); }
	}

	public void SetRandomWeightPoint4( float value ) {
		if(spawner!=null){ spawner.SetRandomWeightOfTransformPosition( 3, value * 100f ); }
	}

	public void SetRandomWeightPoint5( float value ) {
		if(spawner!=null){ spawner.SetRandomWeightOfTransformPosition( 4, value * 100f ); }
	}

	public void SetSpawnerInstances( float value ) {
		if(spawner!=null){ spawner.instancesPerCycle = Mathf.RoundToInt( value * 5f ); }
	}

	public void SetSpawnerFrequency( float value ) {
		if(spawner!=null){ spawner.frequencyFixedInterval = value * 2f; }
	}

}

