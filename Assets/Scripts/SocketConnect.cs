using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
using System;


public class SocketConnect  {
    private Socket mySocket;
    private static SocketConnect st;
    public static System.Object datalock = new System.Object();
    Thread thread;

    SocketConnect(string ipStr) {
        mySocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        IPAddress ip = IPAddress.Parse(ipStr);//将ip地址字符串转换成系统Ip格式
        Debug.Log("IP地址转换："+ipStr+""+"---"+ip);
        IPEndPoint ipEndPoint = new IPEndPoint(ip,2016);//地址+端口号
        IAsyncResult result = mySocket.BeginConnect(ipEndPoint, new AsyncCallback(ConnectCallBack), mySocket);
        bool connectsuccesss = result.AsyncWaitHandle.WaitOne(5000, true);//这里理解成每隔一定时间发送一次连接请求，成功则返回true
        if (connectsuccesss) {
            thread = new Thread(new ThreadStart(GetMessage));//这里声明一个线程的同时，赋予一个新的线程
            thread.IsBackground = true;
            thread.Start();
        }
    }

    void ConnectCallBack(IAsyncResult iAsyncResult) {
        Debug.Log("连接成功。。。");
    }

    public static SocketConnect getSocketInstance(string ip) {
        if (st == null) st = new SocketConnect(ip);
        return st;
    }

    private void GetMessage() {//接收数据包的线程
        while (true) {
            try {
                int dataLength = ReadInt();//数据包意义上的包头部分：接下来的数据长度
                byte[] dataContent = ReadPackage(dataLength);//数据包的包体部分：数据
                SplitPackage(dataLength,dataContent);//拆分数据包
            }
            catch (Exception e){
                Debug.Log("断开连接，无法继续获取信息");
                Debug.Log(e.Message);
            }
        }
    }

    private int ReadInt() {
        byte[] bInt = ReadPackage(4);//一个int占用四个字节
        return ByteUtil.byteArray2Int(bInt,0);
    }

    private byte[] ReadPackage(int length) {
        byte[] bPackage=new byte[length];
        //从绑定的 Socket 套接字接收数据，将数据存入接收缓冲区..获取到的是第一次接受数据包的字节长度
        int cPackageLength = mySocket.Receive(bPackage);
        while (cPackageLength != length) {//接收到的长度与包头提供的长度不相同，则一直接收
            int receivedLength = cPackageLength;//记录已经接收到的长度
            byte[] tempData = new byte[length - cPackageLength];
            int count = mySocket.Receive(tempData);
            cPackageLength += count;//更新接收到的长度
            if (count > 0) {                //如果又接收了数据，则进行合并
                for (int i = 0;i < count;i++) {
                    bPackage[receivedLength + i] = tempData[i];
                }
            }
        }
        return bPackage;
    }

    private void SplitPackage(int dataLength,byte[] dataContent) {
        if (NetworkData.GameState == NetworkData.CONNECTING && dataLength == 4) {
            NetworkData.ClientNumber = ByteUtil.byteArray2Int(dataContent, 0);
            Debug.Log("客户端编号传值：" + NetworkData.ClientNumber);
        }else if(NetworkData.ClientNumber!=0&&NetworkData.GameState==NetworkData.CONNECTING&& dataLength == 9) {
            Debug.Log("是否游戏进入到准备阶段："+Encoding.ASCII.GetString(dataContent));
            if (Encoding.ASCII.GetString(dataContent) == "<#READY#>") {
                Debug.Log("get<Ready>");
                NetworkData.ConnectionState = NetworkData.ON_LINE;
                NetworkData.GameState = NetworkData.CHOOSEPLAYER;
            }
        }
        if (NetworkData.GameState == NetworkData.CHOOSEPLAYER && dataLength == 8) {
            if (NetworkData.ClientNumber == 1) {
                NetworkData.EnemyChoosePlayer = ByteUtil.byteArray2Int(dataContent, 4);
            }
            else if (NetworkData.ClientNumber == 2) {
                NetworkData.EnemyChoosePlayer = ByteUtil.byteArray2Int(dataContent, 0);
            }
        }
        else if (NetworkData.GameState == NetworkData.WAITGAMESTART && dataLength == 13) {
            if (Encoding.ASCII.GetString(dataContent) == "<#LOADLEVEL#>") {
                NetworkData.GameState = NetworkData.LOADLEVEL;
            }
        }
        else if (NetworkData.GameState == NetworkData.LOADLEVEL && dataLength == 13) {
            if (Encoding.ASCII.GetString(dataContent) == "<#GAMESTART#>") {
                Debug.Log("GameStart");
                NetworkData.GameState = NetworkData.GAMESTART;
            }
        }
        else if (NetworkData.GameState == NetworkData.GAMESTART&&dataLength==48) {
            lock (datalock) {
                if (NetworkData.ClientNumber == 1) {
                    NetworkData.enemyPosX = ByteUtil.byteArray2Float(dataContent,24);
                    NetworkData.enemyPosY = ByteUtil.byteArray2Float(dataContent,28);
                    NetworkData.enemyPosZ = ByteUtil.byteArray2Float(dataContent,32);
                    NetworkData.enemyRotX = ByteUtil.byteArray2Float(dataContent,36);
                    NetworkData.enemyRotY = ByteUtil.byteArray2Float(dataContent,40);
                    NetworkData.enemyRotZ = ByteUtil.byteArray2Float(dataContent,44);
                }
                else if (NetworkData.ClientNumber == 2) {
                    NetworkData.enemyPosX = ByteUtil.byteArray2Float(dataContent, 0);
                    NetworkData.enemyPosY = ByteUtil.byteArray2Float(dataContent, 4);
                    NetworkData.enemyPosZ = ByteUtil.byteArray2Float(dataContent, 8);
                    NetworkData.enemyRotX = ByteUtil.byteArray2Float(dataContent, 12);
                    NetworkData.enemyRotY = ByteUtil.byteArray2Float(dataContent, 16);
                    NetworkData.enemyRotZ = ByteUtil.byteArray2Float(dataContent, 20);
                }
            }
        }
        else if (NetworkData.GameState == NetworkData.GAMESTART && dataLength == 8) {
            lock (datalock) {
                if (NetworkData.ClientNumber == 1) {
                    NetworkData.IsEnemyRun = ByteUtil.byteArray2Int(dataContent, 0);
                }
                else if (NetworkData.ClientNumber == 2) {
                    NetworkData.IsEnemyRun = ByteUtil.byteArray2Int(dataContent, 4);
                }
            }
        }

    }

    public void SendMessage(byte[] willSendContent) {
        if (!mySocket.Connected) Debug.Log("断开连接（sendMessage）");
        try {
            int dataLength=willSendContent.Length;
            mySocket.Send(ByteUtil.int2ByteArray(dataLength), SocketFlags.None);
            mySocket.Send(willSendContent, SocketFlags.None);
        }
        catch (Exception e){
            Debug.Log("断开连接（sendMessage）");
            Debug.Log(e.Message);
        }
    }

    public void Close() {
        mySocket.Close();
        thread.Abort();
    }
}
