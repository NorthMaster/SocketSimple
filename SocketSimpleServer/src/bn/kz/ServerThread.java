package bn.kz;

import java.net.ServerSocket;
import java.net.Socket;
import java.util.FormatFlagsConversionMismatchException;

public class ServerThread extends Thread{
	boolean isStartServerAgentThread=false;
	ServerSocket serverSocket;
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		new ServerThread().start();
	}
	public void run()
	{
		try{
			
			serverSocket=new ServerSocket(2016);
			System.out.println("��ʼ����2016�˿ڡ�����");
			isStartServerAgentThread=true;
			new ActionThread().start();;
		}catch(Exception e){
			e.printStackTrace();
		}
		while(isStartServerAgentThread)
		{
			try{
				Socket socket=serverSocket.accept();
				System.out.println("�ͻ��� +1");
				new ServerAgentThread(socket).start();
			}catch (Exception e){
				e.printStackTrace();
			}
		}
	}

}
