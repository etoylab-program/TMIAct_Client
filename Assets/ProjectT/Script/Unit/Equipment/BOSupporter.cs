
using System.Collections.Generic;


public class BOSupporter : BattleOption
{
    private CardData m_data;
    public CardData data { get { return m_data; } }


    public BOSupporter(CharData chardata, Unit owner) : base(owner)
    {
        mOwner = owner;
        mToExecuteType = eToExecuteType.Supporter;

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            if (chardata.EquipCard[i] > (int)eCOUNT.NONE)
            {
                CardData tmpCardData = GameInfo.Instance.GetCardData(chardata.EquipCard[i]);
                if (tmpCardData == null)
                {
                    Debug.LogError(string.Format("{0}번 서포터가 없습니다.", chardata.EquipCard[i]));
                    return;
                }

                mListBattleOptionSet.Clear();
                GameClientTable.BattleOptionSet.Param param = null;
                if (tmpCardData.TableData.SptAddBOSetID1 > 0)
                {
                    param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(tmpCardData.TableData.SptAddBOSetID1);
                    if (param == null)
                    {
                        Debug.LogError(string.Format("{0}번 배틀옵션셋 데이터가 없습니다.", tmpCardData.TableData.SptAddBOSetID1));
                    }
                    else
                    {
                        mListBattleOptionSet.Add(param);
                    }
                }

                if (tmpCardData.TableData.SptAddBOSetID2 > 0)
                {
                    param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(tmpCardData.TableData.SptAddBOSetID2);
                    if (param == null)
                    {
                        Debug.LogError(string.Format("{0}번 배틀옵션셋 데이터가 없습니다.", tmpCardData.TableData.SptAddBOSetID2));
                    }
                    else
                    {
                        mListBattleOptionSet.Add(param);
                    }
                }

                // 서브 스킬은 스킬 레벨 기준으로 배틀 옵션 데이터 생성
                Parse(tmpCardData.SkillLv);

                if (i == (int)eCARDSLOT.SLOT_MAIN)
                {
                    // 메인 슬롯에 장착된 서포터로 BOSupporter.m_data 값 세팅
                    m_data = tmpCardData;
                    // 메인 스킬 배틀 옵션 데이터 생성을 위해 mListBattleOptionSet 초기화
                    mListBattleOptionSet.Clear();
                    if (tmpCardData.TableData.SptMainBOSetID1 > 0)
                    {
                        param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(tmpCardData.TableData.SptMainBOSetID1);
                        if (param == null)
                        {
                            Debug.LogError(string.Format("{0}번 배틀옵션셋 데이터가 없습니다.", tmpCardData.TableData.SptMainBOSetID1));
                        }
                        else
                        {
                            mListBattleOptionSet.Add(param);
                        }
                    }
                    if (tmpCardData.TableData.SptMainBOSetID2 > 0)
                    {
                        param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(tmpCardData.TableData.SptMainBOSetID2);
                        if (param == null)
                        {
                            Debug.LogError(string.Format("{0}번 배틀옵션셋 데이터가 없습니다.", tmpCardData.TableData.SptMainBOSetID2));
                        }
                        else
                        {
                            mListBattleOptionSet.Add(param);
                        }
                    }

                    // 메인 스킬은 각성 단계 기준으로 배틀 옵션 데이터 생성 (각성 단계는 0부터 시작이기 떄문에 +1를 더해서 level값 넘겨줌)
                    Parse(tmpCardData.Wake + 1);
                }
            }
        }
    }

    public BOSupporter(List<CardData> listCardData, Unit owner) : base(owner)
    {
        mOwner = owner;
        mToExecuteType = eToExecuteType.Supporter;

        for (int i = 0; i < listCardData.Count; i++)
        {
            CardData tmpCardData = listCardData[i];
            if (tmpCardData == null)
            {
                Debug.LogError("카드 데이터가 없습니다.");
                return;
            }

            mListBattleOptionSet.Clear();
            GameClientTable.BattleOptionSet.Param param = null;
            if (tmpCardData.TableData.SptAddBOSetID1 > 0)
            {
                param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(tmpCardData.TableData.SptAddBOSetID1);
                if (param == null)
                {
                    Debug.LogError(string.Format("{0}번 배틀옵션셋 데이터가 없습니다.", tmpCardData.TableData.SptAddBOSetID1));
                    return;
                }
                mListBattleOptionSet.Add(param);
            }
            if (tmpCardData.TableData.SptAddBOSetID2 > 0)
            {
                param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(tmpCardData.TableData.SptAddBOSetID2);
                if (param == null)
                {
                    Debug.LogError(string.Format("{0}번 배틀옵션셋 데이터가 없습니다.", tmpCardData.TableData.SptAddBOSetID2));
                    return;
                }
                mListBattleOptionSet.Add(param);
            }

            // 서브 스킬은 스킬 레벨 기준으로 배틀 옵션 데이터 생성
            Parse(tmpCardData.SkillLv);

            if (i == (int)eCARDSLOT.SLOT_MAIN)
            {
                // 메인 슬롯에 장착된 서포터로 BOSupporter.m_data 값 세팅
                m_data = tmpCardData;
                // 메인 스킬 배틀 옵션 데이터 생성을 위해 mListBattleOptionSet 초기화
                mListBattleOptionSet.Clear();
                if (tmpCardData.TableData.SptMainBOSetID1 > 0)
                {
                    param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(tmpCardData.TableData.SptMainBOSetID1);
                    if (param == null)
                    {
                        Debug.LogError(string.Format("{0}번 배틀옵션셋 데이터가 없습니다.", tmpCardData.TableData.SptMainBOSetID1));
                        return;
                    }
                    mListBattleOptionSet.Add(param);
                }
                if (tmpCardData.TableData.SptMainBOSetID2 > 0)
                {
                    param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(tmpCardData.TableData.SptMainBOSetID2);
                    if (param == null)
                    {
                        Debug.LogError(string.Format("{0}번 배틀옵션셋 데이터가 없습니다.", tmpCardData.TableData.SptMainBOSetID2));
                        return;
                    }
                    mListBattleOptionSet.Add(param);
                }

                // 메인 스킬은 각성 단계 기준으로 배틀 옵션 데이터 생성 (각성 단계는 0부터 시작이기 떄문에 +1를 더해서 level값 넘겨줌)
                Parse(tmpCardData.Wake + 1);
            }
        }
    }

    protected override void Parse(int level, int actionTableId = -1)
    {
        Player player = mOwner as Player;

        for (int i = 0; i < mListBattleOptionSet.Count; i++)
        {
            // 액션 추가
            if (!string.IsNullOrEmpty(mListBattleOptionSet[i].BOAction))
            {
                System.Type type = System.Type.GetType("Action" + mListBattleOptionSet[i].BOAction);
                ActionBase action = (ActionBase)mOwner.gameObject.AddComponent(type);
                if (action != null)
                {
					if ( player && player.UseGuardian ) {
						player.AddAfterActionType( type );
					}

                    GameClientTable.BattleOptionSet.Param paramBOSet = GameInfo.Instance.GameClientTable.FindBattleOptionSet(mListBattleOptionSet[i].ID);
                    mOwner.actionSystem2.AddAction(action, 0, null, paramBOSet);
                }

                mActionCommand = Utility.GetActionCommandByString(mListBattleOptionSet[i].BOAction);
            }

            GameClientTable.BattleOption.Param param = GameInfo.Instance.GameClientTable.FindBattleOption(mListBattleOptionSet[i].BattleOptionID);
            if (param == null)
                continue;

            sBattleOptionData data = CreateBattleOptionData(mListBattleOptionSet[i], param, level, actionTableId);
            if (data != null)
            {
                if (param.BOAddBOSetID > 0)
                {
					GameClientTable.BattleOptionSet.Param paramAddBOSet = GameInfo.Instance.GameClientTable.FindBattleOptionSet(param.BOAddBOSetID);
                    param = GameInfo.Instance.GameClientTable.FindBattleOption(paramAddBOSet.BattleOptionID);

					// 액션 추가
					if (!string.IsNullOrEmpty(paramAddBOSet.BOAction))
					{
						System.Type type = System.Type.GetType("Action" + paramAddBOSet.BOAction);
						ActionBase action = (ActionBase)mOwner.gameObject.AddComponent(type);
						if (action != null)
						{
							if ( player && player.UseGuardian ) {
								player.AddAfterActionType( type );
							}

							GameClientTable.BattleOptionSet.Param paramBOSet = GameInfo.Instance.GameClientTable.FindBattleOptionSet(paramAddBOSet.ID);
							mOwner.actionSystem2.AddAction(action, 0, null, paramBOSet);
						}

						mActionCommand = Utility.GetActionCommandByString(paramAddBOSet.BOAction);
					}

					sBattleOptionData dataOnEndCall = CreateBattleOptionData(paramAddBOSet, param, level, actionTableId);
                    data.dataOnEndCall = dataOnEndCall;
                }

                ListBattleOptionData.Add(data);
            }
        }
    }

    public override bool HasActiveSkill()
    {
        if (m_data != null && m_data.TableData.SptBOWorkType == (int)eSkillType.Active)
            return true;

        return false;
    }
}
