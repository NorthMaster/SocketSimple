using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System;
using UnityEngine.SceneManagement;


public class UIContrl : MonoBehaviour {
    public GameObject startUI;
    public GameObject waitConnectUI;
    public GameObject chooseUI;

    public Button startNetwork;
    public Button startGame;
    public Button choose_1;
    public Button choose_2;

    public InputField ipInputField;
    public Image waitConnect;

    public Text waitStateText;
    public Text meChoose;
    public Text enemyChoose;

    public static SocketConnect sc;

    private int lastMeChoose = -1;
    private int lastEnemyChoose = -1;
	// Use this for initialization
	void Start () {
        startNetwork.onClick.AddListener(StartNetworkButtonClick);
        startGame.onClick.AddListener(StartGameButtonClick);
        choose_1.onClick.AddListener(ChooseOneHeroButtonClick);
        choose_2.onClick.AddListener(ChooseTwoHeroButtonClick);
	}
	
	// Update is called once per frame
	void Update () {
        
        if (NetworkData.GameState == NetworkData.CONNECTING) {
            waitConnectUI.SetActive(true);
            waitStateText.text = "第一次连接。。。";
            waitConnect.transform.Rotate(Vector3.forward, Time.deltaTime * 15);
        }
        if (NetworkData.GameState == NetworkData.WAITGAMESTART) {
            waitConnectUI.SetActive(true);
            waitStateText.text = "选人完成 等待确认。。。";
            waitConnect.transform.Rotate(Vector3.forward, -Time.deltaTime * 15);
        }
        if (NetworkData.GameState == NetworkData.CHOOSEPLAYER) {
            waitConnectUI.SetActive(false);
            chooseUI.SetActive(true);
            if (NetworkData.MeChoosePlayer != lastMeChoose || NetworkData.EnemyChoosePlayer != lastEnemyChoose) {
                meChoose.text = NetworkData.MeChoosePlayer + "";
                enemyChoose.text = NetworkData.EnemyChoosePlayer + "";

                lastEnemyChoose = NetworkData.EnemyChoosePlayer;
                lastMeChoose = NetworkData.MeChoosePlayer;
            }    
        }
        if (NetworkData.GameState == NetworkData.LOADLEVEL) {
            waitStateText.text = "全部连入，切换场景。。。";
            Debug.Log("全部连入，切换场景");
            //Application.LoadLevel ("gamescene");
            SceneManager.LoadScene("gamescene");
        }
	}

    void StartNetworkButtonClick() {
        if (sc == null) {
            string ip = ipInputField.text;
            Debug.Log("调用点击开始网络连接的按钮，ip：" + ip);
            sc = SocketConnect.getSocketInstance(ip);
            byte[] bSendConnect = Encoding.ASCII.GetBytes("<#CONNECT#>");
            sc.SendMessage(bSendConnect);
            NetworkData.GameState = NetworkData.CONNECTING;
            startUI.SetActive(false);
        }
    }

    void ChooseOneHeroButtonClick() {
        NetworkData.MeChoosePlayer =1;
        sc.SendMessage(ByteUtil.int2ByteArray(1));
    }
    void ChooseTwoHeroButtonClick() {
        NetworkData.MeChoosePlayer = 2;
        sc.SendMessage(ByteUtil.int2ByteArray(2));
    }

    void StartGameButtonClick() {//确认选人
        NetworkData.GameState = NetworkData.WAITGAMESTART;
        byte[] bSendConnect = Encoding.ASCII.GetBytes("<#LOADLEVEL#>");
        sc.SendMessage(bSendConnect);
        chooseUI.SetActive(false);
    }

    void OnApplicationQuit() {
        if (sc != null) {
            sc.Close();
        }
    }  
}
