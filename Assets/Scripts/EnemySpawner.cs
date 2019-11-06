using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public GameObject enemy;
	public GameObject path;
	public int count;
	public float timer;

	IEnumerator Start()
	{
		while (true)
		{
			enemy.SetActive(false);
			var e = Instantiate(enemy, new Vector3(-30f, 0.3f, 10f), Quaternion.identity);
			e.GetComponent<SimpleEnemy>().paths = path;
			e.SetActive(true);

			if (count-- <= 0) break;

			yield return new WaitForSeconds(timer);
		}
	}


}
