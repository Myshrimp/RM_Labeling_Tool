using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Label;
using Robo.Data;
using Robo.Parser;
using Robo.Utils;

namespace Robo.Cam
{
[RequireComponent(typeof(Camera))]
    public class CameraCapture : MonoBehaviour
    {
        [Header("导出设置")]
        [Tooltip("照片保存路径")]
        public string savePath;
        [Tooltip("日志路径")] 
        public string logPath;
        [Tooltip("标记数据路径")] 
        public string dataPath;
        [Tooltip("Plot路径")] 
        public string plotPath;
        [Tooltip("照片文件名前缀")]
        public string fileNamePrefix = "capture";
        [Tooltip("照片尺寸（宽）")]
        public int photoWidth = 1920;
        [Tooltip("照片尺寸（高）")]
        public int photoHeight = 1080;

        [Tooltip("照片格式")] 
        public ImageFormat imgFormat;

        [Tooltip("纯色背景")]
        public bool skyboxSolidColor = false;
        [Tooltip("生成关键点标示图")] 
        public bool generatePlotImg = true;
        
        [Header("目标物体绑定")]
        [Tooltip("需要计算坐标的CriticalPoint列表")]
        public List<CriticalPoints> targetObjects = new();
        
        private Camera targetCamera;
        private RenderTexture renderTexture;
        private CriticalPointsParser criticalPointsParser;
        
        void Start()
        {
            savePath = Application.streamingAssetsPath+savePath;
            logPath = Application.streamingAssetsPath+logPath;
            dataPath = Application.streamingAssetsPath+dataPath;
            plotPath = Application.streamingAssetsPath+plotPath;
            targetCamera = GetComponent<Camera>();
            criticalPointsParser = new CriticalPointsParser(targetCamera);
            // 创建保存目录
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
        }
        
        void Update()
        {
            // 示例：按F1键拍照
            if (Input.GetKeyDown(KeyCode.F1))
            {
                CapturePhoto();
            }
        }
        
        /// <summary>
        /// 拍照并计算坐标
        /// </summary>
        public void CapturePhoto()
        {
            StartCoroutine(CapturePhotoCoroutine());
        }
        
        /// <summary>
        /// 异步拍照协程
        /// </summary>
        private IEnumerator CapturePhotoCoroutine()
        {
            // 保存图片
            string imgSuffix = GetImageSuffix();
            string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string prefix = $"{fileNamePrefix}_{timestamp}";
            string photoFileName = $"{prefix}{imgSuffix}";
            string photoPath = savePath+photoFileName;
            // 计算并保存坐标
            var normalizedPoints = CalculateAndSaveCoordinates(prefix);
            
            // 等待一帧确保所有渲染完成
            yield return new WaitForEndOfFrame();
            
            // 创建临时渲染纹理
            if (renderTexture != null)
            {
                RenderTexture.ReleaseTemporary(renderTexture);
            }
            renderTexture = RenderTexture.GetTemporary(photoWidth, photoHeight, 24);
            
            // 备份原始渲染目标
            RenderTexture originalRenderTarget = targetCamera.targetTexture;
            CameraClearFlags originalClearFlags = targetCamera.clearFlags;
            
            // 设置摄像机渲染到临时纹理
            targetCamera.targetTexture = renderTexture;
            if (skyboxSolidColor)
            {
                targetCamera.clearFlags = CameraClearFlags.SolidColor;
            }
            targetCamera.Render();
            
            // 从渲染纹理读取像素
            RenderTexture.active = renderTexture;
            TextureFormat textureFormat = GetTextureFormat();
            Texture2D photo = new Texture2D(photoWidth, photoHeight, textureFormat, false);
            photo.ReadPixels(new Rect(0, 0, photoWidth, photoHeight), 0, 0);
            photo.Apply();
            
            // 恢复原始设置
            targetCamera.targetTexture = originalRenderTarget;
            targetCamera.clearFlags = originalClearFlags;
            RenderTexture.active = null;
            byte[] bytes = GetEncodedBytes(photo);
            File.WriteAllBytes(photoPath, bytes);
            Debug.Log($"照片已保存: {photoPath}");
            
            GeneratePlotImage(normalizedPoints, photo);
            // 销毁临时纹理
            Destroy(photo);
            
            // 清理渲染纹理
            RenderTexture.ReleaseTemporary(renderTexture);
            renderTexture = null;
        }
        
        /// <summary>
        /// 计算并保存坐标
        /// </summary>
        private List<Vector2> CalculateAndSaveCoordinates(string prefix)
        {
            string logFileName= $"{prefix}_log.txt";
            string dataFileName = $"{prefix}.txt";

            string logFilePath = logPath+logFileName;
            string dataFilePath = dataPath+dataFileName;
            StringBuilder log = new StringBuilder();
            StringBuilder data = new StringBuilder();
            log.AppendLine("=== 目标物体归一化坐标 ===");
            log.AppendLine($"照片尺寸: {photoWidth} x {photoHeight}");
            log.AppendLine($"计算时间: {System.DateTime.Now}");
            log.AppendLine();
            
            // 计算目标物体的归一化坐标
            log.AppendLine("目标物体坐标:");
            int visibleCount = 0;
            List<PointData> pointDatas = criticalPointsParser.Parse(targetObjects);
            List<Vector2> normalizedPoints = new List<Vector2>();
            foreach (var pd in pointDatas)
            {
                StringBuilder line = new StringBuilder();
                line.Append(pd.theClass.ToString());
                int pointCount = 0;
                foreach (var pos in pd.points)
                {
                    Vector3 viewportPos = targetCamera.WorldToViewportPoint(pos);
                
                    // 检查物体是否在摄像机视野内
                    if (viewportPos.z > 0 && 
                        viewportPos.x >= 0 && viewportPos.x <= 1 && 
                        viewportPos.y >= 0 && viewportPos.y <= 1)
                    {
                        // 转换为像素坐标
                        float pixelX = viewportPos.x * photoWidth;
                        float pixelY = viewportPos.y * photoHeight;
                    
                        // 计算归一化坐标（0-1范围）
                        float normalizedX = pixelX / photoWidth;
                        float normalizedY = 1 - pixelY / photoHeight;
                        normalizedPoints.Add(new Vector2(normalizedX, normalizedY));
                        line.Append(" ");
                        line.Append(normalizedX);
                        line.Append(" ");
                        line.Append(normalizedY);
                        log.AppendLine($"  像素坐标: ({pixelX:F2}, {pixelY:F2})");
                        log.AppendLine($"  归一化坐标: ({normalizedX:F4}, {normalizedY:F4})");
                        log.AppendLine($"  视口坐标: ({viewportPos.x:F4}, {viewportPos.y:F4}, {viewportPos.z:F2})");
                        log.AppendLine();
                    
                        visibleCount++;
                        pointCount++;
                    }
                }

                if (pointCount > 0)
                {
                    data.AppendLine(line.ToString());
                }

                log.AppendLine($"可见目标物体数量: {visibleCount}/{targetObjects.Count}");
                log.AppendLine();
            }
            File.WriteAllText(logFilePath, log.ToString());
            File.WriteAllText(dataFilePath, data.ToString());

            Debug.Log($"坐标信息已保存: {logFilePath}");
            return normalizedPoints;
        }
        
        /// <summary>
        /// 设置照片尺寸
        /// </summary>
        public void SetPhotoSize(int width, int height)
        {
            photoWidth = Mathf.Max(1, width);
            photoHeight = Mathf.Max(1, height);
            Debug.Log($"照片尺寸已设置为: {photoWidth}x{photoHeight}");
        }

        public void GeneratePlotImage(List<Vector2> normalizedPoints, Texture2D texture)
        {
            if (texture && generatePlotImg)
            {
                PhotoUtil.PlotAndSavePoints(normalizedPoints.ToArray(), texture, plotPath);
                Debug.Log("Successfully saved plot image");
            }
        }
        
        /// <summary>
        /// 在编辑器中可视化调试（显示视野检测）
        /// </summary>
        void OnDrawGizmosSelected()
        {
            if (targetCamera == null) return;
        }

        public TextureFormat GetTextureFormat()
        {
            switch (imgFormat)
            {
                case ImageFormat.JPG:
                    return TextureFormat.ARGB32;
                case ImageFormat.PNG:
                    return TextureFormat.RGB24;
            }

            return TextureFormat.ARGB32;
        }

        public string GetImageSuffix()
        {
            switch (imgFormat)
            {
                case ImageFormat.JPG:
                    return ".jpg";
                case ImageFormat.PNG:
                    return ".png";
            }

            return ".jpg";
        }

        public byte[] GetEncodedBytes(Texture2D photo)
        {
            switch (imgFormat)
            {
                case ImageFormat.JPG:
                    return photo.EncodeToJPG();
                case ImageFormat.PNG:
                    return photo.EncodeToPNG();
            }

            return null;
        }
    }
}