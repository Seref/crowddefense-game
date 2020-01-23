using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableSprite : MonoBehaviour
{
	public Action callback;

	void OnMouseDown()
	{		
		if (Input.GetMouseButtonDown(0)) {			
			callback?.Invoke();
		}
	}
	
}
