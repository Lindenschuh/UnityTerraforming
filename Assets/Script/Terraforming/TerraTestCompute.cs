using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerraTestCompute : MonoBehaviour
{
    public ComputeShader CShader;
    private int kernel;

    // Use this for initialization
    private void Start()
    {
        kernel = CShader.FindKernel("CSMain");

        float[,] data = new float[50, 50];

        ComputeBuffer CBuffer = new ComputeBuffer((int)data.LongLength, sizeof(float));
        CShader.SetBuffer(kernel, "Result", CBuffer);

        CShader.SetInts("Mid", new[] { data.GetLength(0) / 2, data.GetLength(1) / 2 });
        CShader.SetInt("BufferSize", data.GetLength(0));
        CShader.Dispatch(kernel, data.GetLength(0) / 10, data.GetLength(1) / 10, 1);

        CBuffer.GetData(data);

        foreach (float f in data)
        {
            Debug.Log(f);
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}