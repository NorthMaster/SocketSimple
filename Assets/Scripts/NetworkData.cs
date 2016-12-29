using UnityEngine;
using System.Collections;

public class NetworkData {
    public static int ConnectionState = 0;//连接状态

    public static int IsMeRun = 0;
    public static int IsEnemyRun = 0;

    public const int ON_LINE = 1;//在线
    public const int FALL_LINE = 2;//掉线

    public static int GameState = 0;//游戏状态

    public const int DISCONNECT = 0;
    public const int CONNECTING = 1;
    public const int CHOOSEPLAYER = 2;
    public const int WAITGAMESTART = 3;
    public const int LOADLEVEL = 4;
    public const int GAMESTART = 5;
    public const int GAMEOVER = 6;

    public static int ClientNumber = 0;//客户端编号
    public static int MeChoosePlayer = 1;
    public static int EnemyChoosePlayer = 2;

    public static float playerPosX;
    public static float playerPosY;
    public static float playerPosZ;
    public static float playerRotX;
    public static float playerRotY;
    public static float playerRotZ;

    public static float enemyPosX;
    public static float enemyPosY;
    public static float enemyPosZ;
    public static float enemyRotX;
    public static float enemyRotY;
    public static float enemyRotZ;

}
