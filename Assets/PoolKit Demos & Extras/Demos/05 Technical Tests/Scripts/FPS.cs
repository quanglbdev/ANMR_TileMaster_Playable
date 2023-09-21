using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Using the HellTap namespace
namespace HellTap.PoolKit {

	[RequireComponent(typeof(Text))]
	public class FPS : MonoBehaviour {

		// Variables
		public string levelName = "This Level's Name";
		string deviceName;
		Text text = null;
		public int spawnsPerPass = 10;

		public string poolToFind = "TestPool";
		Pool pool = null;
		string poolInfoString = "";

		bool testComplete = false;
		public void SetTestComplete(){ testComplete = true; }
		public bool simpleFPSMode = true;

		////////////////////////////////////////////////////////////////////////////////////////////////
		//	START
		//	Initial Setup
		////////////////////////////////////////////////////////////////////////////////////////////////
		
		void Start(){

			// Cache the pool
			if(pool==null){ pool = PoolKit.Find(poolToFind); }
				
			// Set target frame rate to 60 fps
			Application.targetFrameRate = 60;

			// Cache spawns Per pass
			spawnsPerPass = PlayerPrefs.GetInt( "POOLKIT_SPAWNPASSES", spawnsPerPass );

			// Cache device name
			deviceName = SystemInfo.deviceModel;

			// Cache the UI Text
			text = GetComponent<Text>();

			// Initialize the FPS Buffer
			InitializeBuffer();

		}

		////////////////////////////////////////////////////////////////////////////////////////////////
		//	UPDATE
		//	Here we update the FPS and the UI text
		////////////////////////////////////////////////////////////////////////////////////////////////
		
		float elapsedTime = 0f;
		void Update () {

			// =====
			//	FPS
			// =====

			if( !testComplete ){

				// Elapsed Time
				elapsedTime += Time.deltaTime;

				// Track FPS
				if (fpsBuffer == null || fpsBuffer.Length != frameRange) {
					InitializeBuffer();
				}
				UpdateBuffer();
				CalculateFPS();

				// Setup Pool Info String (only in the editor because it slows things down)
				#if UNITY_EDITOR
					if(pool==null){ 
						poolInfoString = ""; 
					} else {
						poolInfoString = "Pool Instances: <color=yellow> " + pool.GetInstanceCount().ToString() + " </color> - Active: <color=yellow>  " + pool.GetActiveInstanceCount().ToString() + "  </color> Inactive: <color=yellow> " + pool.GetInactiveInstanceCount().ToString() + " </color>\n";
					}
				#endif

				// Simple FPS Mode
				if( simpleFPSMode ){

					// Update Text
					text.text = "<b>"+ levelName + "</b>\n"+
								"<i>Testing On: " + deviceName + " - <color=yellow>Elapsed Time: " + Mathf.Round(elapsedTime).ToString() + " Seconds</color></i>\n"+
								"<i>Objects Spawned / Instantiated Per Pass:  - <color=yellow>" + spawnsPerPass.ToString() + " </color></i>\n" +
								poolInfoString +
								"\n<b>AVERAGE FPS</b>\n"+
								"Average:                <color=yellow> " + averageFPS.ToString() + "</color>\n" +
								"Most Consistant:    <color=green> " + mostConsistentFPS.ToString() + "</color>\n" +
								"Worst Average:      <color=red> " + lowestEverAverageFPS.ToString() + "</color>\n" +
								"<size=13>Heavy Load Average:</size>     <color=red> " + mostConsistentFPSUnderLoad.ToString() + " </color>";

				} else {

					// Update Text
					text.text = "<b>"+ levelName + "</b>\n"+
								"<i>Testing On: " + deviceName + " - <color=yellow>Elapsed Time: " + Mathf.Round(elapsedTime).ToString() + " Seconds</color></i>\n"+
								"<i>Objects Spawned / Instantiated Per Pass:  - <color=yellow>" + spawnsPerPass.ToString() + " </color></i>\n" +
								poolInfoString +
								"\n<b>AVERAGE FPS COUNTER</b>\n"+
								"Average:                <color=yellow> " + averageFPS.ToString() + "</color>\n" +
								"Highest:                 <color=yellow> " + highestFPS.ToString() + "</color>\n" +
								"Lowest:                  <color=yellow> " + lowestFPS.ToString() + "</color>\n" +
								"Most Consistant:    <color=green> " + mostConsistentFPS.ToString() + "</color>\n" +
								"\n<b>FPS SINCE LEVEL STARTED</b>\n"+
								"Worst Average:      <color=red> " + lowestEverAverageFPS.ToString() + "</color>\n" +
								"Worst Sample:       <color=red> " + lowestEverFPS.ToString() + "</color>\n" +
								"<size=13>Heavy Load Average:</size>     <color=red> " + mostConsistentFPSUnderLoad.ToString() + " </color>";

				}

			} else {
				text.text = "<b>"+ levelName + "</b>\n"+
							"<i>Testing On: " + deviceName + "</i>\n\n"+
							"<b><color=yellow>TEST COMPLETE!</color></b>\n"+
							poolInfoString +
							"\n<b>RESULTS</b>\n"+
							"Most Consistant:    <color=green> " + mostConsistentFPS.ToString() + "</color>\n" +
							"Worst Average:      <color=red> " + lowestEverAverageFPS.ToString() + "</color>\n" +
							"Worst Sample:       <color=red> " + lowestEverFPS.ToString() + "</color>\n" +
							"<size=13>Heavy Load Average:</size>     <color=red> " + mostConsistentFPSUnderLoad.ToString() + " </color>";
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////
		//	CALCULATE FPS
		//	These values and methods calculate the average FPS
		////////////////////////////////////////////////////////////////////////////////////////////////
		
		// CORE HELPER VALUES
		int averageFPS = 0;
		int highestFPS = 0;
		int lowestFPS = 60;
		int lowestEverFPS = 100;
		int lowestEverAverageFPS = 100;
		int frameRange = 60;		// How big the buffer should be (to calculate the average)
		int mostConsistentFPS = 0;	// What is the average FPS that appears most often? (tally average)
		int mostConsistentFPSUnderLoad = 30;	// What is the average FPS that appears most often? (under 30 fps)

		// Helpers
		int sum = 0;
		int highest = 0;
		int lowest = 100;
		int fps = 0;
		int[] averageFPSTally = new int[101];	// Tallys up to 100 fps
		int averageFPSTallyIndex = -1;

		// CALCULATE THE FPS
		void CalculateFPS () {
			sum = 0;
			highest = 0;
			lowest = int.MaxValue;
			for (int i = 0; i < frameRange; i++) {
				fps = fpsBuffer[i];
				sum += fps;
				if (fps > highest) {
					highest = fps;
				}
				if (fps < lowest) {
					lowest = fps;
				}
			}
			averageFPS = sum / frameRange;
			highestFPS = highest;
			lowestFPS = lowest;

			// Record the biggest drops (wait 3 seconds so it skips lags in initialization)
			if( lowestFPS < lowestEverFPS && Time.timeSinceLevelLoad > 3 ){
				lowestEverFPS = lowestFPS;
			}
			if( averageFPS < lowestEverAverageFPS && Time.timeSinceLevelLoad > 3 ){
				lowestEverAverageFPS = averageFPS;
			}

			// Tally the average FPS
			averageFPSTallyIndex = Mathf.Clamp(averageFPS, 0, averageFPSTally.Length-1);
			averageFPSTally[ averageFPSTallyIndex ] = averageFPSTally[ averageFPSTallyIndex ] + 1;
			GetAverageTally();
		}


		// GET AVERAGE TALLY
		int _gatHighest = 0;
		void GetAverageTally(){

			// Reset the helpers
			_gatHighest = 0;
			
			// Loop through the array and find the highest number
			for( int i = 0; i < averageFPSTally.Length; i++ ){
				if( averageFPSTally[i] >= _gatHighest ){
					_gatHighest = averageFPSTally[i];
					mostConsistentFPS = i;

					// Also track the most consistent under load (30fps or under)
					if( i <= 30 ){
						mostConsistentFPSUnderLoad = i;
					}
				}
			}

			/*
			// Loop through the array and find the highest average fps (under 20)
			for( int i = 0; i < 20; i++ ){
				if( averageFPSTally[i] >= _gatHighest ){
					_gatHighest = averageFPSTally[i];
					mostConsistentFPSUnderLoad = i;
				}
			}
			*/
		}

		// INITIALIZE THE FPS VALUE BUFFER
		void InitializeBuffer () {
			if (frameRange <= 0) {
				frameRange = 1;
			}
			fpsBuffer = new int[frameRange];
			fpsBufferIndex = 0;
		}

		// VALUE BUFFER
		int[] fpsBuffer;
		int fpsBufferIndex;
		void UpdateBuffer () {
			fpsBuffer[fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
			if (fpsBufferIndex >= frameRange) {
				fpsBufferIndex = 0;
			}
		}
	}
}
