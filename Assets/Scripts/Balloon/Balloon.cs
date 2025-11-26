using UnityEngine;

namespace Balloons
{
    public class Balloon : MonoBehaviour
    {
        private float _frequency;
        private float _amplitude;
        private float _movementSpeed;
        private Vector2 _movementDirection;
        private Vector2 _position;

        private bool _isMoving = false;

        private void Update()
        {
            if (_isMoving)
            {
                MoveSinusoidal();
            }
        }

        private void OnDisable()
        {
            _isMoving = false;
        }

        public void InitVectors(Vector2 startPos, Vector2 targetPos, float speed, float frequency, float amplitude)
        {
            _position = startPos;
            _movementDirection = (targetPos - startPos).normalized;
            _movementSpeed = speed;
            _frequency = frequency;
            _amplitude = amplitude;

            _isMoving = true;
        }

        private void MoveSinusoidal()
        {
            _position += _movementDirection * Time.deltaTime * _movementSpeed;
            transform.localPosition = _position + Vector2.Perpendicular(_movementDirection) * Mathf.Sin(Time.time * _frequency) * _amplitude;
        }
    }
}