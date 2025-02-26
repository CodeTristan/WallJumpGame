#if UNITY_ANDROID
namespace Unity.Services.LevelPlay
{
    interface IUnityLevelPlayInitListener
    {
        void onInitSuccess(string configuration);
        void onInitFailed(string error);
    }
}
#endif
