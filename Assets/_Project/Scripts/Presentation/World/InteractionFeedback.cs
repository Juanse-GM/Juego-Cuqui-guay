using UnityEngine;

namespace DatingSim.Presentation.World
{
    /// <summary>
    /// Feedback visual de hover: ilumina el hotspot con un color
    /// usando MaterialPropertyBlock (sin instanciar materiales).
    /// Ajusta el color a la propiedad del shader usado por URP Lit.
    /// </summary>
    public class InteractionFeedback : MonoBehaviour
    {
        [SerializeField] private Color hoverColor = new Color(1f, 0.92f, 0.25f, 1f);

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        private readonly MaterialPropertyBlock block = new MaterialPropertyBlock();
        private Hotspot current;

        public void SetHover(Hotspot hotspot)
        {
            if (current == hotspot) return;

            Clear();
            current = hotspot;
            if (current == null) return;

            var renderers = current.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                var renderer = renderers[i];
                if (renderer == null) continue;

                renderer.GetPropertyBlock(block);
                if (renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty(BaseColorId))
                    block.SetColor(BaseColorId, hoverColor);
                else
                    block.SetColor(ColorId, hoverColor);
                renderer.SetPropertyBlock(block);
            }
        }

        public void Clear()
        {
            if (current == null) return;

            var renderers = current.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                var renderer = renderers[i];
                if (renderer == null) continue;
                block.Clear();
                renderer.SetPropertyBlock(block);
            }
            current = null;
        }

        private void OnDisable()
        {
            Clear();
        }
    }
}
