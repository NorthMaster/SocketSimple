using UnityEngine;
using System.Collections;

public class StepEnemy : MonoBehaviour {
    private GameObject enemy;
    private Vector3 enemyPos;
    private Vector3 enemyRot;
	// Use this for initialization
	void Start () {
        if (enemy == null) {
            enemy = GameObject.FindGameObjectWithTag("tag_enemy");
        }
        enemyPos = Vector3.zero;
        enemyRot = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
        if (NetworkData.GameState != NetworkData.GAMESTART) return;
        lock (SocketConnect.datalock) {
            Debug.Log("拿到敌人数据，加锁状态");
            enemyPos = new Vector3(NetworkData.enemyPosX, NetworkData.enemyPosY, NetworkData.enemyPosZ);
            enemyRot = new Vector3(NetworkData.enemyRotX, NetworkData.enemyRotY, NetworkData.enemyRotZ);
        }
        enemy.transform.position = Vector3.Lerp(enemy.transform.position, enemyPos, Time.deltaTime*20f);
        enemy.transform.eulerAngles = enemyRot;
        enemy.gameObject.GetComponent<Animator>().SetInteger("state", NetworkData.IsEnemyRun);
	}

    void FixedUpdate() {
        
    }
}
