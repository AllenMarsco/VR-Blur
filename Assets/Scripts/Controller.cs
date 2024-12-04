using System;
using UnityEngine;
using Oculus.Interaction.Locomotion;

[RequireComponent(typeof(CharacterController))]
public class Controller : MonoBehaviour, ILocomotionEventHandler
{
    [Header("�ƶ�����")]
    public float speed = 3.0f; // �ƶ��ٶ�
    public float gravity = -9.8f; // ��������
    public float jumpHeight = 1.5f; // ��Ծ�߶�
    public Transform cameraTransform; // ����� Transform������ȷ���ƶ�����

    [Header("��ת����")]
    public float rotationSpeed = 45f; // ��ת�ٶȣ���/�룩

    private CharacterController _characterController; // ��ҿ�����
    private Vector3 _velocity; // ��ֱ������ٶȣ�����ģ������
    private Vector3 _lastPosition; // ��¼��һ֡��λ�ã����ڼ����ٶ�
    private bool _isJumping; // ����Ƿ�������Ծ

    // ʵ�� ILocomotionEventHandler ���¼�
    public event Action<LocomotionEvent, Pose> WhenLocomotionEventHandled;

    void Start()
    {
        // ��ȡ CharacterController ���
        _characterController = GetComponent<CharacterController>();
        if (_characterController == null)
        {
            Debug.LogError("��Ҫ CharacterController �����");
        }

        // ���δָ����������򣬳����Զ���ȡ
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main != null ? Camera.main.transform : null;
            if (cameraTransform == null)
            {
                Debug.LogError("δ�ҵ������ Transform�����ڽű������� Camera Transform��");
            }
        }

        // ��ʼ�� _lastPosition Ϊ��ǰ�ĳ�ʼλ��
        _lastPosition = transform.position;
    }

    void Update()
    {
        // ��ȡ�������ƶ�����
        HandleMovement();

        // ��ȡ��������ת����
        HandleRotation();

        // �����Ծ����
        HandleJump();

        // Ӧ������
        ApplyGravity();

        // �ֶ������˶��¼�
        TriggerLocomotionEvent();
    }

    /// <summary>
    /// �����ƶ��߼�
    /// </summary>
    private void HandleMovement()
    {
        // ��ȡ VR �ֱ�����
        Vector2 input = GetMovementInput();

        // ���û�����룬ֱ�ӷ��أ�������������ƶ����㣩
        if (input.magnitude < 0.1f)
        {
            input = Vector2.zero;
        }

        // �����������������ƶ�����
        Vector3 moveDirection = Vector3.zero;
        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            // ����������Ĵ�ֱ����
            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            // �����ƶ�����
            moveDirection = forward * input.y + right * input.x;
        }

        // �ƶ����
        _characterController.Move(moveDirection * speed * Time.deltaTime);
    }

    /// <summary>
    /// ������ת�߼�
    /// </summary>
    private void HandleRotation()
    {
        // ��ȡ���ֱ�ҡ�˵����루PrimaryThumbstick רָ���ֱ���
        Vector2 leftThumbstickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

        // ���������Ȳ��㣨С�� 0.1����ֱ�ӷ��أ����ⲻ��Ҫ����ת����
        if (leftThumbstickInput.magnitude < 0.1f)
        {
            return;
        }

        // ��ȡˮƽ����ֵ��X �ᣩ��������ת
        float rotationInput = leftThumbstickInput.x;

        // ������ת�Ƕ�
        float rotationAmount = rotationInput * rotationSpeed * Time.deltaTime;

        // ��ת���
        transform.Rotate(0, rotationAmount, 0);
    }

    /// <summary>
    /// ������Ծ�߼�
    /// </summary>
    private void HandleJump()
    {
        // ��� Oculus �ֱ��ϵ���Ծ��ť��ͨ�������ֱ��� A ��ť��
        bool jumpInput = OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick);

        if (jumpInput && !_isJumping)
        {
            // ������Ծ�ٶȣ����ݹ�ʽ v = sqrt(2 * -gravity * jumpHeight)��
            _velocity.y = Mathf.Sqrt(2 * -gravity * jumpHeight);
            _isJumping = true; // ���Ϊ��Ծ״̬
        }
    }

    /// <summary>
    /// Ӧ�������߼�
    /// </summary>
    private void ApplyGravity()
    {
        if (_characterController.isGrounded)
        {
            _velocity.y = 0; // �������ڵ����ϣ����ô�ֱ�ٶ�
            _isJumping = false; // ������Ծ״̬
        }
        else
        {
            _velocity.y += gravity * Time.deltaTime; // �������뿪���棬��������
        }

        // Ӧ����������ɫ�ƶ�
        _characterController.Move(_velocity * Time.deltaTime);
    }

    /// <summary>
    /// ��ȡ�ƶ����루֧�� VR �ֱ���
    /// </summary>
    /// <returns>���ض�ά������������X �� Y��</returns>
    private Vector2 GetMovementInput()
    {
        // ��ȡ���ֱ���ҡ������
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTouch))
        {
            return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        }

        // ����ֱ�δ���ӣ�����������
        return Vector2.zero;
    }

    /// <summary>
    /// ���� Locomotion �¼��������ٶ�
    /// </summary>
    private void TriggerLocomotionEvent()
    {
        // ���㵱ǰ֡���ٶȣ�������һ֡λ�ã�
        Vector3 currentPosition = transform.position;
        Vector3 velocity = (currentPosition - _lastPosition) / Time.deltaTime; // �ٶ� = λ�� / ʱ��

        // ���� _lastPosition Ϊ��ǰ֡��λ��
        _lastPosition = currentPosition;

        // ���� LocomotionEvent
        LocomotionEvent locomotionEvent = new LocomotionEvent(
            identifier: 1,
            pose: new Pose(velocity, transform.rotation), // �˴����ٶȸ�ֵ�� pose.position
            translationType: LocomotionEvent.TranslationType.Velocity,
            rotationType: LocomotionEvent.RotationType.Velocity
        );

        // ��ӡ������Ϣ
        Debug.Log($"[LocomotionEvent] Velocity: {velocity}, Rotation: {transform.rotation.eulerAngles}");
        Debug.Log($"[LocomotionEvent] TranslationType: {locomotionEvent.Translation}, RotationType: {locomotionEvent.Rotation}");

        // �����¼��������
        WhenLocomotionEventHandled?.Invoke(locomotionEvent, new Pose(velocity, transform.rotation));
    }

    /// <summary>
    /// ʵ�� ILocomotionEventHandler �� HandleLocomotionEvent ����
    /// </summary>
    /// <param name="locomotionEvent">�˶��¼�</param>
    public void HandleLocomotionEvent(LocomotionEvent locomotionEvent)
    {
        // ����¼���Ϣ
        Debug.Log($"���� Locomotion �¼�: ID = {locomotionEvent.Identifier}, ƽ������ = {locomotionEvent.Translation}, ��ת���� = {locomotionEvent.Rotation}");
    }
}