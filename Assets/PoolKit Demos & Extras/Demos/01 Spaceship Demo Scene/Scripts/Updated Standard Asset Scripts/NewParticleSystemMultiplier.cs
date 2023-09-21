using System;
using UnityEngine;

namespace UnityStandardAssets.Effects
{
    public class NewParticleSystemMultiplier : MonoBehaviour
    {
        // a simple script to scale the size, speed and lifetime of a particle system

        // Multipler
        public float multiplier = 1;

        // Pre-Cached Particle Systems
        public ParticleSystem[] systems = null;
        
        private void Awake(){       // <- This will only run this once when the object is instantiated

            // If we don't have a particle system, end early
            if( systems == null || systems.Length == 0 ){ return; }

            // Loop through the particle systems and play them
            for ( int i = 0; i < systems.Length; i++ ){
                if( systems[i] != null ){
    				ParticleSystem.MainModule mainModule = systems[i].main;
    				mainModule.startSizeMultiplier *= multiplier;
                    mainModule.startSpeedMultiplier *= multiplier;
                    mainModule.startLifetimeMultiplier *= Mathf.Lerp(multiplier, 1, 0.5f);
                    systems[i].Clear();
                    systems[i].Play();
                }
            }
        }
    }
}
