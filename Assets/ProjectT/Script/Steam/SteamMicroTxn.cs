#if !DISABLESTEAMWORKS

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine;
using UnityEngine.Networking;

namespace Steamworks
{
    public class StoreReceipt
    {
        public string orderid;
        public byte Authorized;
    }

    public class SteamMicroTxn
    {
        protected Callback<MicroTxnAuthorizationResponse_t> mMicroTxnAuthorizationResponse;

        public SteamMicroTxn()
        {
            mMicroTxnAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(OnMicroTxnAuthorizationResponse);
        }

        private void OnMicroTxnAuthorizationResponse(MicroTxnAuthorizationResponse_t pCallback)
        {
            StoreReceipt receipt = new StoreReceipt();
            receipt.orderid = pCallback.m_ulOrderID.ToString();
            receipt.Authorized = pCallback.m_bAuthorized;

            string confirmStr = JsonUtility.ToJson(receipt);
            Debug.Log(confirmStr);

            int storeID = 0;

            if (!int.TryParse(IAPManager.Instance.BuyStoreID, out storeID))
            {
                IAPManager.Instance.IsBuying = false;
                return;
            }

            string log = string.Format("AppID : {0}, OrderID : {1}, AuAuthorized : {2}, storeID : {3}",
               pCallback.m_unAppID, pCallback.m_ulOrderID, pCallback.m_bAuthorized, storeID);
            Debug.Log(log);

            if (receipt.Authorized == 1)
            {
                GameInfo.Instance.Send_ReqStorePurchaseInApp(confirmStr, storeID, null);
            }
            else
            {
                IAPManager.Instance.IsBuying = false;
                WaitPopup.Hide();
            }
        }
    }

    public class SteamAuth
    {
        private bool _useTicket = false;
        public bool UseTicket { get { return _useTicket; } set { _useTicket = value; } }
        protected HAuthTicket _authSessionTicket;
        protected byte[] _ticket = new byte[1024];
        protected uint _pcbTicket;
        private string _sendTicket;
        public string SendTicket { get { return _sendTicket; } }
        protected EBeginAuthSessionResult _eBeginAuthSessionResult;

        private Coroutine _sessionCheckCor = null;

        protected Callback<GetAuthSessionTicketResponse_t> mGetAuthSessionTicketResponse;

        public SteamAuth()
        {
            mGetAuthSessionTicketResponse = Callback<GetAuthSessionTicketResponse_t>.Create(OnGetAuthSessionTicketResponse);
        }

        public void OnGetAuthSessionTicketResponse(GetAuthSessionTicketResponse_t callback)
        {
            //Debug.Log("[" + GetAuthSessionTicketResponse_t.k_iCallback + " - GetAuthSessionTicketResponse] - " + callback.m_hAuthTicket + " -- " + callback.m_eResult);

            if (callback.m_eResult == EResult.k_EResultOK)
            {
                if (_authSessionTicket != HAuthTicket.Invalid && _pcbTicket != (int)eCOUNT.NONE)
                {
                    System.Array.Resize(ref _ticket, (int)_pcbTicket);
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    for (int i = 0; i < _pcbTicket; i++)
                        sb.AppendFormat("{0:x2}", _ticket[i]);

                    _sendTicket = sb.ToString();

                    _useTicket = true;
                }
            }
            else
            {
                _useTicket = false;

                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3294, callback.m_eResult.ToString()),
                    () =>
                    {
                        Application.Quit();
                    },
                    true);

            }
        }

        public void UserAuthSession()
        {
            GetTicket();

            _useTicket = false;
            _eBeginAuthSessionResult = SteamUser.BeginAuthSession(_ticket, (int)_pcbTicket, SteamUser.GetSteamID());
        }

        public void GetTicket()
        {
            _useTicket = false;
            _authSessionTicket = SteamUser.GetAuthSessionTicket(_ticket, _ticket.Length, out _pcbTicket);

        }

        IEnumerator SessionCheck()
        {
            while (!_useTicket)
                yield return null;


        }
    }

    public class SteamValidate
    {
        protected Callback<ValidateAuthTicketResponse_t> mValidateAuthTicketResponse;

        public SteamValidate()
        {
            mValidateAuthTicketResponse = Callback<ValidateAuthTicketResponse_t>.Create(OnValidateAuthTicketResponse);
        }

        public static void OnValidateAuthTicketResponse(ValidateAuthTicketResponse_t callback)
        {
            Debug.Log("[" + ValidateAuthTicketResponse_t.k_iCallback + " - ValidateAuthTicketResponse] - " + callback.m_eAuthSessionResponse);

            SteamUser.EndAuthSession(SteamUser.GetSteamID());

            if (callback.m_eAuthSessionResponse != EAuthSessionResponse.k_EAuthSessionResponseOK)
            {
                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3294, callback.m_eAuthSessionResponse.ToString()),
                    () => {
                        Application.Quit();
                    },
                    true);
                return;
            }
        }
    }
}

#endif
