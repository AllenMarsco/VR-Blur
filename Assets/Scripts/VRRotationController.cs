using UnityEngine;

public class VRRotationController : MonoBehaviour
{
    [Header("旋转设置")]
    public float rotationSpeed = 45f; // 旋转速度（度/秒）

    

    
void Update()
    {
        // 获取左手柄摇杆的输入（PrimaryThumbstick 专指左手柄）
        Vector2 leftThumbstickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

        // 如果输入幅度不足（小于 0.1），直接返回，避免不必要的旋转计算
        if (leftThumbstickInput.magnitude < 0.1f)
        {
            return;
        }

        // 获取水平输入值（X 轴），用于旋转
        float rotationInput = leftThumbstickInput.x;

        // 计算旋转角度
        float rotationAmount = rotationInput * rotationSpeed * Time.deltaTime;

        // 旋转玩家
        transform.Rotate(0, rotationAmount, 0);
    }
}