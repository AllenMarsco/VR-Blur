using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class VRMovementController : MonoBehaviour
{
    [Header("�ƶ�����")]
    public float speed = 3.0f; // �ƶ��ٶ�
    public float gravity = -9.8f; // ��������
    public float jumpHeight = 1.5f; // ��Ծ�߶�
    public Transform cameraTransform; // ����� Transform������ȷ���ƶ�����

   
private CharacterController _characterController; // ��ҿ�����
    private Vector3 _velocity; // ��ֱ������ٶȣ�����ģ������
    private bool _isJumping; // ����Ƿ�������Ծ

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
    }

    void Update()
    {
        // ��ȡ VR �ֱ�����
        Vector2 input = GetInput();

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

        // �����Ծ����
        HandleJump();

        // Ӧ������
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
    /// ������Ծ�߼�
    /// </summary>
    private void HandleJump()
    {
        // ��� Oculus �ֱ��ϵ���Ծ��ť��ͨ�������ֱ��� A ��ť��
        bool jumpInput = OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick);
        Debug.Log("ground״̬�ǣ�" + _characterController.isGrounded + "  ,isJumping:" + _isJumping);

        if (jumpInput && !_isJumping)
        {
            // ������Ծ�ٶȣ����ݹ�ʽ v = sqrt(2 * -gravity * jumpHeight)��
            _velocity.y = Mathf.Sqrt(2 * -gravity * jumpHeight);
            _isJumping = true; // ���Ϊ��Ծ״̬
        }
    }

    /// <summary>
    /// ��ȡ���루֧�� VR �ֱ���
    /// </summary>
    /// <returns>���ض�ά������������X �� Y��</returns>
    private Vector2 GetInput()
    {
        // ��ȡ���ֱ���ҡ������
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTouch))
        {
            return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        }

        // ����ֱ�δ���ӣ�����������
        return Vector2.zero;
    }
}