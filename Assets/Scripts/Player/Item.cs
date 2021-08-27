using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Item : MonoBehaviour
    {

        [Header("Hand settngs")]
        [SerializeField] protected WristData _wristData = WristData.zero;
        [SerializeField] protected Vector3 _position = Vector3.zero;
        [SerializeField] protected Quaternion _rotation = Quaternion.identity;

        public WristData WristData
        {
            get
            {
                Vector3 thumb = transform.position + transform.rotation * Vector3.Scale(_wristData.thumb, transform.lossyScale);
                Vector3 indexFinger = transform.position + transform.rotation * Vector3.Scale(_wristData.indexFinger, transform.lossyScale);
                Vector3 middleFinger = transform.position + transform.rotation * Vector3.Scale(_wristData.middleFinger, transform.lossyScale);
                Vector3 ringFinger = transform.position + transform.rotation * Vector3.Scale(_wristData.ringFinger, transform.lossyScale);
                Vector3 littleFinger = transform.position + transform.rotation * Vector3.Scale(_wristData.littleFinger, transform.lossyScale);
                return new WristData(thumb, indexFinger, middleFinger, ringFinger, littleFinger);
            }
        }

        public Vector3 LocalPositionInWrist => _position;
        public Quaternion LocalRotationInWrist => _rotation;

        private void OnDrawGizmosSelected()
        {
            float radius = Mathf.Clamp(0.01f * transform.lossyScale.magnitude, 0.005f, 0.015f);
            WristData handHand = WristData;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(handHand.thumb, radius);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(handHand.indexFinger, radius);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(handHand.middleFinger, radius);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(handHand.ringFinger, radius);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(handHand.littleFinger, radius);
        }
    }
}