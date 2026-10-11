using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>Purple pill with a coin and a coin count. Its SortingGroup sets where it draws.</summary>
    public sealed class CoinBadge : MonoBehaviour
    {
        [SerializeField] TextMesh amount;

        public void Set(int coins) => amount.text = coins.ToString();
        public void Set(string text) => amount.text = text;
    }
}
