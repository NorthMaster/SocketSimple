package bn.kz;

public class RunAction {
	int clietNumber;
	int RunState;
	
	public RunAction(int clientNumber,int RunState)
	{
		this.clietNumber=clientNumber;
		this.RunState=RunState;
	}
	
	public void doAction(){//记录客户端传进来的信息
		if(clietNumber==1){
			GameData.Client_1_Run=RunState;
		}else if(clietNumber==2){
			GameData.Client_2_Run=RunState;
		}
	}
}
