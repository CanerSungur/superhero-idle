using System;

namespace ZestGames
{
    public static class EventManager { }

    public static class GameEvents 
    {
        public static Action OnGameStart, OnLevelSuccess, OnLevelFail, OnChangePhase;
        public static Action<Enums.GameEnd> OnGameEnd, OnChangeScene;
    }

    public static class PlayerEvents 
    { 
        
    }

    public static class UiEvents
    {
        public static Action<int> OnUpdateLevelText, OnUpdateCollectableText;
        public static Action<string, FeedBackUi.Colors> OnGiveFeedBack;
    }

    public static class CollectableEvents
    {
        public static Action<int> OnCollect;
    }
    
    public static class InputEvents
    {
        public static Action OnTapHappened, OnTouchStarted, OnTouchStopped;
    }
}
