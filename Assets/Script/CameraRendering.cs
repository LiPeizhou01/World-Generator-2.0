using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRendering : MonoBehaviour
{
    /*
    public Mesh blockMesh;
    public int subMeshIndex = 0;
    public Material blockMaterial;
    public DepthMapGenerator depthTextureGenerator;
    public ComputeShader compute;

    int m_blockCount;
    int kernel;
    Camera mainCamera;

    ComputeBuffer argsBuffer;
    ComputeBuffer blockMatrixBuffer;
    ComputeBuffer cullResultBuffer;

    uint[] args = new uint[5] {0,0,0,0,0};
    int cullResultBufferId, vpMatrixId, positionBufferId, hizTextureId;

    void Start()
    {
        m_blockCount = MapLoader.Instance.GetCurrentBlockCount();
        Debug.Log($" m_blockCount:{m_blockCount}");
        mainCamera = Camera.main;

        if(blockMesh != null)
        {
            args[0] = blockMesh.GetIndexCount(subMeshIndex);
            args[2] = blockMesh.GetIndexStart(subMeshIndex);
            args[3] = blockMesh.GetBaseVertex(subMeshIndex);
        }

        InitComputeBuffer();
        
        blockMatrixBuffer.SetData(MapLoader.Instance.GetLoadingBlockMatrixs());
        InitComputeShader();

    }

    void InitComputeShader()
    {
        kernel = compute.FindKernel("blockCulling");
        compute.SetInt("blockCount", m_blockCount);
        compute.SetInt("depthTextureSize", depthTextureGenerator.depthTextureSize);
        compute.SetBool("isOpenGL", Camera.main.projectionMatrix.Equals(GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false)));
        compute.SetBuffer(kernel, "blockMatrixBuffer", blockMatrixBuffer);

        cullResultBufferId = Shader.PropertyToID("cullResultBuffer");
        vpMatrixId = Shader.PropertyToID("vpMatrix");
        hizTextureId = Shader.PropertyToID("hizTexture");
        positionBufferId = Shader.PropertyToID("positionBuffer");
    }

    void InitComputeBuffer()
    {
        if (blockMatrixBuffer != null)
        {
            return;
        }
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);
        blockMatrixBuffer = new ComputeBuffer(m_blockCount, sizeof(float) * 16);
        cullResultBuffer = new ComputeBuffer(m_blockCount, sizeof(float) * 16, ComputeBufferType.Append);
    }
    void Update()
    {
        compute.SetTexture(kernel, hizTextureId, depthTextureGenerator.depthTexture);
        compute.SetMatrix(vpMatrixId, GL.GetGPUProjectionMatrix(mainCamera.projectionMatrix, false) * mainCamera.worldToCameraMatrix);
        cullResultBuffer.SetCounterValue(0);
        compute.SetBuffer(kernel, cullResultBufferId, cullResultBuffer);
        //或许需要修改
        compute.Dispatch(kernel, 1 + m_blockCount / 640, 1, 1);
        blockMaterial.SetBuffer(positionBufferId, cullResultBuffer);

        //获取实际要渲染的数量
        ComputeBuffer.CopyCount(cullResultBuffer, argsBuffer, sizeof(uint));
        Debug.Log($"cullResult:{cullResultBuffer.count}");
        Graphics.DrawMeshInstancedIndirect(blockMesh, subMeshIndex, blockMaterial, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), argsBuffer);
    }

    void OnDisable()
    {
        blockMatrixBuffer?.Release();
        blockMatrixBuffer = null;

        cullResultBuffer?.Release();
        cullResultBuffer = null;

        argsBuffer?.Release();
        argsBuffer = null;
    }
    */
}
