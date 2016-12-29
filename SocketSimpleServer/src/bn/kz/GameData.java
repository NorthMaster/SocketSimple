package bn.kz;

public class GameData {
	public static Object lock=new Object();
	public static int GameState=1;
	public static int Client_1_Choose=0;
	public static int Client_2_Choose=0;
	public static int ReadyClient=0;
	public static int Client_1_Run=0;
	public static int Client_2_Run=0;
	
	public static final int CONNECTING=1;
	public static final int CHOOSEPLAYER=2;
	public static final int LOADLEVEL=3;
	public static final int GAMESTART=4;
	
	public static float Client_1_PosX;
	public static float Client_1_PosY;
	public static float Client_1_PosZ;
	public static float Client_1_RotX;
	public static float Client_1_RotY;
	public static float Client_1_RotZ;
	
	public static float Client_2_PosX;
	public static float Client_2_PosY;
	public static float Client_2_PosZ;
	public static float Client_2_RotX;
	public static float Client_2_RotY;
	public static float Client_2_RotZ;
}
