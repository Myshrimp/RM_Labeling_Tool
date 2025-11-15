using UnityEngine;
using System.Collections;
using System.IO;
using Flower;
using UnityGameFramework.Runtime;

namespace ScreenShot
{
    public class ScreenShotComponent : GameFrameworkComponent
    {
    [Header("相机引用（用于方法3）")]
    public Camera targetCamera; // 指定要截图的相机，如果为空则默认为主相机

    [Header("截图设置")]
    public string screenshotFolder = "Screenshots"; // 保存截图的文件夹名

    void Update()
    {
        // 按下指定按键触发截图
        if (MyGameEntry.Input.GetBool("ScreenShot"))
        {
            // 取消下面任一方法的注释来使用它

            // 方法1: 全屏截图
            // StartCoroutine(CaptureFullScreen());

            // 方法2: 自定义区域截图 (示例:截取中间400x300区域)
            // StartCoroutine(CaptureCustomArea(new Rect(Screen.width/2 - 200, Screen.height/2 - 150, 400, 300)));

            // 方法3 (推荐): 指定相机截图
            StartCoroutine(CaptureCameraView());
        }
    }

    // 方法1: 全屏截图[citation:1][citation:2]
    IEnumerator CaptureFullScreen()
    {
        // 确保在渲染一帧完成后执行
        yield return new WaitForEndOfFrame();

        // 创建保存路径
        string directoryPath = Path.Combine(Application.persistentDataPath, screenshotFolder);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        string filename = "Screenshot_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string path = Path.Combine(directoryPath, filename);

        // 调用Unity API进行全屏截图
        ScreenCapture.CaptureScreenshot(path);
        Debug.Log("全屏截图已保存至: " + path);
    }

    // 方法2: 自定义区域截图[citation:1][citation:5]
    IEnumerator CaptureCustomArea(Rect captureRect)
    {
        yield return new WaitForEndOfFrame();

        string directoryPath = Path.Combine(Application.persistentDataPath, screenshotFolder);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        // 创建纹理并读取指定区域的像素[citation:1]
        Texture2D screenshotTexture = new Texture2D((int)captureRect.width, (int)captureRect.height, TextureFormat.RGB24, false);
        screenshotTexture.ReadPixels(captureRect, 0, 0);
        screenshotTexture.Apply();

        // 编码为PNG并保存[citation:5]
        byte[] bytes = screenshotTexture.EncodeToPNG();
        string filename = "CustomArea_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string path = Path.Combine(directoryPath, filename);
        File.WriteAllBytes(path, bytes);

        Destroy(screenshotTexture);
        Debug.Log("自定义区域截图已保存至: " + path);
    }

    // 方法3: 指定相机截图[citation:1][citation:4][citation:8]
    IEnumerator CaptureCameraView()
    {
        yield return new WaitForEndOfFrame();

        if (targetCamera == null)
            targetCamera = Camera.main;

        string directoryPath = Path.Combine(Application.streamingAssetsPath, screenshotFolder);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        // 创建渲染纹理 (RenderTexture)[citation:1]
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        // 将相机的输出指向渲染纹理[citation:1][citation:4]
        targetCamera.targetTexture = rt;
        // 手动渲染相机[citation:1][citation:4]
        targetCamera.Render();
        // 设置当前激活的渲染纹理[citation:1]
        RenderTexture.active = rt;

        // 从激活的渲染纹理中读取像素[citation:1]
        Texture2D screenshotTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshotTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshotTexture.Apply();

        // 重置相机和渲染纹理设置[citation:1]
        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // 保存图片
        byte[] bytes = screenshotTexture.EncodeToPNG();
        string filename = "CameraView_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string path = Path.Combine(directoryPath, filename);
        File.WriteAllBytes(path, bytes);

        Destroy(screenshotTexture);
        Debug.Log("相机视图截图已保存至: " + path);
    }
    }
}