using System;
using System.Collections;
using UnityEngine;
using MLAPI.SceneManagement;

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

        public event Action OnSceneLoadingStart;
        public event Action OnSceneLoadingComplete;

        public void LoadGame(Action OnComplete = null)
        {
            StartCoroutine(Load(_gameSceneName, OnComplete));
        }

        public void LoadLobby(Action OnComplete = null)
        {
            StartCoroutine(Load(_lobbySceneName, OnComplete));
        }

        private IEnumerator Load(string sceneName, Action OnComplete = null)
        {
            OnSceneLoadingStart?.Invoke();

            _progress = NetworkSceneManager.SwitchScene(_loadingSceneName);

            while(_progress.IsAllClientsDoneLoading == false)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            _progress = NetworkSceneManager.SwitchScene(sceneName);
            
            while (_progress.IsAllClientsDoneLoading == false)
            {
                yield return null;
            }

            _progress = null;

            OnSceneLoadingComplete?.Invoke();

            OnComplete?.Invoke();
        }
    }

    public enum SceneType { LOBBY, LOADING, GAME }
}