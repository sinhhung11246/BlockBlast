using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>Play screen texts (best, rolling score, "no more space") plus the crown and settings gear.</summary>
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] TextMesh best, score, outOfSpace;
        [SerializeField] SpriteRenderer gear;

        float rollTime;
        int target;
        float shown; // rolls toward target

        public void Init(float scoreRollTime) => rollTime = scoreRollTime;

        public int Best { set => best.text = value.ToString(); }
        public int Score { set => target = value; }
        public bool OutOfSpace { set => outOfSpace.gameObject.SetActive(value); }

        public bool HitsSettings(Vector2 world) => SpriteFactory.Hits(gear, world);

        void Update()
        {
            if (target < shown || Mathf.Abs(target - shown) < 0.5f) shown = target; // restart or arrived
            else shown += (target - shown) * (1f - Mathf.Exp(-Time.deltaTime * 4f / rollTime));
            score.text = Mathf.RoundToInt(shown).ToString();
        }
    }
}
