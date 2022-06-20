using UnityEngine;
using DG.Tweening;

namespace SuperheroIdle
{
    public class FightSmokeLimp : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private float distanceFromSmoke = 1.5f;

        private void OnEnable()
        {
            transform.DOLocalMove(transform.localPosition + (transform.forward * distanceFromSmoke), .65f).OnComplete(() => {
                transform.DOLocalMove(transform.localPosition - (transform.forward * distanceFromSmoke), .35f).SetEase(Ease.InSine);
                Destroy(gameObject);
                //transform.DOMove(transform.position + (transform.forward * 3), 1f);
                //transform.DOJump(transform.position - (transform.forward * 3), 0.7f, 1, 1f);
            });

            //Delayer.DoActionAfterDelay(this, 0.75f, () => Destroy(gameObject));
        }

        private void OnDisable()
        {
            transform.DOKill();
        }
    }
}
