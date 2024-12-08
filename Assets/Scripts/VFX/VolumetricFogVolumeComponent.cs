using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable, VolumeComponentMenu("Volumetric Fog")]
public class VolumetricFogVolumeComponent : VolumeComponent
{
    public IntParameter stepCount = new IntParameter(50);
    public FloatParameter fogDensity = new FloatParameter(10f);
    public ColorParameter scatteringCoefficients = new ColorParameter(Color.white);
    public ColorParameter absorptionCoefficients = new ColorParameter(Color.clear);
    public FloatParameter lightScale = new FloatParameter(1f);
}
