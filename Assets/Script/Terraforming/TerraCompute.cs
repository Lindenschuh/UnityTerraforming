using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerraCompute
{
    private ComputeShader CShader;
    private int kernel;

    public TerraCompute(ComputeShader CS)
    {
        CShader = CS;
        kernel = CShader.FindKernel("CSMain");
    }

    public float[,] CalculateWihtShader(float[,] data, float value)
    {
        ComputeBuffer CBuffer = new ComputeBuffer(data.GetLength(0) * data.GetLength(1), sizeof(float));
        CShader.SetBuffer(kernel, "Result", CBuffer);
        CShader.SetFloat("Value", value);
        CShader.SetInt("BufferSize", data.GetLength(1));
        CBuffer.SetData(data);
        CShader.Dispatch(kernel, data.GetLength(0) / 10, data.GetLength(1) / 10, 1);

        CBuffer.GetData(data);
        CBuffer.Dispose();

        return data;
    }
}