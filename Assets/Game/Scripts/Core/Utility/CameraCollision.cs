//https://gist.github.com/kroltan/6a87e8a47438d4db1b2edfeb8e44a803

using UnityEngine;

namespace Game.Scripts.Core.Utility
{
    [RequireComponent(typeof(Camera))]
    public class CameraCollision : MonoBehaviour
    {
        [field: SerializeField] public LayerMask LayerMask { get; private set; } = -1;
        [field: SerializeField] public float TargetDistance { get; private set; } = 5;
        [field: SerializeField] public float Speed { get; private set; } = 1;

        private Camera _camera;
        private Vector3 _targetPosition;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            _targetPosition = transform.position;
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Speed * Time.deltaTime);

            //Ideal position is where the camera wants to be when there are no obstacles
            var idealPosition = transform.parent.position - transform.forward * TargetDistance;
            //Anti-clipping protection
            var wallDistance = _camera.nearClipPlane;
            Ray ray = new Ray(transform.parent.position, -transform.forward);
            var collided = Physics.SphereCast(ray, wallDistance,
                out var hit,
                TargetDistance,
                LayerMask
            );

            if (collided && hit.distance < TargetDistance)
            {
                _targetPosition = hit.point + hit.normal * wallDistance;
                var delta = transform.position - _targetPosition;

                //Camera is in the wall! Move it instantly
                if (Vector3.Dot(delta.normalized, transform.forward) < 0)
                {
                    transform.position = _targetPosition;
                }
            }
            else
            {
                _targetPosition = idealPosition;
            }
        }
    }
}