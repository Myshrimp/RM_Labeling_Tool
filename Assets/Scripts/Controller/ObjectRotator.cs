using UnityEngine;

namespace Controller
{
    public class ObjectRotator : MonoBehaviour
    {
        public enum RotationDirection
        {
            CounterClockwise, // 逆时针
            Clockwise         // 顺时针
        }

        [Header("旋转设置")]
        public Vector3 rotationAxis = Vector3.up;     // 旋转轴方向
        public float rotationSpeed = 45f;             // 旋转速度（度/秒）
        public RotationDirection rotationDirection = RotationDirection.CounterClockwise; // 旋转方向
        public Space rotationSpace = Space.World;     // 旋转参考坐标系

        /// <summary>
        /// 设置旋转参数
        /// </summary>
        public void SetRotation(Vector3 axis, float speed, RotationDirection direction)
        {
            rotationAxis = axis.normalized;
            rotationSpeed = speed;
            rotationDirection = direction;
        }

        /// <summary>
        /// 动态改变旋转方向
        /// </summary>
        public void ChangeRotationDirection(RotationDirection newDirection)
        {
            rotationDirection = newDirection;
        }

        /// <summary>
        /// 切换顺时针/逆时针
        /// </summary>
        public void ToggleRotationDirection()
        {
            rotationDirection = (rotationDirection == RotationDirection.Clockwise) 
                ? RotationDirection.CounterClockwise 
                : RotationDirection.Clockwise;
        }

        /// <summary>
        /// 获取当前实际旋转速度（考虑方向）
        /// </summary>
        private float GetActualRotationSpeed()
        {
            return (rotationDirection == RotationDirection.Clockwise) 
                ? -rotationSpeed 
                : rotationSpeed;
        }

        void Update()
        {
            // 应用旋转
            transform.Rotate(rotationAxis, GetActualRotationSpeed() * Time.deltaTime, rotationSpace);
        }

        /// <summary>
        /// 在Inspector中可视化旋转轴和方向
        /// </summary>
        void OnDrawGizmosSelected()
        {
            // 绘制旋转轴
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, rotationAxis * 2f);

            // 绘制旋转方向指示器
            if (rotationAxis != Vector3.zero)
            {
                // 计算一个垂直于旋转轴的向量用于显示方向
                Vector3 perpendicular = Vector3.Cross(rotationAxis, Vector3.up);
                if (perpendicular == Vector3.zero)
                    perpendicular = Vector3.Cross(rotationAxis, Vector3.forward);
                
                perpendicular = perpendicular.normalized * 0.5f;

                // 根据旋转方向绘制箭头
                Gizmos.color = (rotationDirection == RotationDirection.Clockwise) ? Color.blue : Color.green;
                
                Vector3 startPoint = transform.position + perpendicular;
                Vector3 endPoint = transform.position - perpendicular;
                
                // 绘制方向箭头
                DrawArrow(startPoint, endPoint, Gizmos.color);
            }
        }

        /// <summary>
        /// 绘制箭头
        /// </summary>
        private void DrawArrow(Vector3 start, Vector3 end, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(start, end);

            // 绘制箭头头部
            Vector3 direction = (end - start).normalized;
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 30, 0) * Vector3.back;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -30, 0) * Vector3.back;

            Gizmos.DrawLine(end, end + right * 0.2f);
            Gizmos.DrawLine(end, end + left * 0.2f);
        }
    }
}