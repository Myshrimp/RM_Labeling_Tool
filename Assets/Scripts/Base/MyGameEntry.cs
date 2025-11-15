using UnityEngine;

namespace Flower
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class MyGameEntry : MonoBehaviour
    {
        private void Start()
        {
            InitBuiltinComponents();
            InitCustomComponents();
        }
    }
}
