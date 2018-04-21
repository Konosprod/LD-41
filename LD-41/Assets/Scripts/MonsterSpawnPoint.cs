using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnPoint : MonoBehaviour {

    public List<GameObject> monsterPrefabs;

    public List<GameObject> SpawnMonsters(int nb)
    {
        List<GameObject> monsters = new List<GameObject>();
        
        for(int i=0; i< nb; i++)
        {
            GameObject monster = Instantiate(monsterPrefabs[Random.Range(0, monsterPrefabs.Count)], new Vector3(transform.position.x + Random.Range(-4f, 4f), transform.position.y, transform.position.z), transform.rotation);
            monsters.Add(monster);
        }

        return monsters;
    }
}
