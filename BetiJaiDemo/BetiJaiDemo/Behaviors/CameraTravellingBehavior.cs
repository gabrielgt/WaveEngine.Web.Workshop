using System;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;

namespace BetiJaiDemo.Behaviors
{
    public class CameraTravellingBehavior : Behavior
    {
        private const int CameraSmoothTimeSeconds = 3;
        private Vector3 _cameraPositionCurrentVelocity;
        private Vector3 _cameraRotationCurrentVelocity;
        private Vector3 _cameraTargetPosition;
        private Vector3 _cameraTargetRotation;
        private bool _isCameraAnimationInProgress;

        [BindComponent]
        public Transform3D CameraTransform;

        public void AnimateTo(Vector3 position, Vector3 rotation)
        {
            _cameraTargetPosition = position;
            _cameraTargetRotation = rotation;
            _isCameraAnimationInProgress = true;
        }

        public void FixTo(Vector3 position, Vector3 rotation)
        {
            CameraTransform.Position = position;
            CameraTransform.Rotation = rotation;
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (!_isCameraAnimationInProgress)
            {
                return;
            }

            CameraTransform.Position = Vector3.SmoothDamp(
                CameraTransform.Position, _cameraTargetPosition, ref _cameraPositionCurrentVelocity, CameraSmoothTimeSeconds, (float) gameTime.TotalSeconds);

            CameraTransform.Rotation = Vector3.SmoothDamp(
                CameraTransform.Rotation, _cameraTargetRotation, ref _cameraRotationCurrentVelocity, CameraSmoothTimeSeconds, (float) gameTime.TotalSeconds);

            const float positionGap = 0.1f;
            const float rotationGap = 0.01f;
            if (Vector3.Distance(CameraTransform.Position, _cameraTargetPosition) <= positionGap &&
                Vector3.Distance(CameraTransform.Rotation, _cameraTargetRotation) <= rotationGap)
            {
                _isCameraAnimationInProgress = false;
            }
        }
    }
}