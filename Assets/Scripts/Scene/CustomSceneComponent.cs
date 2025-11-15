using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;

namespace Scene
{
    public class CustomSceneComponent : GameFrameworkComponent
    {
        [SerializeField] private HashSO _config;

        public void LoadScene(string key, LoadSceneMode loadMode)
        {
            SceneManager.LoadScene(_config.Get(key), loadMode);
        }

        public void UnloadScene(string key)
        {
            SceneManager.UnloadSceneAsync(_config.Get(key));
        }

        protected override void Awake()
        {
            base.Awake();
            _config.Init();
        }
    }
}