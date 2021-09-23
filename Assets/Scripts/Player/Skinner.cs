using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Player
{
    public class Skinner : MonoBehaviour
    {
        [SerializeField] private bool _chooseOnStart = false;
        [SerializeField] private GameObject[] _skins = null;
        private int _skinIndex = -1;
        public int SkinIndex
        {
            get => _skinIndex;
            set
            {
                if (value < 0) { return; }
                _skinIndex = Mathf.Clamp(value, 0, _skins.Length - 1);
                SkinIndexChanged?.Invoke(_skinIndex);
            }
        }
        public event Action<int> SkinIndexChanged;

        private void Start()
        {
            if (_chooseOnStart)
            {
                SkinIndex = Random.Range(0, _skins.Length);
                for (int i = 0; i < _skins.Length; i++)
                {
                    if (i == SkinIndex) _skins[i].SetActive(true);
                    else Destroy(_skins[i]);
                }
            }
        }
    }
}