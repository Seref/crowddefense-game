using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this is the helper function

public static class HelperFunctions {
	public static Quaternion LookAt2D(Vector2 target, Vector2 pivot) {
		Vector2 dir = target - pivot;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		return Quaternion.AngleAxis(angle, Vector3.forward);
	}

	public static Quaternion LookAt2D(Vector3 difference) {		
		float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
		return Quaternion.Euler(0.0f, 0.0f, rotationZ);
	}

}
