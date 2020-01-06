using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioForcer : MonoBehaviour
{	
	public float targetAspectRatio = 16f / 9f;
	public float currentAspectRatio = 0f;
	void Update()
	{
		if (currentAspectRatio != Camera.main.aspect)
		{
			GetComponent<Camera>().rect = new Rect(0f, (1.0f - Camera.main.aspect / targetAspectRatio) / 2, 1f, Camera.main.aspect / targetAspectRatio);
			currentAspectRatio = Camera.main.aspect;
		}
	}
}

