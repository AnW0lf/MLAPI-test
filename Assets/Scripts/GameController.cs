using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using MLAPI;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        public static GameController Singleton { get; private set; } = null;

        [Header("Population")]
        [SerializeField] private GameObject[] _civilianPrefabs = null;
        [SerializeField] private int _maxCount = 100;
        [SerializeField] private Rect _populationArea = Rect.zero;
        [SerializeField] private float _raycastHeight = 20f;
        [Header("Interest points")]
        [SerializeField] private Transform[] _interestPoints = null;

        private List<GameObject> _civilians = new List<GameObject>();

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(Singleton);
        }

        private void Start()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
            {
                Populate();
            }
        }

        private void Populate()
        {
            while (_civilians.Count < _maxCount)
            {
                Vector3? randomPlace = GetRandomPointOnGround();
                if (randomPlace != null)
                {
                    GameObject prefab = _civilianPrefabs[Random.Range(0, _civilianPrefabs.Length)];
                    GameObject civilian = Instantiate(prefab, (Vector3)randomPlace, Quaternion.identity);
                    _civilians.Add(civilian);
                }
            }
        }

        private Vector3? GetRandomPointOnGround()
        {
            Ray ray = new Ray(RandomPoint, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, _raycastHeight))
            {
                if (hit.transform.CompareTag("Ground"))
                {
                    return hit.point;
                }
            }
            return null;
        }

        private Vector3 RandomPoint
        {
            get
            {
                float x = transform.position.x + Random.Range(-_populationArea.width / 2f, _populationArea.width / 2f);
                float y = transform.position.y + _raycastHeight;
                float z = transform.position.z + Random.Range(-_populationArea.height / 2f, _populationArea.height / 2f);
                return new Vector3(x, y, z);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector3 center = new Vector3(_populationArea.x, _raycastHeight / 2f, _populationArea.y) + transform.position;
            Vector3 size = new Vector3(_populationArea.width, _raycastHeight, _populationArea.height);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
