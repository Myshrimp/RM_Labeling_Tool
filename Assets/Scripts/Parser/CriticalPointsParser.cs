using System.Collections.Generic;
using UnityEngine;
using Label;
using Robo.Cam;
using Robo.Data;

namespace Robo.Parser
{
    public class CriticalPointsParser
    {
        private Camera _cam;

        public CriticalPointsParser(Camera mainCam)
        {
            _cam = mainCam;
        }

        private List<RawPointData> CollectPoints(List<CriticalPoints> criticalPoints)
        {
            List<RawPointData> result = new List<RawPointData>();
            foreach (var cp in criticalPoints)
            {
                RawPointData data = new RawPointData(cp);
                result.Add(data);
            }

            return result;
        }
        
        public List<PointData> Parse(List<CriticalPoints> criticalPoints)
        {
            List<RawPointData> rawPointDatas = CollectPoints(criticalPoints);
            List<PointData> pointDatas = new List<PointData>();
            Vector3 camDir = _cam.transform.forward;
            foreach (var pd in rawPointDatas)
            {
                PointData newPd = new PointData(pd.theClass);
                for (int i = 0; i < pd.points.Length; i++)
                {
                    Vector3 position = pd.points[i];
                    Vector3 direction = pd.directions[i];
                    if (Vector3.Dot(direction, camDir) < 0)
                    {
                        newPd.points.Add(position);
                    }
                }
                pointDatas.Add(newPd);
            }

            return pointDatas;
        }
    }
}