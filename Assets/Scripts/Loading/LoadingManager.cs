using MLAPI;
using MLAPI.SceneManagement;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Loading
{
    public class LoadingManager : MonoBehaviour
    {
        [SerializeField] private string _lobbySceneName = "Lobby";
        [SerializeField] private string _loadingSceneName = "Loading";
        [SerializeField] private string _gameSceneName = "Game";

        public static LoadingManager Singleton { get; private set; } = null;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        SceneSwitchProgress _progress = null;
        public int DoneClientsCount => _progress.DoneClients.Count;
        public int AllClientsCount => NetworkManager.Singleton.ConnectedClientsList.Count;

        public event UnityAction OnSceneLoadingStart;
        public event UnityAction OnSceneLoadingComplete;

        public void LoadGame(UnityAction OnComplete = null)
        {
            StartCoroutine(Load(_gameSceneName, OnComplete));
        }

        public void LoadLobby(UnityAction OnComplete = null)
        {
            StartCoroutine(Load(_lobbySceneName, OnComplete));
        }

        private IEnumerator Load(string sceneName, UnityAction OnComplete = null)
        {
            OnSceneLoadingStart?.Invoke();

            _progress = NetworkSceneManager.SwitchScene(_loadingSceneName);
            Task frameTask = Task.Delay(50);

            while(_progress.IsAllClientsDoneLoading == false)
            {
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            _progress = NetworkSceneManager.SwitchScene(sceneName);

            while (_progress.IsAllClientsDoneLoading == false)
            {
                yield return null;
            }

            _progress = null;

            OnComplete?.Invoke();

            OnSceneLoadingComplete?.Invoke();
        }
    }
}