using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyEventTrigger : MonoBehaviour
{
    public enum eEVENTTYPE
    {
        PARENTS = 0,
        CHILD,
    }

    public eEVENTTYPE kType;    //타입
    public int kRoomObjID;      //룸오브젝트 아이디
    public Transform kPosePos;  //포즈위치
    public Transform kIconPos;  //아이콘 표시위치
    public LobbyEventTrigger kParents; //부모 이벤트
    public List<LobbyEventTrigger> kChildList;  //자식 이벤트
    public List<GameObject> kOnOffList;
}
