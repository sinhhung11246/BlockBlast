using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>Game Over popup: background, title, panel with the final score and coins earned, home and replay buttons.</summary>
    public sealed class GameOverView : MonoBehaviour
    {
        [SerializeField] TextMesh label, score;
        [SerializeField] CoinBadge earned;
        [SerializeField] SpriteRenderer replay, home;

        public void Show(int finalScore, bool newRecord, int coinsEarned)
        {
            label.text = newRecord ? "NEW BEST SCORE" : "SCORE";
            score.text = finalScore.ToString();
            earned.Set("+" + coinsEarned);
            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);

        public bool HitsReplay(Vector2 world) => SpriteFactory.Hits(replay, world);
        public bool HitsHome(Vector2 world) => SpriteFactory.Hits(home, world);
    }
}
