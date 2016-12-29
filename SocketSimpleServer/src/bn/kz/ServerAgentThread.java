package bn.kz;

import java.awt.List;
import java.io.*;
import java.net.*;
import java.nio.channels.NonWritableChannelException;
import java.util.*;

import javax.sound.sampled.AudioFormat.Encoding;

import org.ietf.jgss.GSSManager;

public class ServerAgentThread extends Thread {
	public static int count=0;

	static java.util.List<ServerAgentThread> ulist=new ArrayList<ServerAgentThread>();
	static Queue<Action> aq=new LinkedList<Action>();
	static Queue<RunAction> rq=new LinkedList<RunAction>();
	
	Socket sc;
	DataInputStream din;
	DataOutputStream dout;
	
	static Object lock=new Object();
	int clientNumber=0;
	
	public ServerAgentThread(Socket sc)
	{
		//System.out.println("gouzao serveragent");
		this.sc=sc;
		try{
			din=new DataInputStream(sc.getInputStream());
			dout=new DataOutputStream(sc.getOutputStream());
		}catch (Exception e){
			e.printStackTrace();
		}
	}
	
	@Override
	public void run(){
		while(true)
		{
			try{
				if(din.available()==0) continue;
				//System.out.println("开始第一次读数据");
				int dataLength=ReadInt();
				//System.out.println("datalength"+dataLength);
				byte[] dataPackage=ReadPackage(dataLength);
				SplitPackage(dataPackage);
			}catch (Exception e){
				e.printStackTrace();
			}
		}
	}
	
	int ReadInt(){
		byte[]bint=ReadPackage(4);
		return ByteUtil.byteArray2Int(bint);
	}
	
	byte[] ReadPackage(int dataLength)
	{
		byte[]dataPackage=new byte[dataLength];
		try{
			int state=din.read(dataPackage,0,dataLength);
			while(state!=dataLength)
			{
				int index=state;
				byte[]tempData=new byte[dataLength-state];
				int count=din.read(tempData,0,tempData.length);
				state+=count;
				if(count>0)
				{
					for(int i=0;i<count;i++)
					{
						dataPackage[index+i]=tempData[i];
					}
				}
			}
		}catch(IOException e)
		{
			e.printStackTrace();
		}
		return dataPackage;
	}
	
	void SplitPackage(byte[]dataPackage)
	{
		if(GameData.GameState==GameData.CONNECTING&&dataPackage.length==11){
			count++;
			clientNumber=count;
			ulist.add(this);
			SendMessage(ByteUtil.int2ByteArray(count),this);
			System.out.println("客户端编号："+count);
			if(count>=2)
			{
				String msg="<#READY#>";
				GameData.GameState=GameData.CHOOSEPLAYER;
				//System.out.println("准备好了！！！ready");
				SendMessage(msg.getBytes());
			}
		}else if(GameData.GameState==GameData.CHOOSEPLAYER&&dataPackage.length==4){
			if(clientNumber==1){
				GameData.Client_1_Choose=ByteUtil.byteArray2Int(dataPackage);
			}else if(clientNumber==2){
				GameData.Client_2_Choose=ByteUtil.byteArray2Int(dataPackage);
			}
			byte[]package1Choose=ByteUtil.int2ByteArray(GameData.Client_1_Choose);
			byte[]package2Choose=ByteUtil.int2ByteArray(GameData.Client_2_Choose);
			byte[]bSent=new byte[package1Choose.length+package2Choose.length];
			//组装选人信息
			System.arraycopy(package1Choose,0,bSent,0,package1Choose.length);
			System.arraycopy(package2Choose, 0, bSent, package1Choose.length, package2Choose.length);
			SendMessage(bSent);	
		}else if(GameData.GameState==GameData.CHOOSEPLAYER&&dataPackage.length==13){
			if(new String(dataPackage).equals("<#LOADLEVEL#>")){
				System.out.println("客户端"+clientNumber+"完成选人，等待中。。。");
				GameData.ReadyClient++;
				if(GameData.ReadyClient==2)
				{
					//System.out.println("两个客户端完成选人动作，发送信息");
					SendMessage("<#LOADLEVEL#>".getBytes());
					GameData.GameState=GameData.LOADLEVEL;
					GameData.ReadyClient=0;
				}
			}
		}else if(GameData.GameState==GameData.LOADLEVEL&&dataPackage.length==13){
			if(new String(dataPackage).equals("<#GAMESTART#>")){
				GameData.ReadyClient++;
				if(GameData.ReadyClient==2){
					SendMessage("<#GAMESTART#>".getBytes());
					GameData.GameState=GameData.GAMESTART;
					System.out.println("加载场景完毕  游戏开始");
				}
			}
		}
		if(GameData.GameState==GameData.GAMESTART&&dataPackage.length==24){//传入某客户端的姿态数据，两个客户端都接收
			//System.out.println("接收客户端位置转向数据！！！");
			byte[]dataPosX={dataPackage[0],dataPackage[1],dataPackage[2],dataPackage[3]};
			byte[]dataPosY={dataPackage[4],dataPackage[5],dataPackage[6],dataPackage[7]};
			byte[]dataPosZ={dataPackage[8],dataPackage[9],dataPackage[10],dataPackage[11]};
			byte[]dataRotX={dataPackage[12],dataPackage[13],dataPackage[14],dataPackage[15]};
			byte[]dataRotY={dataPackage[16],dataPackage[17],dataPackage[18],dataPackage[19]};
			byte[]dataRotZ={dataPackage[20],dataPackage[21],dataPackage[22],dataPackage[23]};	
			
			Action action=new Action(
					this.clientNumber,
					ByteUtil.byteArray2Float(dataPosX),
					ByteUtil.byteArray2Float(dataPosY),
					ByteUtil.byteArray2Float(dataPosZ),
					ByteUtil.byteArray2Float(dataRotX),
					ByteUtil.byteArray2Float(dataRotY),
					ByteUtil.byteArray2Float(dataRotZ)	
					);
			//System.out.println("接收到了传进来的数据，客户端编号"+this.clientNumber+":"+ByteUtil.byteArray2Float(dataPosX)+","+
					//ByteUtil.byteArray2Float(dataPosY)+","+ByteUtil.byteArray2Float(dataPosZ));
			synchronized(lock){//放入动作队列 
				aq.offer(action);
			}
		}else if(GameData.GameState==GameData.GAMESTART&&dataPackage.length==4){//传入某客户端的跑动数据，两个客户端都接收	
			RunAction runaction=new RunAction(
					this.clientNumber,
					ByteUtil.byteArray2Int(dataPackage)
					);
			//System.out.println("接收到了传进来的数据，客户端编号"+this.clientNumber+":"+ByteUtil.byteArray2Float(dataPosX)+","+
					//ByteUtil.byteArray2Float(dataPosY)+","+ByteUtil.byteArray2Float(dataPosZ));
			synchronized(lock){//放入动作队列 
				rq.offer(runaction);
			}
		}
	}
	
	public static void SendMessage(){
		byte[]p1PosX=ByteUtil.float2ByteArray(GameData.Client_1_PosX);
		byte[]p1PosY=ByteUtil.float2ByteArray(GameData.Client_1_PosY);
		byte[]p1PosZ=ByteUtil.float2ByteArray(GameData.Client_1_PosZ);
		
		byte[]p1RotX=ByteUtil.float2ByteArray(GameData.Client_1_RotX);
		byte[]p1RotY=ByteUtil.float2ByteArray(GameData.Client_1_RotY);
		byte[]p1RotZ=ByteUtil.float2ByteArray(GameData.Client_1_RotZ);
		
		byte[]p2PosX=ByteUtil.float2ByteArray(GameData.Client_2_PosX);
		byte[]p2PosY=ByteUtil.float2ByteArray(GameData.Client_2_PosY);
		byte[]p2PosZ=ByteUtil.float2ByteArray(GameData.Client_2_PosZ);
		
		byte[]p2RotX=ByteUtil.float2ByteArray(GameData.Client_2_RotX);
		byte[]p2RotY=ByteUtil.float2ByteArray(GameData.Client_2_RotY);
		byte[]p2RotZ=ByteUtil.float2ByteArray(GameData.Client_2_RotZ);
		
		byte[]dataPackage={
				p1PosX[0],p1PosX[1],p1PosX[2],p1PosX[3],
				p1PosY[0],p1PosY[1],p1PosY[2],p1PosY[3],
				p1PosZ[0],p1PosZ[1],p1PosZ[2],p1PosZ[3],
				
				p1RotX[0],p1RotX[1],p1RotX[2],p1RotX[3],
				p1RotY[0],p1RotY[1],p1RotY[2],p1RotY[3],
				p1RotZ[0],p1RotZ[1],p1RotZ[2],p1RotZ[3],
					
				p2PosX[0],p2PosX[1],p2PosX[2],p2PosX[3],
				p2PosY[0],p2PosY[1],p2PosY[2],p2PosY[3],
				p2PosZ[0],p2PosZ[1],p2PosZ[2],p2PosZ[3],
				
				p2RotX[0],p2RotX[1],p2RotX[2],p2RotX[3],
				p2RotY[0],p2RotY[1],p2RotY[2],p2RotY[3],
				p2RotZ[0],p2RotZ[1],p2RotZ[2],p2RotZ[3]
		};
		for(ServerAgentThread sa:ulist){
			try{
				sa.dout.write(ByteUtil.int2ByteArray(dataPackage.length));
				sa.dout.write(dataPackage);
				sa.dout.flush();
			}catch (Exception e){
				e.printStackTrace();
			}
		}
	}
	
	//本次发送的信息为本客户端的编号，告诉客户端你是第几个连入服务器的
	void SendMessage(byte[]dataPackage,ServerAgentThread sa){
		try{
			sa.dout.write(ByteUtil.int2ByteArray(dataPackage.length));
			sa.dout.write(dataPackage);
			sa.dout.flush();
		}catch (Exception e){
			e.printStackTrace();
		}
	}
	
	//向所有客户端发送数据，告诉准备好了可以下一步
	void SendMessage(byte[]dataPackage){
		try{
			for(ServerAgentThread sa:ulist)
			{
				sa.dout.write(ByteUtil.int2ByteArray(dataPackage.length));
				sa.dout.write(dataPackage);
				sa.dout.flush();
			}
		}catch (Exception e)
		{
			e.printStackTrace();
		}
	}	
}
