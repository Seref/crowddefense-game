﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOrder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y*10f) * -1;
	}
}
