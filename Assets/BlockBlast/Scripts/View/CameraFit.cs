using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>
    /// Sizes the orthographic camera so the whole Figma screen is always visible; wider or taller
    /// devices see extra background. Refits when the screen is resized or rotated.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class CameraFit : MonoBehaviour
    {
        [Tooltip("Figma screen (1440x2960 px, 167 px per cell) in world units, where 1 unit = 1 board cell.")]
        [SerializeField] Vector2 designSize = new Vector2(8.62f, 17.72f);

        Camera cam;
        float fittedAspect;

        void Awake()
        {
            cam = GetComponent<Camera>();
            Fit();
        }

        void Update()
        {
            if (cam.aspect != fittedAspect) Fit();
        }

        void Fit()
        {
            fittedAspect = cam.aspect;
            cam.orthographicSize = Mathf.Max(designSize.y, designSize.x / cam.aspect) * 0.5f;
        }
    }
}
