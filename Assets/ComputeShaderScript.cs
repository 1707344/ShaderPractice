using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ComputeShaderScript : MonoBehaviour
{
    public ComputeShader computeShader;

    public Texture2D skyboxTexture;
    private RenderTexture _target;
    public Camera _camera;

    // Start is called before the first frame update
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetShaderParameters();

        InitRenderTexture();

        computeShader.SetTexture(0, "Result", _target);
        
        //Starting the threads
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        computeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        Graphics.Blit(_target, destination);

    }

    private void SetShaderParameters()
    {
        computeShader.SetTexture(0, "_SkyboxTexture", skyboxTexture);
        computeShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        computeShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
        computeShader.SetFloat("_Time", Time.time);
    }

    private void InitRenderTexture()
    {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height)
        {
            // Release render texture if we already have one
            if (_target != null)
                _target.Release();

            // Get a render target for Ray Tracing
            _target = new RenderTexture(Screen.width, Screen.height, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;
            _target.Create();
        }
    }
}
