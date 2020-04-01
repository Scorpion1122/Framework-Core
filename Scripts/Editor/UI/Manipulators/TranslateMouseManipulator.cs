using UnityEngine;
using UnityEngine.UIElements;

namespace TKO.Framework.UI
{
    public class TranslateMouseManipulator : MouseManipulator
    {
        private VisualElement translateTarget;
        private bool isActive;
        
        public TranslateMouseManipulator(VisualElement translateTarget, MouseButton button)
        {
            this.translateTarget = translateTarget;
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

            Vector2 delta = mouseEvent.mouseDelta;

            Vector3 position = translateTarget.transform.position;
            position.y = position.y + delta.y;
            position.x = position.x + delta.x;
            translateTarget.transform.position = position;

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