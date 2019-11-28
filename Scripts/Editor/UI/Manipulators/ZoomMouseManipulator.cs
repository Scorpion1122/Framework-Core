using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Thijs.Framework.UI
{
    public class ZoomMouseManipulator : MouseManipulator
    {
        private VisualElement zoomTarget;
        private float zoomSpeed = 0.05f;
        private float maxZoom = 1f;
        private float minZoom = 0.5f;

        public ZoomMouseManipulator(VisualElement zoomTarget)
        {
            this.zoomTarget = zoomTarget;
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.MiddleMouse });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<WheelEvent>(OnScrollWheel);
        }

        private void OnScrollWheel(WheelEvent mouseEvent)
        {
            float delta = mouseEvent.delta.y * zoomSpeed * -1f;

            Vector3 position = zoomTarget.transform.position;
            Vector3 scale = zoomTarget.transform.scale;

            Vector2 mouseOffset = target.ChangeCoordinatesTo(zoomTarget, mouseEvent.localMousePosition);
            float x = mouseOffset.x + zoomTarget.layout.x;
            float y = mouseOffset.y + zoomTarget.layout.y;

            Vector3 positionOffset = position + Vector3.Scale(new Vector3(x, y, 0.0f), scale);
            scale.y = scale.y + delta;
            scale.x = scale.x + delta;

            scale.y = Mathf.Clamp(scale.y, minZoom, maxZoom);
            scale.x = Mathf.Clamp(scale.x, minZoom, maxZoom);
            scale.z = 1f;

            Vector3 newPosition = positionOffset - Vector3.Scale(new Vector3(x, y, 0.0f), scale);

            zoomTarget.transform.position = newPosition;
            zoomTarget.transform.scale = scale;
            mouseEvent.StopPropagation();
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<WheelEvent>(OnScrollWheel);
        }
    }
}
