
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System;

public class FacilitySupport
{
    public static long[] TempTradeMaterialUIDs = new long[4];

    public static void CopyToTempTradeMaterialUIDs(long[] uids)
    {
        if(uids == null)
        {
            TempTradeMaterialUIDs = null;
            return;
        }

        TempTradeMaterialUIDs = null;
        TempTradeMaterialUIDs = new long[uids.Length];
        uids.CopyTo(TempTradeMaterialUIDs, 0);
    }

    public static int GetCurTradeMaterialGrade(eFacilityTradeType eType)
    {
        int result = 0;

        for (int i = 0; i < TempTradeMaterialUIDs.Length; i++)
        {
            if (TempTradeMaterialUIDs[i] > 0)
            {
                if (eType == eFacilityTradeType.CARD)
                {
                    var w = GameInfo.Instance.CardList.Find(x => x.CardUID == TempTradeMaterialUIDs[i]);
                    if (w != null)
                    {
                        result = w.TableData.Grade;
                        break;
                    }
                }
                else if (eType == eFacilityTradeType.WEAPON)
                {
                    var w = GameInfo.Instance.WeaponList.Find(x => x.WeaponUID == TempTradeMaterialUIDs[i]);
                    if (w != null)
                    {
                        result = w.TableData.Grade;
                        break;
                    }
                }
            }
        }

        return result;
    }

    public static void TardeToggleMaterial(long uid, FacilityData _facilitydata)
    {
        for (int i = 0; i < TempTradeMaterialUIDs.Length; i++)
        {
            if (TempTradeMaterialUIDs[i] == uid)
            {
                TempTradeMaterialUIDs[i] = 0;
                return;
            }
        }

        int itemMaxCount = _facilitydata.TableData.EffectValue + (_facilitydata.Level - 1);

        for (int i = 0; i < TempTradeMaterialUIDs.Length; i++)
        {
            if (i >= itemMaxCount)
                break;
            if (TempTradeMaterialUIDs[i] == 0)
            {
                TempTradeMaterialUIDs[i] = uid;
                return;
            }
        }
    }

    public static bool IsTradeSelectMaterial(long uid)
    {
        for (int i = 0; i < TempTradeMaterialUIDs.Length; i++)
        {
            if (TempTradeMaterialUIDs[i] == uid)
                return true;
        }

        return false;
    }
}
