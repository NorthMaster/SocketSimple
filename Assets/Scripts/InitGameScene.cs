using UnityEngine;
using System.Collections;
using System.Text;

public class InitGameScene : MonoBehaviour {
    public GameObject[] model_me;
    public GameObject[] model_enemy;
    private Vector3 mePosition;
    private Vector3 enemyPosition;
    private GameObject me;
    private GameObject enemy;
    // Use this for initialization
    void Awake() {
        mePosition = new Vector3(-24, 0, 35);
        enemyPosition = new Vector3(32, 0, 32);
        InitMeAndEnemy();
    }

    // Update is called once per frame
    void Update() {

    }

    void InitMeAndEnemy() {
        lock (SocketConnect.datalock) {
            NetworkData.playerPosX = NetworkData.ClientNumber == 1 ? mePosition.x : enemyPosition.x;
            NetworkData.playerPosY = NetworkData.ClientNumber == 1 ? mePosition.y : enemyPosition.y;
            NetworkData.playerPosZ = NetworkData.ClientNumber == 1 ? mePosition.z : enemyPosition.z;
            NetworkData.playerRotX = 0;
            NetworkData.playerRotY = 0;
            NetworkData.playerRotZ = 0;
        }
        me = Instantiate(model_me[NetworkData.MeChoosePlayer - 1], NetworkData.ClientNumber == 1 ? mePosition : enemyPosition, Quaternion.identity) as GameObject;
        enemy = Instantiate(model_enemy[NetworkData.EnemyChoosePlayer - 1], NetworkData.ClientNumber == 1 ? enemyPosition : mePosition, Quaternion.identity) as GameObject;
        enemy.GetComponent<JoystickControl>().OnDisable();
        this.gameObject.AddComponent<TimeSendMessage>();
        this.gameObject.AddComponent<StepEnemy>();
        byte[] bSendConnect = Encoding.ASCII.GetBytes("<#GAMESTART#>");
        UIContrl.sc.SendMessage(bSendConnect);  
            //AddTag("tag_me", me);
            //AddTag("tag_enemy",enemy);
            //Debug.Log("tag名字："+me.tag.ToString()+"---"+enemy.tag.ToString());
        
    }


    //using UnityEditor;
    //void AddTag(string tag, GameObject obj) {
    //    if (isHaveTag(tag)) return;
    //    SerializedObject tagManager = new SerializedObject(obj);//序列化物体
    //    SerializedProperty it = tagManager.GetIterator();//序列化属性
    //    while (it.NextVisible(true)) {//属性的可见性
    //        if (it.name == "m_TagString") {
    //            it.stringValue = tag;
    //            tagManager.ApplyModifiedProperties();
    //        }
    //    }
    //}

    //bool isHaveTag(string str) {
    //    for (int i = 0;i < UnityEditorInternal.InternalEditorUtility.tags.Length;i++) {
    //        if (UnityEditorInternal.InternalEditorUtility.tags[i].Equals(str)) return true;
    //    }
    //    return false;
    //}
}
