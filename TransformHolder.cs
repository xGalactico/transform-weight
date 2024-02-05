using System;
using UnityEngine;

public class TransformHolder : MonoBehaviour
{
    #region Internal
    [Serializable]
    public class TransformWeight
    {
        public enum Axis
        {
            X,
            Y,
            Z,
        }

        [Range(0, 1)]
        public float X, Y, Z;

        public void SetWeight(Axis axis, float value)
        {
            switch (axis)
            {
                case Axis.Y: Y = value; break;
                case Axis.Z: Z = value; break;
                default: X = value; break;
            }
        }
    }
    #endregion

    [Header("Transform Config :")]
    [SerializeField] private bool m_enabled = true;
    [SerializeField] private TransformWeight m_positionWeight = new() { X = 0.2f, Y = 0.2f, Z = 0.2f};
    [SerializeField] private TransformWeight m_rotationWeight = new();

    [Header("References :")]
    [SerializeField] private Transform m_transformTarget = null;

    private TransformWeight _currentPositionWeight;
    private TransformWeight _currentRotationWeight;

    public TransformWeight PositionWeight { get { return _currentPositionWeight; }}
    public TransformWeight RotationWeight { get { return _currentRotationWeight; }}
    public void ResetPositionWeight() => _currentPositionWeight = m_positionWeight;
    public void ResetRotationWeight() => _currentRotationWeight = m_rotationWeight;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        SetupTransformWeight();
    }

    private void SetupTransformWeight()
    {
        _currentPositionWeight = m_positionWeight;
        _currentRotationWeight = m_rotationWeight;
    }

    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    void LateUpdate()
    {
        HandlePosition();
        HandleRotation();
    }

    private void HandlePosition()
    {
        // Check if a target transform object is assigned
        if (!m_enabled || !m_transformTarget)
        {
            return; // If not, exit the function
        }

        // Get the position of the target object
        Vector3 targetPosition = m_transformTarget.position;

        // Gradually interpolate towards the position of the target object with weights for each axis
        float targetX = Mathf.Lerp(transform.position.x, targetPosition.x, _currentPositionWeight.X);
        float targetY = Mathf.Lerp(transform.position.y, targetPosition.y, _currentPositionWeight.Y);
        float targetZ = Mathf.Lerp(transform.position.z, targetPosition.z, _currentPositionWeight.Z);

        // Create the final position vector with the interpolated values
        Vector3 finalPosition = new(targetX, targetY, targetZ);

        // Assign the final result to the local position of the transform
        transform.position = finalPosition;
    }

    private void HandleRotation()
    {
        // Check if a target transform object is assigned
        if (!m_enabled || !m_transformTarget)
        {
            return; // If not, exit the function
        }

        // Get the rotation angles of the target object
        var offset = m_transformTarget.eulerAngles;

        // Gradually interpolate towards the rotation of the target object with weights for each axis
        float targetX = Mathf.LerpAngle(transform.eulerAngles.x, offset.x, _currentRotationWeight.X);
        float targetY = Mathf.LerpAngle(transform.eulerAngles.y, offset.y, _currentRotationWeight.Y);
        float targetZ = Mathf.LerpAngle(transform.eulerAngles.z, offset.z, _currentRotationWeight.Z);

        // Create the final quaternion with the interpolated angles
        Quaternion finalRotation = Quaternion.Euler(targetX, targetY, targetZ);

        // Assign the final result to the local rotation of the transform
        transform.rotation = finalRotation;
    }
}
