using UnityEngine;
using DG.Tweening;
using ZestGames;

namespace SuperheroIdle
{
    public class Money2D : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private Enums.MoneyType type;

        private Transform moneyPointTransform;

        private void OnDisable()
        {
            transform.DOKill();
        }

        //public void Spend(PhaseUnlocker phaseUnlocker)
        //{
        //    transform.DOJump(phaseUnlocker.transform.position, 4, 1, 1.5f).OnComplete(() =>
        //    {
        //        phaseUnlocker.ConsumeMoney(DataManager.MoneyValue);
        //        gameObject.SetActive(false);
        //    });
        //}
        //public void Collect(Vector3 position)
        //{
        //    moneyPointTransform = Camera.main.transform.GetChild(0);
        //    //Vector3 _collectableHUDWorldPos = Camera.main.ScreenToWorldPoint(Hud.CollectableHUDTransform.position + new Vector3(-0.5f, 0f, 5f));

        //    transform.position = CharacterManager.PlayerTransform.position;
        //    transform.DOMove(moneyPointTransform.position, 1f).OnComplete(() =>
        //    {
        //        CollectableEvents.OnCollect?.Invoke(DataManager.MoneyValue);
        //        gameObject.SetActive(false);
        //    });
        //    //transform.DOJump(CharacterManager.PlayerTransform.position, 4, 1, 1.5f).OnComplete(() => {
        //    //    CollectableEvents.OnCollect?.Invoke(DataManager.MoneyValue);
        //    //    gameObject.SetActive(false);
        //    //});
        //}
    }
}
