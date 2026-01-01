using System.Collections.Generic;
using Robo.Core.Math;
using Robo.Data;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Label
{
    public class CriticalPoints : MonoBehaviour
    {
        [Tooltip("0 for blue, 1 for red")]
        [Range(0,1)]
        [SerializeField] private int _color;
        [SerializeField] private bool _isRTag;
        [SerializeField] private Box _box;
        private int _tag=1;
        private List<Transform> points;

#if UNITY_EDITOR
        public bool hardLockBoxCenterOnParent;
        public bool showGizmo;
#endif

        public int Color
        {
            get { return _color; }
            private set{}
        }

        public int Tag
        {
            get
            {
                if (_isRTag) return 0;
                return _tag;
            }
            set
            {
                if (_isRTag)
                {
                    _tag = 0;
                    return; 
                }
                _tag = value;
            }
        }
        public List<Transform> Points
        {
            get { return points; }
            private set {}
        }

        public Box Box
        {
            get { return _box; }
            private set{}
        }

        /// <summary>
        /// 让检测框的朝向与父节点物体一致
        /// </summary>
        public void SetBoxNormalEqualToParent()
        {
            _box.normal = transform.forward;
        }

        public void SetBoxCenterOnParent()
        {
            _box.center = transform.position;
        }
        private void Awake()
        {
            points = new List<Transform>();
            int index = 0;
            foreach (Transform child in transform)
            {
                points.Add(child);
            }
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (!showGizmo)
                return;
            if(hardLockBoxCenterOnParent) 
                _box.center = transform.position;
#endif
            Vector3[] vertices = Math.CalculatePlaneVertices(_box.normal, _box.center, _box.size);

            for(int i=1;i<4;i++)
            {
                Gizmos.DrawLine(vertices[i-1], vertices[i]);
            }
            Gizmos.DrawLine(vertices[3], vertices[0]);
        }
    }
}