using SuperheroIdle;
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
        public static Action OnIdle, OnMove, OnChangeToCivillian, OnChangeToHero;
        public static Action<Criminal> OnStartFighting, OnStopFighting;
        public static Action<PhoneBooth> OnGoToPhoneBooth, OnEnterPhoneBooth, OnExitPhoneBooth, OnExitPhoneBoothSuccessfully;
        public static Action OnSetCurrentMovementSpeed, OnSetCurrentChangeTime, OnSetCurrentFightPower, OnSetCurrentIncomeIncrease, OnSetCurrentCostDecrease;
    }

    public static class UiEvents
    {
        public static Action<int> OnUpdateLevelText, OnUpdateCollectableText;
        public static Action<string, FeedBackUi.Colors> OnGiveFeedBack;
        public static Action OnUpdateMovementSpeedText, OnUpdateChangeSpeedText, OnUpdateFightSpeedText, OnUpdateIncomeIncreaseText, OnUpdateCostDecreaseText;
    }

    public static class CollectableEvents
    {
        public static Action<int> OnCollect, OnConsume;
    }
    
    public static class InputEvents
    {
        public static Action OnTapHappened, OnTouchStarted, OnTouchStopped;
    }

    public static class PeopleEvents
    {
        public static Action OnCriminalDecreased, OnCivillianDecreased;
    }

    public static class CrimeEvents
    {
        public static Action<Phase> OnCrimeStarted, OnCrimeEnded;
    }

    public static class UpgradeEvents
    {
        public static Action OnUpgradeMovementSpeed, OnUpgradeChangeTime, OnUpgradeFightTime, OnUpgradeIncome, OnUpgradeCostDecrease;
        public static Action OnOpenUpgradeCanvas, OnCloseUpgradeCanvas;
    }
    
    public static class PhaseEvents
    {
        public static Action<PhaseUnlocker, int> OnConsumeMoney;
        public static Action<PhaseUnlocker, Phase> OnUnlockPhase;
    }

    public static class AudioEvents
    {
        public static Action OnPlayCollectMoney, OnPlaySpendMoney, OnStartPunch, OnStopPunch;
    }
}
