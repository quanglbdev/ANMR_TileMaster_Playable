using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Effects
{
	public class NewExplosionPhysicsForce : MonoBehaviour
	{
		public float multiplier = 2f;
		public float explosionForce = 20;
		Collider[] cols = new Collider[0];
		float radius = 1f;

		void Start(){

			radius = 10f * UnityEngine.Random.Range( 1f, 2f );
			cols = Physics.OverlapSphere( transform.position, radius );
					   
			for ( int i = 0; i < cols.Length; i++ ){

				if ( cols[i].attachedRigidbody != null ){
					//rigidbodies.Add(col.attachedRigidbody);
					cols[i].attachedRigidbody.AddExplosionForce(
						explosionForce*multiplier, transform.position, radius, 1*multiplier, ForceMode.Impulse
					);
				}
			}
		}
	}
}
