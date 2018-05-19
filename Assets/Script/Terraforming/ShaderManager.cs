using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EasingFunctions
{
    Lerp,
    Cubic,
    Mean,
    Quint
}

public class ShaderManager : MonoBehaviour
{
    public ComputeShader CShader;
    private int LerpKernel;
    private int CubicKernel;
    private int MeanKernel;
    private int QuintKernel;

    private void Start()
    {
        LerpKernel = CShader.FindKernel("CSLerp");
        CubicKernel = CShader.FindKernel("CSCubic");
        MeanKernel = CShader.FindKernel("CSMean");
        QuintKernel = CShader.FindKernel("CSQuint");
    }

    private int ResolveKernel(EasingFunctions eF)
    {
        switch (eF)
        {
            case EasingFunctions.Lerp:
                return LerpKernel;

            case EasingFunctions.Cubic:
                return CubicKernel;

            case EasingFunctions.Mean:
                return MeanKernel;

            case EasingFunctions.Quint:
                return QuintKernel;

            default:
                return -1;
        }
    }

    public float[,] CalculateWihtShader(float[,] data, float value, EasingFunctions ef)
    {
        ComputeBuffer CBuffer = new ComputeBuffer(data.GetLength(0) * data.GetLength(1), sizeof(float));
        int kernel = ResolveKernel(ef);
        CShader.SetBuffer(kernel, "Result", CBuffer);
        CShader.SetFloat("Value", value);
        CShader.SetFloats("BrushSize", new float[] { data.GetLength(0), data.GetLength(1) });

        CBuffer.SetData(data);
        CShader.Dispatch(kernel, Mathf.CeilToInt(data.GetLength(0) / 5), Mathf.CeilToInt(data.GetLength(1) / 5), 1);

        CBuffer.GetData(data);
        CBuffer.Dispose();

        return data;
    }
}