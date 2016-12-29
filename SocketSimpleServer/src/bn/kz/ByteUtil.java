package bn.kz;

public class ByteUtil {
	public static byte[] float2ByteArray(float fnum) {
	    int num=Float.floatToIntBits(fnum);
	    //System.out.println(num);
		byte[] bt = new byte[4];
	    for (int i = 0; (i < 4) && (i < 4); i++) {
	        bt[i] = (byte) (num >> 8 * i & 0xFF);
	        //System.out.print(bt[i]+"  ");
	    } 
	    return bt;
	}
	public static float byteArray2Float(byte[] bt) {
	    int num = 0;
	    byte bLoop;
	    for (int i = 0; i < bt.length; i++) {
	        bLoop = bt[i];
	        num += (bLoop & 0xFF) << (8 * i);
	    }
	    return Float.intBitsToFloat(num);
	}

	public static byte[] int2ByteArray(int num) {
	    byte[] bt = new byte[4];
	    for (int i = 0; (i < 4) && (i < 4); i++) {
	        bt[i] = (byte) (num >> 8 * i & 0xFF);
	        //System.out.print(bt[i]+"  ");
	    } 
	    return bt;
	}
	public static int byteArray2Int(byte[] bt) {
	    int num = 0;
	    byte bLoop;
	    for (int i = 0; i < bt.length; i++) {
	        bLoop = bt[i];
	        num += (bLoop & 0xFF) << (8 * i);
	    }
	    return num;
	}
}
