using Unity.Cinemachine;
using UnityEngine;

[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")]
public class LockCameraAxes : CinemachineExtension
{
    public float lockedXPosition = 0;
    public float lockedZPosition = -10;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        // Usamos 'Finalize' para asegurar que se aplique después de todos los cálculos internos
        if (stage == CinemachineCore.Stage.Finalize)
        {
            var pos = state.RawPosition;
            pos.x = lockedXPosition;
            pos.z = lockedZPosition;
            state.RawPosition = pos;
        }
    }
}


