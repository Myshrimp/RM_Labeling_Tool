using System.IO;
using UnityEngine;
namespace Robo.Utils
{
    public class PhotoUtil
    {
            /// <summary>
    /// 将归一化的Vector2数组绘制到Texture2D上
    /// </summary>
    /// <param name="normalizedPoints">已归一化的Vector2数组（每个分量的范围[0,1]）</param>
    /// <param name="texture">目标纹理</param>
    /// <param name="pointColor">点的颜色</param>
    /// <param name="backgroundColor">背景颜色</param>
    /// <param name="pointRadius">点的半径（像素）</param>
    /// <param name="applyTexture">是否应用纹理更改</param>
    /// <returns>处理后的Texture2D</returns>
    public static Texture2D PlotPointsOnTexture(
        Vector2[] normalizedPoints, 
        Texture2D texture,
        Color pointColor,
        Color backgroundColor,
        int pointRadius = 2,
        bool applyTexture = true)
    {
        if (texture == null)
        {
            Debug.LogError("Texture is null!");
            return null;
        }

        if (normalizedPoints == null || normalizedPoints.Length == 0)
        {
            Debug.LogWarning("No points to plot!");
            return texture;
        }

        // 获取纹理尺寸
        int width = texture.width;
        int height = texture.height;

        // 创建颜色数组用于设置纹理
        Color[] pixels = texture.GetPixels();
        
        // 设置背景颜色
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }

        // 绘制每个点
        foreach (Vector2 normalizedPoint in normalizedPoints)
        {
            // 将归一化坐标转换为纹理坐标
            // 注意：Unity纹理坐标原点在左下角
            int pixelX = Mathf.FloorToInt(normalizedPoint.x * (width - 1));
            int pixelY = Mathf.FloorToInt(normalizedPoint.y * (height - 1));
            
            // 确保坐标在纹理范围内
            pixelX = Mathf.Clamp(pixelX, 0, width - 1);
            pixelY = Mathf.Clamp(pixelY, 0, height - 1);
            
            // 绘制点（带半径）
            DrawPoint(pixelX, pixelY, pointRadius, pointColor, pixels, width, height);
        }

        // 应用像素到纹理
        texture.SetPixels(pixels);
        
        if (applyTexture)
        {
            texture.Apply();
        }

        return texture;
    }

    /// <summary>
    /// 绘制一个点（带半径的圆）
    /// </summary>
    private static void DrawPoint(int centerX, int centerY, int radius, Color color, Color[] pixels, int width, int height)
    {
        int radiusSqr = radius * radius;
        
        // 遍历以点为中心的方形区域
        for (int x = centerX - radius; x <= centerX + radius; x++)
        {
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                // 检查是否在纹理范围内
                if (x < 0 || x >= width || y < 0 || y >= height)
                    continue;
                
                // 计算到圆心的距离平方
                int dx = x - centerX;
                int dy = y - centerY;
                int distanceSqr = dx * dx + dy * dy;
                
                // 如果在半径范围内，则设置颜色
                if (distanceSqr <= radiusSqr)
                {
                    int index = y * width + x;
                    
                    // 可选：使用抗锯齿（距离越远，透明度越低）
                    float alphaFactor = 1f - (Mathf.Sqrt(distanceSqr) / radius);
                    Color finalColor = color;
                    finalColor.a = color.a * alphaFactor;
                    
                    // 混合颜色（考虑原有的背景色）
                    pixels[index] = Color.Lerp(pixels[index], finalColor, finalColor.a);
                }
            }
        }
    }
        public static string PlotAndSavePoints(
            Vector2[] normalizedPoints,
            Texture2D texture,
            string savePath = "PointPlots",
            string fileName = "point_plot")
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            // 绘制点
            Texture2D resultTexture = PlotPointsOnTexture(
                normalizedPoints,
                texture,
                Color.red,
                Color.white,
                3
            );

            if (resultTexture == null)
            {
                Debug.LogError("Failed to plot points!");
                return null;
            }
            
            // 生成唯一文件名
            string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fullFileName = $"{fileName}_{timestamp}.png";
            string filePath = savePath+fullFileName;
            
            // 将纹理编码为PNG
            byte[] pngData = resultTexture.EncodeToPNG();
        
            // 保存文件
            File.WriteAllBytes(filePath, pngData);
        
            // 刷新Unity资产数据库（以便在编辑器中立即看到新文件）
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif

            Debug.Log($"Point plot saved to: {filePath}");
        
            // 返回相对路径（便于在Unity中使用）
            return savePath+fullFileName;
        }
    }
}