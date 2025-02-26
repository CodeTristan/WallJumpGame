#if UNITY_ANDROID
using UnityEngine;

namespace Unity.Services.LevelPlay
{
    class UnityLevelPlayInitListener : AndroidJavaProxy, IUnityLevelPlayInitListener
    {
        const string k_ILevelPlayInitListenerName = "com.ironsource.unity.androidbridge.IUnityLevelPlayInitListener";
        IUnityLevelPlayInitListener m_Listener;
        public UnityLevelPlayInitListener(IUnityLevelPlayInitListener listener) : base(k_ILevelPlayInitListenerName)
        {
            m_Listener = listener;
        }

        public void onInitSuccess(string configuration)
        {
            ThreadUtil.Post(state => m_Listener.onInitSuccess(configuration));
        }

        public void onInitFailed(string error)
        {
            ThreadUtil.Post(state => m_Listener.onInitFailed(error));
        }
    }
}
#endif
