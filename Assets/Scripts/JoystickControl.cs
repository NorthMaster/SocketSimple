using UnityEngine;
using System.Collections;

public class JoystickControl : MonoBehaviour {
   

    void Awake() {
       
    }

    public void OnEnable() {
            Debug.Log("摇杆使用事件调用，对其注册监听");
            EasyJoystick.On_JoystickMove += JoystickMove;
            EasyJoystick.On_JoystickMoveEnd += JoystickMoveEnd;
    }

    public void OnDisable() {
        EasyJoystick.On_JoystickMove -= JoystickMove;
        EasyJoystick.On_JoystickMoveEnd -= JoystickMoveEnd;
    }

    void JoystickMove(MovingJoystick move) {
        if (move.joystickName == "yaogan") {
            Debug.Log("移动。。。");

            NetworkData.IsMeRun=this.gameObject.GetComponent<Animator>().GetInteger("state");
            Debug.Log("获取到的移动状态数据：" + this.gameObject.GetComponent<Animator>().GetInteger("state"));

            float joyPositionX = move.joystickAxis.x;
            float joyPositionY = move.joystickAxis.y;

            if (joyPositionX != 0 || joyPositionY != 0) {
                transform.LookAt(new Vector3(transform.position.x+joyPositionX,transform.position.y,transform.position.z+joyPositionY));
                transform.Translate(Vector3.forward*Time.deltaTime * 10);
                gameObject.GetComponent<Animator>().SetInteger("state",1);
            }
        }
    }

    void JoystickMoveEnd(MovingJoystick move) {
        Debug.Log("移动结束");
        if (move.joystickName == "yaogan") {
            gameObject.GetComponent<Animator>().SetInteger("state", 2);
        }
    }
}
