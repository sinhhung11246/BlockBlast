using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>
    /// Scales its sprite to cover the main camera's view and refits whenever the view size changes,
    /// e.g. the Editor Game view turning portrait after the first frame, or a phone rotating.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class CoverCamera : MonoBehaviour
    {
        [Tooltip("Extra scale, e.g. so camera shake never shows the edge.")]
        [SerializeField] float margin = 1f;

        SpriteRenderer sr;
        Camera cam;
        Vector2 fitted;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            cam = Camera.main;
        }

        void LateUpdate()
        {
            if (View() != fitted) Fit();
        }

        void Fit()
        {
            fitted = View();
            Vector2 size = sr.sprite.bounds.size;
            float scale = Mathf.Max(fitted.x / size.x, fitted.y / size.y) * margin;
            transform.localScale = new Vector3(scale, scale, 1f);
        }

        Vector2 View() => new Vector2(cam.orthographicSize * 2f * cam.aspect, cam.orthographicSize * 2f);
    }
}
