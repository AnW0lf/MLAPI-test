using UnityEngine;

namespace Assets.Scripts.Player
{
    [CreateAssetMenu(fileName = "New Player Profile", menuName = "Custom/Player Profile")]
    public class PlayerProfile : ScriptableObject
    {
        [SerializeField] private string _nickname = string.Empty;
        [SerializeField] private int _iconIndex = 0;
        [SerializeField] private int _skinIndex = 0;

        public string Nickname => _nickname;
        public int IconIndex => _iconIndex;
        public int SkinIndex => _skinIndex;
    }
}