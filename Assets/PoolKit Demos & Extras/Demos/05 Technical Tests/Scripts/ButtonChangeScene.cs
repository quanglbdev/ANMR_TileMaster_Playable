using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HellTap.PoolKit{
	public class ButtonChangeScene : MonoBehaviour {

		// Use this for initialization
		public void LoadScene ( string sceneToLoad ) {
			if( Time.timeSinceLevelLoad > 1f ){
				SceneManager.LoadScene(sceneToLoad);
				gameObject.SetActive(false);
			}
		}
		
	}
}
