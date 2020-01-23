using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bomb : MonoBehaviour
{
	public Action callback;

	void OnMouseDown()
	{
		if (Input.GetMouseButtonDown(0)) {
			Debug.Log("Clikkk");
			callback?.Invoke();
		}
	}
	
}
