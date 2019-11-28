using UnityEngine.UIElements;

namespace Thijs.Framework.UI
{
    public abstract class VisualComponent : VisualElement, IVisualComponent
    {
        public abstract string StyleSheetPath { get; }
        public abstract string TemplatePath { get; }
        public VisualElement Element => this;

        public VisualComponent()
        {
            VisualComponentFactory.InitializeVisualComponent(this);
        }
    }
}
