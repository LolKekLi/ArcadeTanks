using UnityEngine.UI;

namespace Project.UI
{
    public class EmptyGraphics : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            return;
        }
    }
}