using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pisit
{
    public class PisitCharacterController : MonoBehaviour
    {
        [System.Serializable]
        public class MinMaxValue
        {
            public float max;
            public float min;
            public MinMaxValue()
            {
                max = 0f;
                min = 0f;
            }
        }

        [SerializeField] private CharcterInput _characterInput;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] Vector3 moveDirection;

        [SerializeField] private float cameraLookAngle = 0f; // Y
        [SerializeField] private float cameraPivotAngle = 0f; // X

        [SerializeField] private float cameraLookSpeed = 2f; // Y
        [SerializeField] private float cameraPivotSpeed = 2f; // X

        [SerializeField] private float minimumPivotAngle = -15f; // x
        [SerializeField] private float maximumPivotAngle = 55f; // x

        [SerializeField]
        //private MinMaxValue pivotClampBackCamera = new MinMaxValue();

        public Transform mainCamera;

        [SerializeField]
        private Transform _cameraFollowTarget;

        // Interface
        public float MoveSpeed => moveSpeed;

        void Awake()
        {
            _characterInput = GetComponent<CharcterInput>();
            _characterController = GetComponent<CharacterController>();

            if (_characterInput == null )
            {
                Debug.LogError("Character Input Component is missing");
            }
            if (_characterController == null)
            {
                Debug.LogError("CharacterController Component is missing");
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        // Update is called once per frame
        void Update()
        {
            HandleCameraRotation();
            HandleMoveInput();
        }

        private void HandleCameraRotation()
        {
            cameraLookAngle += (_characterInput.LookInput.x * cameraLookSpeed * Time.deltaTime);

            cameraPivotAngle -= (_characterInput.LookInput.y * cameraPivotSpeed * Time.deltaTime);
            cameraPivotAngle = Mathf.Clamp(cameraPivotAngle, minimumPivotAngle, maximumPivotAngle);

            // handle actual rotation
            Vector3 rotation = Vector3.zero;

            rotation.x = cameraPivotAngle;
            rotation.y = cameraLookAngle;

            // create a new rotation with delta offset from look input
            Quaternion newRotation = Quaternion.Euler(rotation);
            _cameraFollowTarget.rotation = newRotation;
        }

        private void HandleMoveInput()
        {
            //throw new NotImplementedException();
            // Create 3D Vector from your input

            //moveDirection =
            //    transform.forward * _characterInput.MoveInput.y +
            //    transform.right * _characterInput.MoveInput.x;

            moveDirection =
                mainCamera.forward * _characterInput.MoveInput.y +
                mainCamera.right * _characterInput.MoveInput.x;

            moveDirection.Normalize(); // Remove beyond 1 speed in diagonal

            moveDirection.y = 0f;

            _characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
}
