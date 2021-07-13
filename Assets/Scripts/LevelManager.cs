using UnityEngine;
using MLAPI;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Singleton { get; private set; } = null;

    [Header("Population")]
    [SerializeField] private GameObject[] _civilianPrefabs = null;
    [SerializeField] private int _maxCount = 100;
    [SerializeField] private Rect _populationArea = Rect.zero;
    [SerializeField] private float _raycastHeight = 20f;

    private List<GameObject> _civilians = new List<GameObject>();

    private void Awake()
    {
        if (Singleton == null) Singleton = this;
        else if (Singleton != this) Destroy(Singleton);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandlerServerStarted;
    }

    private void HandlerServerStarted()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Populate();
        }
    }

    private void Populate()
    {
        while(_civilians.Count < _maxCount)
        {
            Vector3? randomPlace = GetRandomPointOnGround();
            if(randomPlace != null)
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
        if (Physics.Raycast(ray, out RaycastHit hit))
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
