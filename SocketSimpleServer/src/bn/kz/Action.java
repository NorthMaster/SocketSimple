package bn.kz;

import java.awt.datatransfer.FlavorEvent;

public class Action {
	int clietNumber;
	float posX;
	float posY;
	float posZ;
	float rotX;
	float rotY;
	float rotZ;
	
	public Action(int clientNumber,float posX,float posY,float posZ,
			float rotX,float rotY,float rotZ)
	{
		this.clietNumber=clientNumber;
		this.posX=posX;
		this.posY=posY;
		this.posZ=posZ;
		this.rotX=rotX;
		this.rotY=rotY;
		this.rotZ=rotZ;
	}
	
	public void doAction(){//记录客户端传进来的信息
		if(clietNumber==1){
			GameData.Client_1_PosX=posX;
			GameData.Client_1_PosY=posY;
			GameData.Client_1_PosZ=posZ;
			GameData.Client_1_RotX=rotX;
			GameData.Client_1_RotY=rotY;
			GameData.Client_1_RotZ=rotZ;
		}else if(clietNumber==2){
			GameData.Client_2_PosX=posX;
			GameData.Client_2_PosY=posY;
			GameData.Client_2_PosZ=posZ;
			GameData.Client_2_RotX=rotX;
			GameData.Client_2_RotY=rotY;
			GameData.Client_2_RotZ=rotZ;
		}
	}
}
