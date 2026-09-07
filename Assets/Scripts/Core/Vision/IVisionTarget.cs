using UnityEngine;

namespace FireLine.Scripts.Core.Vision
{
    public interface IVisionTarget
    {
        Transform VisionTransform { get; }
        void SetVisionVisible(bool visible);
    }
}