using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public PathCreator pathCreator;
    public FloatingCounter floatCounter;
    public int count;
    public float timer;
    public int waveSize = 5;


    GameManager gameManager2D;
    void Start()
    {
        gameManager2D = GetComponent<GameManager>();
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(2);

        var gameManager = GetComponent<GameManager>();
        while (true) {
            gameManager.Wave++;

            for (int i = 0; i < 5; i++) {
                GameObject enemy = ObjectPooler.Instance.GetPooledObject("Enemy");
                if (enemy != null) {
                    enemy.transform.position = new Vector3(-10f + i, 10f, 0);
                    enemy.transform.rotation = Quaternion.identity;
                    enemy.SetActive(true);
                    enemy.GetComponent<Enemy>().StartPath(pathCreator.path, gameManager);
                    var Text = Instantiate(floatCounter, new Vector3(-1000, -1000, 0), Quaternion.identity, GameObject.FindWithTag("UI").transform);
                    Text.Show(25.0f, enemy.transform, enemy.GetComponent<SpriteRenderer>().bounds.size.y);
                    

                }

                if (count-- <= 0)
                    yield break;
            }
            yield return new WaitForSeconds(timer);
        }

    }



}
