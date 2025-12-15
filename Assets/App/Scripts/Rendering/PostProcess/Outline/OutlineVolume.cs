using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Outline
{
    [Serializable]
    [VolumeComponentMenu("Outline")]
    public class SelectableOutlineVolume : VolumeComponent
    {
        public ColorParameter OutlineColor = new(Color.red);
        public FloatParameter OutlineWidth = new(1f);
    }
}