using Controllers;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private Collider collider;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float movementSpeed = 2f;
        [SerializeField] private float turnSpeed = 2f;
        [SerializeField] private float bulletSpeed = 10;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform gunMuzzle;
        [SerializeField] private bool isPoolable;

        [SerializeField] private PoolingManager poolingManager;

        private Vector2 _movementInputValues, _rotationInputValues;


        private void Awake()
        {
            AssignComponents();
        }

        private void AssignComponents()
        {
            if (collider == null) collider = GetComponent<Collider>();
            if (characterController == null) characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            _movementInputValues = GetMovementInputData();
            _rotationInputValues = GetRotationMovementData();

            if (Input.GetMouseButtonDown(0))
            {
                if (!isPoolable)
                    FireBulletWithoutPooling();
                else FireBulletWithPooling();
            }
        }

        private void FixedUpdate()
        {
            if (!Input.anyKey) return;
            Move();
            Rotate();
        }


        private Vector2 GetMovementInputData()
        {
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        private Vector2 GetRotationMovementData()
        {
            return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        }

        #region Movement

        private void Move()
        {
            Vector3 move = new Vector3(_movementInputValues.x * movementSpeed * Time.fixedDeltaTime, 0,
                _movementInputValues.y * movementSpeed * Time.fixedDeltaTime);

            move = (Camera.main.transform.forward * move.z) + (Camera.main.transform.right * move.x);
            move.y = 0;

            characterController.Move(new Vector3(move.x, -1,
                move.z));
        }

        private void Rotate()
        {
            float targetAngle = Mathf.Atan2(_rotationInputValues.x, _rotationInputValues.y) * Mathf.Rad2Deg +
                                Camera.main.transform.eulerAngles.y;

            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
        }

        #endregion

        #region Shooting

        private void FireBulletWithoutPooling()
        {
            GameObject bullet = Instantiate(bulletPrefab, gunMuzzle.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().AddForce(gunMuzzle.forward * bulletSpeed, ForceMode.Impulse);
        }

        private void FireBulletWithPooling()
        {
            var bullet = PoolingManager.Instance.DequeuePoolableGameObject();
            bullet.transform.position = gunMuzzle.position;
            bullet.GetComponent<BulletController>().IsCalledByPooling = true;
            bullet.GetComponent<Rigidbody>().AddForce(gunMuzzle.forward * bulletSpeed, ForceMode.Impulse);
        }

        #endregion
    }
}