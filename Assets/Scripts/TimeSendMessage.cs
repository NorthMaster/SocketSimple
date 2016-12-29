using UnityEngine;
using System.Collections;
using System.Text;
using System.Net;

public class TimeSendMessage : MonoBehaviour {
    private GameObject me;
    private float timer;
	// Use this for initialization
	void Start () {
        if (me == null) {
            me = GameObject.FindGameObjectWithTag("tag_me");
            //Debug.Log("根据tag找到的当前系统控制的物体："+me.name);
        }
        timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate() {
        if (NetworkData.GameState != NetworkData.GAMESTART) return;//如果游戏并没有开始，不进行数据的发送
        timer += Time.fixedDeltaTime;
        if (timer > 0.05f) {
            Debug.Log("发送主客户端信息的方法，每隔0.05调用");
            SendMeMessage();
            timer = 0;
        }
    }

    void SendMeMessage() {
        Vector3 mePosition = me.transform.position;
        Vector3 meRotation = me.transform.localRotation.eulerAngles;
        //Debug.Log("发送信息的方法。。。已经获取位置信息：" + mePosition + "" + meRotation);
        lock (SocketConnect.datalock) {//修改全局数据的时候是不能被引用的
            NetworkData.playerPosX = mePosition.x;
            NetworkData.playerPosY = mePosition.y;
            NetworkData.playerPosZ = mePosition.z;
            NetworkData.playerRotX = meRotation.x;
            NetworkData.playerRotY = meRotation.y;
            NetworkData.playerRotZ = meRotation.z;
        }
        UIContrl.sc.SendMessage(PackageData());
        UIContrl.sc.SendMessage(ByteUtil.int2ByteArray(NetworkData.IsMeRun));//发送移动状态数据
    }

    byte[] PackageData() {
        Debug.Log("打包第一视角人物数据");
        byte[]data=new byte[24];
        byte[][]tempData=new byte[6][];
        lock (SocketConnect.datalock) {
            tempData.SetValue(ByteUtil.float2ByteArray(NetworkData.playerPosX), 0);
            tempData.SetValue(ByteUtil.float2ByteArray(NetworkData.playerPosY), 1);
            tempData.SetValue(ByteUtil.float2ByteArray(NetworkData.playerPosZ), 2);
            tempData.SetValue(ByteUtil.float2ByteArray(NetworkData.playerRotX), 3);
            tempData.SetValue(ByteUtil.float2ByteArray(NetworkData.playerRotY), 4);
            tempData.SetValue(ByteUtil.float2ByteArray(NetworkData.playerRotZ), 5);
        }
        int index=0;
        for (int i = 0;i < tempData.GetLength(0);i++) {
            for (int j = 0;j < tempData[i].Length;j++) {
                data[index]=tempData[i][j];
                index++;
            }
        }
        return data;
    }
}
