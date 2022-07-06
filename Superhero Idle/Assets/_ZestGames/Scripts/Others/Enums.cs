namespace ZestGames
{
    public class Enums
    {
        public enum GameState { WaitingToStart, Started, PlatrofmEnded, GameEnded }
        public enum GameEnd { None, Success, Fail }
        public enum PoolStamp { Something, PoliceMan, PoliceCar, Civillian, Criminal, Money, CollectMoney, SpendMoney, Medic, ConfettiExplosion }
        public enum AudioType { Testing_PlayerMove, Button_Click, UpgradeMenu, PhaseUnlockUnlock, PhoneBoothUnlock, MoneySpawn, Punch, StateChange }
        public enum PlayerState { Civillian, Hero }
        public enum CivillianType { WithBag, WithoutBag }
        public enum CriminalAttackType { Civillian, ATM }
        public enum CarrySide { Left, Right }
        public enum MoneyType { Spend, Collect }
    }
}
