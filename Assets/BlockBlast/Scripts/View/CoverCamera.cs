using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>
    /// Scales its sprite to cover the camera view and refits whenever the view size changes,
    /// e.g. the Editor Game view turning portrait after the first frame, or a phone rotating.
    /// </summary>
    public sealed class CoverCamera : MonoBehaviour
    {
        SpriteRenderer sr;
        Camera cam;
        float margin;
        Vector2 fitted;

        public void Init(SpriteRenderer renderer, Camera camera, float scaleMargin)
        {
            sr = renderer;
            cam = camera;
            margin = scaleMargin;
            Fit();
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
