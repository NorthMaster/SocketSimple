package bn.kz;

public class ActionThread extends Thread {
	static final int SLEEP=1;
	
	@Override
	public void run(){
		while(true)
		{
			//System.out.println("action thread run£¨£©");
			Action action=null;
			synchronized(GameData.lock)
			{
				action=ServerAgentThread.aq.poll();
			}
			if(action!=null)
			{
				action.doAction();
				ServerAgentThread.SendMessage();	
			}
			try{
				Thread.sleep(SLEEP);
			}catch(Exception e){
				e.printStackTrace();
			}
		}
	}
}
