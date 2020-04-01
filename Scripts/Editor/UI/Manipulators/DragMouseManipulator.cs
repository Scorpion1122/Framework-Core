using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TKO.Framework.UI
{
    public class DragMouseManipulator : MouseManipulator
    {
        private VisualElement dragTarget;
        private bool isActive;
        
        public DragMouseManipulator(VisualElement dragTarget, MouseButton button)
        {
            this.dragTarget = dragTarget;
            activators.Add(new ManipulatorActivationFilter { button = button });
            isActive = false;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        private void OnMouseDown(MouseDownEvent mouseEvent)
        {
            if (isActive)
            {
                mouseEvent.StopImmediatePropagation();
                return;
            }

            if (CanStartManipulation(mouseEvent))
            {
                isActive = true;
                target.CaptureMouse();
                mouseEvent.StopPropagation();
            }
        }

        private void OnMouseMove(MouseMoveEvent mouseEvent)
        {
            if (!isActive || !target.HasMouseCapture())
                return;

            Vector2 scale = dragTarget.worldTransform.lossyScale;
            Vector2 delta = mouseEvent.mouseDelta;
            
            dragTarget.style.top = dragTarget.layout.y + delta.y / scale.y;
            dragTarget.style.left = dragTarget.layout.x + delta.x / scale.x;

            mouseEvent.StopPropagation();
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (!isActive || !target.HasMouseCapture() || !CanStopManipulation(e))
                return;

            isActive = false;
            target.ReleaseMouse();
            e.StopPropagation();
        }
    }
}
