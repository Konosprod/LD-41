using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnPoint : MonoBehaviour {
    // Indicates wether the spawnpoint is done or not (= spawned all the mobs it had to spawn)
    public bool isDone = false;

    public List<int> waves = new List<int>();
    public List<int> nbMobsPerWaves = new List<int>();

    public List<GameObject> monsterPrefabs;

    public List<GameObject> SpawnMonsters(int currentTurn)
    {
        List<GameObject> monsters = new List<GameObject>();
        
        if(waves.Contains(currentTurn))
        {
            int index = waves.FindIndex(x => x == currentTurn);
            int nbMobs = nbMobsPerWaves[index];
            //Debug.Log("Current turn : " + currentTurn + ", index : " + index + ", nbMobs : " + nbMobs);
            // Spawn the chosen amount of mobs
            for (int i = 0; i < nbMobs; i++)
            {
                GameObject prefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Count)]; // We take a random prefab among those offered for the spawn (allows reskin of mobs for variety)
                Vector3 spawnPos = new Vector3(transform.position.x + Random.Range(-4f, 4f), transform.position.y, transform.position.z + Random.Range(-4f, 4f));
                //bool spawned = false;
                GameObject monster = null;
                /*while (!spawned)
                {
                    if (!Physics.CheckSphere(spawnPos, 1f, 1 << 8))
                    {
                        spawned = true;*/
                monster = Instantiate(prefab.gameObject, spawnPos, prefab.gameObject.transform.rotation);
                    /*}
                    spawnPos = new Vector3(transform.position.x + Random.Range(-4f, 4f), transform.position.y, transform.position.z);
                }*/
                monsters.Add(monster);
            }

            if(index == waves.Count-1)
            {
                //Debug.Log("I'm done spawning mobs :D");
                isDone = true;
            }
        }
        
        return monsters;
    }
}
