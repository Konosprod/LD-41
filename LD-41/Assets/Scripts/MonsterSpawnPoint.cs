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
            GameObject prefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Count)];
            GameObject monster = Instantiate(prefab.gameObject, new Vector3(transform.position.x + Random.Range(-4f, 4f), transform.position.y, transform.position.z), prefab.gameObject.transform.rotation);
            monsters.Add(monster);
        }

        return monsters;
    }
}
