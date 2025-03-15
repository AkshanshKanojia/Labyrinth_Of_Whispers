using UnityEngine;

namespace FPS
{
    public class PlayerInteractionHandler : MonoBehaviour
    {
        [Header("Interaction Properties")]
        [SerializeField] private Transform _interactionDetectionPoint;
        [SerializeField] private float _cameraViewValidationRadius = 1f, _cameraViewValidationDistance = 1f, _minPalyerDistanceForInteraction = 0.25f;
        [SerializeField] private LayerMask _interactionLayerMask;

        [Header("Debug Properties")]
        [SerializeField] private bool _drawInteractionDetectionGizmos = true;
        [SerializeField] private int _spehereCastResolution = 10;

        internal bool interactionsEnabled = false, isInteractingWithObject;
        internal IInteractable activeInteractableItem;

        private Player _player;
        private Transform _cameraTransform;

        #region Initialization
        internal void InitalizeInteractions(Player player)
        {
            _player = player;
            _cameraTransform = _player.CameraTransform;
        }
        #endregion

        #region Interaction Methods
        internal bool CheckForInteractions()
        {
            if (isInteractingWithObject || !interactionsEnabled)
            {
                //if already interacting, return
                return false;
            }

            if (Physics.SphereCast(_interactionDetectionPoint.transform.position, _cameraViewValidationRadius, _cameraTransform.forward,
                out RaycastHit hit, _cameraViewValidationDistance, _interactionLayerMask))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    Vector3 interactablePoint = hit.point;
                    interactablePoint.y = transform.position.y;

                    if ((interactablePoint - transform.position).magnitude > _minPalyerDistanceForInteraction)
                    {
                        return false;
                    }

                    //only interact if object is visible to player and within interaction range
                    activeInteractableItem = interactable;
                    activeInteractableItem.OnInteractionBegin();
                    isInteractingWithObject = true;
                    return true;
                }
            }

            return false;
        }

        internal void CancelInteraction()
        {
            if (!isInteractingWithObject || !interactionsEnabled)
            {
                //if not interacting, return
                return;
            }

            isInteractingWithObject = false;
            activeInteractableItem.OnInteractionEnd();
        }

        internal void SetInteractions(bool enabled)
        {
            interactionsEnabled = enabled;
        }
        #endregion

        #region Debug

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_drawInteractionDetectionGizmos)
            {
                if (!_cameraTransform)
                {
                    _cameraTransform = _interactionDetectionPoint;
                }

                Gizmos.color = Color.red;

                Gizmos.DrawWireSphere(_interactionDetectionPoint.position, _cameraViewValidationRadius);
                Gizmos.DrawLine(_interactionDetectionPoint.position, _interactionDetectionPoint.position + _cameraTransform.forward * _cameraViewValidationDistance);

                float splitDistance = _cameraViewValidationDistance / (_spehereCastResolution - 1);

                for (int i = 0; i < (_spehereCastResolution - 1); i++)
                {
                    Gizmos.DrawWireSphere(_interactionDetectionPoint.position + _cameraTransform.forward * splitDistance * (i + 1), _cameraViewValidationRadius);
                }

                //draw player validation radius
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, _minPalyerDistanceForInteraction);
            }
        }
#endif

        #endregion
    }
}
