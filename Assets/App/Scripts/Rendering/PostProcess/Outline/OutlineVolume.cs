using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Outline
{
    [Serializable]
    [VolumeComponentMenu("Outline")]
    public class OutlineVolume : VolumeComponent
    {
        public ColorParameter OutlineColor = new(Color.white);
        public FloatParameter OutlineWidth = new(1f);
    }
}