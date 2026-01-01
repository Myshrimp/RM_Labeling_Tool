using UnityEngine;

namespace Robo.Core.Math
{
    public class Math
    {
        public static Vector3[] CalculatePlaneVertices(Vector3 planeNormal, Vector3 centerPoint, Vector3 planeSize)
        {
            // 归一化法线向量
            planeNormal = planeNormal.normalized;
        
            // 计算平面内的两个垂直轴（右方向和上方向）
            // 先找一个与世界坐标系不完全平行的参考向量
            Vector3 referenceVector;
            if (Mathf.Abs(Vector3.Dot(planeNormal, Vector3.up)) < 0.9f)
            {
                referenceVector = Vector3.up;
            }
            else
            {
                referenceVector = Vector3.right;
            }
        
            // 计算右方向（平面内的一个轴）
            Vector3 right = Vector3.Cross(planeNormal, referenceVector).normalized;
            // 计算上方向（平面内的另一个轴，与右方向和法线都垂直）
            Vector3 up = Vector3.Cross(right, planeNormal).normalized;
        
            // 计算半宽和半高
            float halfWidth = planeSize.x * 0.5f;
            float halfHeight = planeSize.y * 0.5f;
        
            // 计算四个顶点（从中心点偏移）
            Vector3[] vertices = new Vector3[4];
        
            // 左上顶点
            vertices[0] = centerPoint + (-right * halfWidth) + (up * halfHeight);
            // 右上顶点
            vertices[1] = centerPoint + (right * halfWidth) + (up * halfHeight);
            // 右下顶点
            vertices[2] = centerPoint + (right * halfWidth) + (-up * halfHeight);
            // 左下顶点
            vertices[3] = centerPoint + (-right * halfWidth) + (-up * halfHeight);
        
            return vertices;
        }
    }
}