using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerRandomChanceChangeText : MonoBehaviour {

	Text theText = null;
	void Awake(){ theText = GetComponent<Text>(); }
	public string prefix = "SP1: ";
	public string postfix = "%";
	public float multiplier = 100f;
	public string toStringCode = "0";

	public void UpdateChanceText( float value ) {
		if( theText != null ){ theText.text = prefix + (value * multiplier).ToString( toStringCode ) + postfix; }
	}
}
