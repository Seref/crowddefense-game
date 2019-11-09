using System.Collections;
using UnityEngine;

public class EnemySpawner2D : MonoBehaviour
{	
	public GameObject path;
	public int count;
	public float timer;

	IEnumerator Start()
	{
		yield return new WaitForSeconds(2);
		while (true)
		{			
			GameObject enemy = ObjectPooler.Instance.GetPooledObject("Enemy");
			if (enemy != null)
			{
				enemy.transform.position = new Vector3(-10f, 10f, 0);
				enemy.transform.rotation = Quaternion.identity;
				enemy.SetActive(true);
				enemy.GetComponent<Enemy2D>().StartPath(path);				
			}									

			if (count-- <= 0) break;

			yield return new WaitForSeconds(timer);
		}
	}


}
