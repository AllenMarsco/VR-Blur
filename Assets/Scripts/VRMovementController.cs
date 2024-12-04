using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class VRMovementController : MonoBehaviour
{
    [Header("移动设置")]
    public float speed = 3.0f; // 移动速度
    public float gravity = -9.8f; // 重力参数
    public float jumpHeight = 1.5f; // 跳跃高度
    public Transform cameraTransform; // 摄像机 Transform，用于确定移动方向

   
private CharacterController _characterController; // 玩家控制器
    private Vector3 _velocity; // 垂直方向的速度，用于模拟重力
    private bool _isJumping; // 标记是否正在跳跃

    void Start()
    {
        // 获取 CharacterController 组件
        _characterController = GetComponent<CharacterController>();
        if (_characterController == null)
        {
            Debug.LogError("需要 CharacterController 组件！");
        }

        // 如果未指定摄像机方向，尝试自动获取
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main != null ? Camera.main.transform : null;
            if (cameraTransform == null)
            {
                Debug.LogError("未找到摄像机 Transform，请在脚本中设置 Camera Transform！");
            }
        }
    }

    void Update()
    {
        // 获取 VR 手柄输入
        Vector2 input = GetInput();

        // 如果没有输入，直接返回（避免无意义的移动计算）
        if (input.magnitude < 0.1f)
        {
            input = Vector2.zero;
        }

        // 基于摄像机方向计算移动向量
        Vector3 moveDirection = Vector3.zero;
        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            // 忽略摄像机的垂直方向
            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            // 计算移动向量
            moveDirection = forward * input.y + right * input.x;
        }

        // 移动玩家
        _characterController.Move(moveDirection * speed * Time.deltaTime);

        // 检测跳跃输入
        HandleJump();

        // 应用重力
        if (_characterController.isGrounded)
        {
            _velocity.y = 0; // 如果玩家在地面上，重置垂直速度
            _isJumping = false; // 重置跳跃状态
        }
        else
        {
            _velocity.y += gravity * Time.deltaTime; // 如果玩家离开地面，增加重力
        }

        // 应用重力到角色移动
        _characterController.Move(_velocity * Time.deltaTime);
    }

    /// <summary>
    /// 处理跳跃逻辑
    /// </summary>
    private void HandleJump()
    {
        // 检测 Oculus 手柄上的跳跃按钮（通常是右手柄的 A 按钮）
        bool jumpInput = OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick);
        Debug.Log("ground状态是：" + _characterController.isGrounded + "  ,isJumping:" + _isJumping);

        if (jumpInput && !_isJumping)
        {
            // 计算跳跃速度（根据公式 v = sqrt(2 * -gravity * jumpHeight)）
            _velocity.y = Mathf.Sqrt(2 * -gravity * jumpHeight);
            _isJumping = true; // 标记为跳跃状态
        }
    }

    /// <summary>
    /// 获取输入（支持 VR 手柄）
    /// </summary>
    /// <returns>返回二维的输入向量（X 和 Y）</returns>
    private Vector2 GetInput()
    {
        // 获取右手柄的摇杆输入
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTouch))
        {
            return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        }

        // 如果手柄未连接，返回零向量
        return Vector2.zero;
    }
}