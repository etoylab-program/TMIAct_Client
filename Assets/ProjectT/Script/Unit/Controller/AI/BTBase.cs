
using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;


public class BTBase : MonoBehaviour
{
    public enum eTargetOnStart
    {
        None = 0,
        Player,
        OtherObject,
    }


    private Unit        mOwner  = null;
    private BTRoot      mBTRoot = null;
    private Coroutine   mCr     = null;

    public bool             IsStop                  { get; private set; } = false;
    public float            attackImpossibleTime    { get; private set; }
    public eTargetOnStart   targetOnStart           { get; private set; }
    public bool             CheckAggro              { get; private set; }
    public float            AggroHpPercentage       { get; private set; }
    public float            AggroEndTime            { get; private set; }


    public Vector3 GetRawDirection()
    {
        return GetDirection();
    }

    public Vector3 GetDirection()
    {
        if (mOwner == null)
            return Vector3.zero;

        return mOwner.transform.forward;
    }

    public bool LoadBT(string name)
    {
        mOwner = GetComponent<Unit>();
        if (mOwner == null)
        {
            Debug.LogError("Unit 컴포넌트가 필요합니다.");
            return false;
        }

        TextAsset textAsset = ResourceMgr.Instance.LoadFromAssetBundle("ai", string.Format("AI/{0}.json", name)) as TextAsset;
        if(textAsset == null)
        {
            Debug.LogError(name + " AI파일을 읽어올 수 없습니다.");
            return false;
        }

		JsonData rawData = JsonMapper.ToObject(textAsset.text);
		if(rawData == null || rawData.Count <= 0)
		{
			return false;
		}

		JsonData rootData = Utility.GetJsonDataOrNull(rawData, "Root");
		if (rootData == null || rootData.Count <= 0)
		{
			return false;
		}

		mBTRoot = new BTRoot("Root", mOwner);

		string str = Utility.GetJsonValue(rootData, "TargetOnStart", "");
        if (!string.IsNullOrEmpty(str))
        {
            targetOnStart = (eTargetOnStart)Enum.Parse(typeof(eTargetOnStart), str);
        }

        CheckAggro = false;
        AggroHpPercentage = 0.6f;
        AggroEndTime = 3.0f;

		str = Utility.GetJsonValue(rootData, "CheckAggro", "");
        if (!string.IsNullOrEmpty(str))
        {
			string[] split = Utility.Split(str, ','); //str.Split(',');
            
            if (split.Length > 2)
            {
                AggroEndTime = Utility.SafeParse(split[2]);
            }

            if (split.Length > 1)
            {
                AggroHpPercentage = Utility.SafeParse(split[1]) / 100.0f;
            }

            if(split.Length > 0)
            {
                CheckAggro = split[0].CompareTo("true") == 0 ? true : false;
            }
        }

		float time = Utility.GetJsonValue(rootData, "AttackImpossibleTime", 0.0f);
		if (time == 0.0f)
		{
			float min = Utility.GetJsonValue(rootData, "AttackImpossibleTimeMin", 0.0f);
			float max = Utility.GetJsonValue(rootData, "AttackImpossibleTimeMax", 0.0f);

			if (max > 0.0f)
			{
				int r = UnityEngine.Random.Range((int)(min * 10.0f), (int)(max * 10.0f));
				attackImpossibleTime = (float)r / 10.0f;
			}
		}
		else
		{
			attackImpossibleTime = time;
		}

		JsonData nodeData = Utility.GetJsonDataOrNull(rootData, "Node");
		if(nodeData != null)
		{
			foreach (JsonData child in nodeData)
			{
				IEnumerator<string> e = child.Keys.GetEnumerator();
				while (e.MoveNext())
				{
					BTNode btChildNode = AddNodes(e.Current, child[e.Current]);
					if (btChildNode == null)
					{
						continue;
					}

					mBTRoot.AddChild(btChildNode);
				}
			}
		}

		return true;
    }

	private BTNode AddNodes(string name, JsonData parentData)
    {
		string strType = Utility.GetJsonValue(parentData, "Type", "");

        BTNode btNode = null;
        if (strType == eBTType.Sequence.ToString() || strType == eBTType.Selector.ToString())
        {
            if(strType == eBTType.Sequence.ToString())
                btNode = new BTSequence(name, mOwner);
            else
                btNode = new BTSelector(name, mOwner);

			JsonData nodeData = Utility.GetJsonDataOrNull(parentData, "Node");
			if (nodeData != null)
			{
				foreach (JsonData child in nodeData)
				{
					IEnumerator<string> e = child.Keys.GetEnumerator();
					while (e.MoveNext())
					{
						BTNode btChildNode = AddNodes(e.Current, child[e.Current]);
						if (btChildNode == null)
							continue;

						btNode.AddChild(btChildNode);
					}
				}
			}
        }
        else if (strType == eBTType.Condition.ToString())
        {
            btNode = new BTCondition(name, mOwner);

            string strReference = Utility.GetJsonValue(parentData, "Reference", "");
            string strValue = Utility.GetJsonValue(parentData, "Value", "");
			string strCompare = Utility.GetJsonValue(parentData, "Compare", "");
			string strParam = Utility.GetJsonValue(parentData, "Param", "");

            BTCondition btCond = btNode as BTCondition;
            if (btCond == null)
            {
                Debug.LogError("BTCondition으로 캐스팅 실패");
                return null;
            }

            btCond.Set(strReference, strValue, strCompare, strParam);
        }
        else if (strType == eBTType.Action.ToString())
        {
            btNode = new BTAction(name, mOwner);

            string strCommand = Utility.GetJsonValue(parentData, "Command", "");
            string direction = Utility.GetJsonValue(parentData, "Direction", "");
            float min = Utility.GetJsonValue(parentData, "MinValue", 0.0f);
            float max = Utility.GetJsonValue(parentData, "MaxValue", 0.0f);
            string strDistCommand = Utility.GetJsonValue(parentData, "DistCommand", "");
            string strPosByTargetDir = Utility.GetJsonValue(parentData, "PosByTargetDir", "");
            int lookAtByRotate = Utility.GetJsonValue(parentData, "LookAtByRotation", 0);

            BTAction btAction = btNode as BTAction;
            if (btAction == null)
            {
                Debug.LogError("BTAction으로 캐스팅 실패");
                return null;
            }

            btAction.Set(strCommand, direction, min, max, strDistCommand, strPosByTargetDir, lookAtByRotate == 0 ? false : true);
        }
        else if (strType == eBTType.RemoveAction.ToString())
        {
            btNode = new BTRemoveAction(name, mOwner);

            string strCommand = Utility.GetJsonValue(parentData, "Command", "");

            BTRemoveAction btRemoveAction = btNode as BTRemoveAction;
            if (btRemoveAction == null)
            {
                Debug.LogError("BTRemoveAction 캐스팅 실패");
                return null;
            }

            btRemoveAction.Set(strCommand);
        }
        else if (strType == eBTType.HangingAround.ToString())
        {
            btNode = new BTHangingAround(name, mOwner);

            float minValue = Utility.GetJsonValue(parentData, "MinDuration", 0.0f);
			float maxValue = Utility.GetJsonValue(parentData, "MaxDuration", 0.0f);
            string strDistCommand = Utility.GetJsonValue(parentData, "DistCommand", "");
            string strRandomCheckDist = Utility.GetJsonValue(parentData, "RandomCheckDist", "");
            string direction = Utility.GetJsonValue(parentData, "Direction", null);
            string turn = Utility.GetJsonValue(parentData, "Turn", "false");

            BTHangingAround btHangingAround = btNode as BTHangingAround;
            if (btHangingAround == null)
            {
                Debug.LogError("BTHangingAround로 캐스팅 실패");
                return null;
            }

            btHangingAround.Set(minValue, maxValue, strDistCommand, strRandomCheckDist, direction, turn);
        }
        else if(strType == eBTType.CallFunc.ToString())
        {
            btNode = new BTCallFunc(name, mOwner);

            string strFunc = Utility.GetJsonValue(parentData, "Func", "");

            BTCallFunc btCallFunc = btNode as BTCallFunc;
            if (btCallFunc == null)
            {
                Debug.LogError("BTCallFunc로 캐스팅 실패");
                return null;
            }

            btCallFunc.Set(strFunc);
        }
        else if(strType == eBTType.CoolTime.ToString())
        {
            btNode = new BTCoolTime(name, mOwner);

            float coolTime = Utility.GetJsonValue(parentData, "CoolTime", 0.0f); ;

            BTCoolTime btCoolTime = btNode as BTCoolTime;
            if (btCoolTime == null)
            {
                Debug.LogError("BTCoolTime 캐스팅 실패");
                return null;
            }

            btCoolTime.Set(coolTime);
        }
        else if(strType == eBTType.Input.ToString())
        {
            btNode = new BTInput(name, mOwner);

            string direction = Utility.GetJsonValue(parentData, "Direction", "");
            string button = Utility.GetJsonValue(parentData, "Button", "");
            string repeat = Utility.GetJsonValue(parentData, "Repeat", "");
            string waitTime = Utility.GetJsonValue(parentData, "WaitTime", "");
            string waitCutFrame = Utility.GetJsonValue(parentData, "WaitCutFrame", "");
            string waitAni = Utility.GetJsonValue(parentData, "WaitAni", "");
            string heighest = Utility.GetJsonValue(parentData, "Highest", "");
            string minChargeTime = Utility.GetJsonValue(parentData, "MinChargeTime", "");
            string maxChargeTime = Utility.GetJsonValue(parentData, "MaxChargeTime", "");

            BTInput btInput = btNode as BTInput;
            if (btInput == null)
            {
                Debug.LogError("BTInput 캐스팅 실패");
                return null;
            }

            btInput.Set(direction, button, repeat, waitTime, waitCutFrame, waitAni, heighest, minChargeTime, maxChargeTime);
        }
        else if(strType == eBTType.WaitAction.ToString())
        {
            btNode = new BTWaitAction(name, mOwner);

            string command = Utility.GetJsonValue(parentData, "Command", "");

            BTWaitAction btWaitAction = btNode as BTWaitAction;
            if (btWaitAction == null)
            {
                Debug.LogError("BTWaitAction 캐스팅 실패");
                return null;
            }

            btWaitAction.Set(command);
        }
        else
        {
            Debug.LogError(strType + "은 존재하지 않는 노드 타입입니다.");
            return null;
        }

        return btNode;
    }

    public void StartBT()
    {
        if(mCr != null)
        {
            StopBT();
        }

        IsStop = false;
        mCr = StartCoroutine(mBTRoot.Invoke(mBTRoot));
    }

    public void StopBT()
    {
        if (mCr == null)
            return;

        Debug.Log("################################################" + mOwner.name + " StopBT");

        IsStop = true;
        mBTRoot.InitRandomValues();

        StopAllCoroutines();
        mCr = null;
    }
}