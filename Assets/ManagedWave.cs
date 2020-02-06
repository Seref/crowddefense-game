using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagedWave : MonoBehaviour
{
	public TMPro.TextMeshProUGUI text;
	public Animator animator;
	public Animator textanimator;
	public Canvas canvas;


	public void Start()
	{
		canvas.worldCamera = Camera.main;
	}

	public void PlayAnimation(string texts)
	{
		text.text = texts;
		textanimator.enabled = true;
		animator.enabled = true;
	}
    
}
