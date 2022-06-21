using SuperheroIdle;
using System;
using UnityEngine;

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
        public static Action OnIdle, OnMove, OnChangeToCivillian, OnChangeToHero, OnEnterPhoneBooth, OnExitPhoneBooth, OnExitPhoneBoothSuccessfully;
        public static Action<Criminal> OnStartFighting, OnStopFighting;
        public static Action<Vector3> OnGoToPhoneBooth;
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

    public static class PeopleEvents
    {
        public static Action OnCriminalDecreased, OnCivillianDecreased;
    }
}
