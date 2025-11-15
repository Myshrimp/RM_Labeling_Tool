using CustomInput;
using Scene;
using ScreenShot;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Flower
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class MyGameEntry : MonoBehaviour
    {
        public static CustomSceneComponent CustomScene;
        public static InputComponent Input;
        public static ScreenShotComponent ScreenShot;
        private static void InitCustomComponents()
        {
            CustomScene = UnityGameFramework.Runtime.GameEntry.GetComponent<CustomSceneComponent>();
            Input = UnityGameFramework.Runtime.GameEntry.GetComponent<InputComponent>();
            ScreenShot = UnityGameFramework.Runtime.GameEntry.GetComponent<ScreenShotComponent>();
        }
    }
}
