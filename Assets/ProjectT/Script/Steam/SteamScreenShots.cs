#if !DISABLESTEAMWORKS

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine;
using UnityEngine.Networking;

namespace Steamworks
{
    public class SteamScreenShots
    {
        protected Callback<ScreenshotRequested_t> mScreenshotRequested;
        protected Callback<ScreenshotReady_t> mScreenshotReady;

        public SteamScreenShots()
        {
            //SteamScreenshots.HookScreenshots(true); //F12 스크린샷 막기
            mScreenshotRequested = Callback<ScreenshotRequested_t>.Create(OnScreenshotRequested);
            mScreenshotReady = Callback<ScreenshotReady_t>.Create(OnScreenshotReady);
        }

        public void OnScreenshotRequested(ScreenshotRequested_t pCallback)
        {
            Debug.Log(string.Format("OnScreenshotRequested {0}", pCallback.ToString()));
        }

        public void OnScreenshotReady(ScreenshotReady_t pCallback)
        {

        }
    }
}

#endif
