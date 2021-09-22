using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Assets.Scripts.Player
{
    public class HandMotion : MonoBehaviour
    {
        [Header("Hand Settings")]
        [SerializeField] private Hand _hand = null;
        [SerializeField] private Transform _handAnchorPoint = null;
        [SerializeField] private Transform _handTargetPoint = null;
        [SerializeField] [Range(0.1f, 10f)] private float _handMotionSpeed = 2f;
        [Header("Item Settings")]
        public Item item;
        [SerializeField] private float _verticalOffset = -0.085f;

        private readonly Quaternion _handRotationOffset = Quaternion.Euler(0f, 90f, 90f);
        private readonly Quaternion _handRotationAimOffset = Quaternion.Euler(90f, -90f, 0f);
        private readonly float _handTargetDistance = 0.5f;

        private enum HandState { FREE = 0, BAG = 1, VISIBLE = 2 }
        private HandState State => (HandState)(Mathf.RoundToInt(Mathf.Clamp(_hand.State, 0f, 2f)));

        private bool _isHandVisible = false;
        public bool IsHandVisible
        {
            get => _isHandVisible;
            set
            {
                _isHandVisible = value;
                if (_isHandVisible) ShowItem();
                else HideItem();
            }
        }

        public vThirdPersonCamera tpCamera { get; set; } = null;
        public bool IsAim { get; set; } = false;

        private void Update()
        {
            if (IsHandVisible && ItemActive)
            {
                _hand.wrist.weight = Mathf.Clamp(_hand.wrist.weight + Time.deltaTime * 5f, 0f, 1f);
                _hand.SetFingerPositions(item.WristData);
            }
            else
            {
                _hand.wrist.weight = Mathf.Clamp(_hand.wrist.weight - Time.deltaTime * 5f, 0f, 1f);
            }

            if (IsHandVisible)
            {
                if (IsAim)
                {
                    if (tpCamera)
                    {
                        Vector3 hitPoint = tpCamera.RayCastCenter(transform);
                        Vector3 fromHandToPoint = hitPoint - _handAnchorPoint.position;
                        Vector3 direction = fromHandToPoint.normalized;
                        float distance = fromHandToPoint.magnitude;
                        Vector3 position = _handAnchorPoint.position + direction * _handTargetDistance;

                        _handTargetPoint.position = position;
                        _handTargetPoint.LookAt(hitPoint);
                        _handTargetPoint.rotation *= _handRotationAimOffset;

                        if (distance != 0f)
                        {
                            float angle = Mathf.Atan(_verticalOffset / distance) * Mathf.Rad2Deg;
                            _handTargetPoint.rotation *= Quaternion.Euler(0f, angle, 0f);
                        }
                    }
                }
                else
                {
                    _handTargetPoint.position = _handAnchorPoint.position + transform.forward * _handTargetDistance;
                    _handTargetPoint.localRotation = _handRotationOffset;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (IsAim)
            {
                if (tpCamera)
                {
                    Vector3 from = tpCamera.transform.position;
                    Vector3 to = tpCamera.RayCastCenter(transform);

                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(from, to);
                    Gizmos.DrawWireSphere(to, 0.2f);

                    from = _handTargetPoint.position;
                    to = from + _handTargetPoint.right * 10f;

                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(from, to);
                }
            }
        }

        private bool ItemActive
        {
            get => item != null && item.gameObject.activeSelf;
            set
            {
                if (item == null) { return; }
                item.gameObject.SetActive(value);
                if (ItemActive) _hand.LocateItem(item);
            }
        }

        private Coroutine _moveHand = null;
        private IEnumerator MoveHand(float state, float speed, Coroutine self = null)
        {
            float duration = Mathf.Abs(_hand.State - state) / speed;
            yield return Utils.CrossFading(_hand.State, state, duration, (state) => _hand.State = state, (a, b, c) => Mathf.Lerp(a, b, c));
            if (self != null) self = null;
        }

        private Coroutine _updateHandState = null;
        private IEnumerator UpdateHandState(HandState state, Coroutine self = null)
        {
            if (_moveHand != null) StopCoroutine(_moveHand);

            switch (state)
            {
                case HandState.FREE:
                    {
                        ItemActive = false;
                        yield return _moveHand = StartCoroutine(
                            MoveHand((float)HandState.FREE, _handMotionSpeed, _moveHand));
                    }
                    break;
                case HandState.BAG:
                    {
                        ItemActive = false;
                        yield return _moveHand = StartCoroutine(
                            MoveHand((float)HandState.BAG, _handMotionSpeed, _moveHand));
                        ItemActive = true;
                    }
                    break;
                case HandState.VISIBLE:
                    {
                        ItemActive = true;
                        yield return _moveHand = StartCoroutine(
                            MoveHand((float)HandState.VISIBLE, _handMotionSpeed, _moveHand));
                    }
                    break;
            }

            if (self != null) self = null;
        }

        private Coroutine _setHandState = null;
        private IEnumerator SetHandState(HandState state, Coroutine self = null)
        {
            if (_updateHandState != null) StopCoroutine(_updateHandState);

            HandState currentState = State;
            yield return _updateHandState = StartCoroutine(UpdateHandState(currentState, _updateHandState));
            while (State != state)
            {
                int direction = Mathf.Clamp(state - currentState, -1, 1);
                HandState newState = (HandState)Mathf.Clamp((int)currentState + direction, 0, 2);
                yield return _updateHandState = StartCoroutine(UpdateHandState(newState, _updateHandState));
                currentState = newState;
            }

            if (self != null) self = null;
        }

        public void ShowItem()
        {
            if (_setHandState != null) StopCoroutine(_setHandState);
            _setHandState = StartCoroutine(SetHandState(HandState.VISIBLE));
        }

        public void HideItem()
        {
            if (_setHandState != null) StopCoroutine(_setHandState);
            _setHandState = StartCoroutine(SetHandState(HandState.FREE));
        }

        public void SwitchHand()
        {
            IsHandVisible = !IsHandVisible;
        }
    }

    [Serializable]
    public struct WristData
    {
        public Vector3 thumb;
        public Vector3 indexFinger;
        public Vector3 middleFinger;
        public Vector3 ringFinger;
        public Vector3 littleFinger;

        public WristData(Vector3 thumb, Vector3 indexFinger, Vector3 middleFinger, Vector3 ringFinger, Vector3 littleFinger)
        {
            this.thumb = thumb;
            this.indexFinger = indexFinger;
            this.middleFinger = middleFinger;
            this.ringFinger = ringFinger;
            this.littleFinger = littleFinger;
        }

        public static WristData zero => new WristData(Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero);
    }

    [Serializable]
    public class Hand
    {
        public Rig wrist = null;
        public Transform thumb = null;
        public Transform indexFinger = null;
        public Transform middleFinger = null;
        public Transform ringFinger = null;
        public Transform littleFinger = null;
        public Transform container = null;
        public TwoBoneIKConstraint[] handPoints = null;
        private float _state = 0f;

        public Hand(Rig wrist, Transform thumb, Transform indexFinger, Transform middleFinger, Transform ringFinger, Transform littleFinger, Transform container, TwoBoneIKConstraint[] handPoints)
        {
            this.wrist = wrist;
            this.thumb = thumb;
            this.indexFinger = indexFinger;
            this.middleFinger = middleFinger;
            this.ringFinger = ringFinger;
            this.littleFinger = littleFinger;
            this.container = container;
            this.handPoints = handPoints;
            _state = 0f;
        }

        public void SetFingerPositions(WristData handData)
        {
            thumb.position = handData.thumb;
            indexFinger.position = handData.indexFinger;
            middleFinger.position = handData.middleFinger;
            ringFinger.position = handData.ringFinger;
            littleFinger.position = handData.littleFinger;
        }

        public void LocateItem(Item item)
        {
            item.transform.SetParent(container);
            item.transform.localPosition = item.LocalPositionInWrist;
            item.transform.localRotation = item.LocalRotationInWrist;
            SetFingerPositions(item.WristData);
        }

        public float State
        {
            get => _state;
            set
            {
                _state = Mathf.Clamp(value, 0f, handPoints.Length);

                for (int i = 0; i < handPoints.Length; i++)
                {
                    float weight = Mathf.Clamp01(1 - Mathf.Abs(i + 1f - _state));
                    handPoints[i].weight = weight;
                }
            }
        }
    }
}