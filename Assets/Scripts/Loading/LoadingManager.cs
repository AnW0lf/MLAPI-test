using MLAPI.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Loading
{
    public class LoadingManager : MonoBehaviour
    {
        [SerializeField] private string _lobbySceneName = "Lobby";
        [SerializeField] private string _loadingSceneName = "Loading";
        [SerializeField] private string _gameSceneName = "Game";

        private Dictionary<SceneType, string> _scenes = new Dictionary<SceneType, string>();

        public static LoadingManager Singleton { get; private set; } = null;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            _scenes.Add(SceneType.LOBBY, _lobbySceneName);
            _scenes.Add(SceneType.LOADING, _loadingSceneName);
            _scenes.Add(SceneType.GAME, _gameSceneName);
        }

        SceneSwitchProgress _progress = null;

        public event Action OnSceneLoadingStart;
        public event Action OnSceneLoadingComplete;

        public void LoadGame(UnityAction OnComplete = null)
        {
            StartCoroutine(Load(_scenes[SceneType.GAME], OnComplete));
        }

        public void LoadLobby(UnityAction OnComplete = null)
        {
            StartCoroutine(Load(_scenes[SceneType.LOBBY], OnComplete));
        }

        private IEnumerator Load(string sceneName, UnityAction OnComplete = null)
        {
            OnSceneLoadingStart?.Invoke();

            _progress = NetworkSceneManager.SwitchScene(_scenes[SceneType.LOADING]);

            while(_progress.IsAllClientsDoneLoading == false)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);

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