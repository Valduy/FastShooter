using MoreMountains.CorgiEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyWavesSpawner : MonoBehaviour
{
    private float _actualEnemyCount;
    private List<GameObject> _spawnedEnemies;
    private Stack<GameObject> _instantiationStack;

    public int EnemyCount;
    public float TimeBetweenWaves;
    public float TimeBetweenSpawns;
    public float Multiplier;

    public GameObject Prefab;
    public SpawnPoint[] SpawnPoints;

    void Start()
    {
        _actualEnemyCount = EnemyCount;
        _spawnedEnemies = new List<GameObject>();
        _instantiationStack = new Stack<GameObject>();
        StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(TimeBetweenWaves);

            _spawnedEnemies.ForEach(e => _instantiationStack.Push(e));

            for (int i = 0; i < EnemyCount; i++)
            {
                SpawnEnemy(GetRandomSpawnPointPosition());
                yield return new WaitForSeconds(TimeBetweenSpawns);
            }

            _actualEnemyCount *= Multiplier;
            EnemyCount = (int)_actualEnemyCount;

            yield return WaitForWaveDeath();
        }
    }

    private void SpawnEnemy(Vector3 position)
    {
        GameObject enemy;

        if (_instantiationStack.Any())
        {
            enemy = _instantiationStack.Pop();
        }
        else
        {
            enemy = Instantiate(Prefab, position, Quaternion.identity);
            _spawnedEnemies.Add(enemy);
        }

        enemy.GetComponent<Health>().Revive();
        enemy.transform.position = position;
        enemy.SetActive(true);
    }

    private IEnumerator WaitForWaveDeath()
    {
        while (_spawnedEnemies.Any(e => e.activeSelf))
        {
            yield return new WaitForSeconds(1);
        }
    }

    private Vector3 GetRandomSpawnPointPosition()
    {
        var enableSpawnPoints = SpawnPoints.Where(sp => sp.IsEnable).ToList();
        var randomSpawnPoint = enableSpawnPoints[Random.Range(0, enableSpawnPoints.Count)];
        return randomSpawnPoint.transform.position;
    }
}
