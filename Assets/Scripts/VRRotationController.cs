using UnityEngine;

public class VRRotationController : MonoBehaviour
{
    [Header("��ת����")]
    public float rotationSpeed = 45f; // ��ת�ٶȣ���/�룩

    

    
void Update()
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
}