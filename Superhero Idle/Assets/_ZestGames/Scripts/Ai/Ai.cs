using UnityEngine;
using System;
using ZestCore.Utility;

namespace ZestGames
{
    public class Ai : MonoBehaviour
    {
        #region COMPONENTS

        private Animator animator;
        public Animator Animator => animator == null ? animator = GetComponent<Animator>() : animator;

        private Collider coll;
        public Collider Collider => coll == null ? coll = GetComponent<Collider>() : coll;

        private Rigidbody rb;
        public Rigidbody Rigidbody => rb == null ? rb = GetComponent<Rigidbody>() : rb;

        #endregion

        #region SCRIPT REFERENCES

        private AiAnimationController animationController;
        public AiAnimationController AnimationController => animationController == null ? animationController = GetComponent<AiAnimationController>() : animationController;

        private AiCollision collision;
        public AiCollision Collision => collision == null ? collision = GetComponent<AiCollision>() : collision;

        private IAiMovement movement;
        public IAiMovement Movement => movement == null ? movement = GetComponent<IAiMovement>() : movement;

        #endregion

        [Header("-- MOVEMENT SETUP --")]
        [SerializeField] private float maxMovementSpeed = 3f;
        [SerializeField] private float minMovementSpeed = 1f;
        [SerializeField] private bool useAcceleration = false;
        [SerializeField, Range(0.1f, 3f)] private float accelerationRate = 0.5f;
        private float _currentMovementSpeed;

        [Header("-- GROUNDED SETUP --")]
        [SerializeField, Tooltip("Select layers that you want this object to be grounded.")] private LayerMask groundLayerMask;
        [SerializeField, Tooltip("Height that this object will be considered grounded when above groundable layers.")] private float groundedHeightLimit = 0.05f;

        #region CONTROLS

        public bool IsDead { get; private set; }
        public Transform Target { get; private set; }

        public bool CanMove => GameManager.GameState == Enums.GameState.Started;
        public bool IsMoving => Movement.IsMoving;
        public bool IsGrounded => Physics.Raycast(Collider.bounds.center, Vector3.down, Collider.bounds.extents.y + groundedHeightLimit, groundLayerMask);
        public float CurrentMovementSpeed => _currentMovementSpeed;

        #endregion

        #region EVENTS

        public Action OnIdle, OnMove, OnDie;
        public Action<Transform> OnSetTarget;

        #endregion

        private void Init()
        {
            IsDead = false;
            Target = null;
            if (useAcceleration)
                _currentMovementSpeed = minMovementSpeed;
            else
                _currentMovementSpeed = maxMovementSpeed;

            CharacterTracker.AddAi(this);

            OnSetTarget += SetTarget;

            AnimationController.Init(this);
            Movement.Init(this);
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            CharacterTracker.RemoveAi(this);
            OnSetTarget -= SetTarget;
        }

        private void Update()
        {
            if (!IsMoving && IsGrounded && Rigidbody) Rigidbody.velocity = Vector3.zero;

            UpdateCurrentMovementSpeed();
        }

        private void UpdateCurrentMovementSpeed()
        {
            if (!useAcceleration) return;

            if (IsMoving)
                _currentMovementSpeed = Mathf.MoveTowards(_currentMovementSpeed, maxMovementSpeed, accelerationRate * Time.deltaTime);
            else
                _currentMovementSpeed = minMovementSpeed;
        }

        private void Die()
        {
            IsDead = true;
            OnDie?.Invoke();
            CharacterTracker.RemoveAi(this);
            Delayer.DoActionAfterDelay(this, 5f, () => gameObject.SetActive(false));
        }

        public void SetTarget(Transform transform)
        {
            if (!CanMove) return;

            Target = transform;
            OnMove?.Invoke();
        }
    }
}
