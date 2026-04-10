using UnityEngine;
using Cinemachine;

[SaveDuringPlay]
[AddComponentMenu("")]
public class LockCameraY : CinemachineExtension
{
    [Tooltip("Ниже этой отметки камера не пойдет")]
    public float m_YLimit = 0;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            var pos = state.RawPosition;
            if (pos.y < m_YLimit) pos.y = m_YLimit;
            state.RawPosition = pos;
        }
    }
}