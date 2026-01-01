using System;
using System.Collections.Generic;
using System.Drawing;
using Label;
using UnityEngine;

namespace Robo.Data
{
    [Serializable]
    public struct Box
    {
        public Vector3 center;
        public Vector3 normal;
        public Vector3 size;
    }
    
    /// <summary>
    /// Critical points data including target box and target points, all calculated in world-space
    /// </summary>
    public struct RawPointData
    {
        public RawPointData(CriticalPoints cp)
        {
            theClass = cp.Color * 3 + cp.Tag;
            box = cp.Box;
            List<Transform> cps = cp.Points;
            points = new Vector3[cps.Count];
            directions = new Vector3[cps.Count];
            int index = 0;
            foreach (var t in cps)
            {
                points[index] = t.position;
                directions[index] = t.forward;
                index += 1;
            }
        }
        
        public int theClass;
        public Box box;
        public Vector3[] points;
        public Vector3[] directions;
    }

    public struct PointData
    {
        public PointData(int cls)
        {
            theClass = cls;
            points = new List<Vector3>();
        }
        public int theClass;
        public List<Vector3> points;
    }
}