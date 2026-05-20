using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[ExecuteAlways]
public class SplineFollower : MonoBehaviour
{
    public SplineContainer splineContainer;

    [Range(0f, 1f)]
    public float t = 0f;

    public bool faceDirection = true;
    public bool updateInEditMode = true;

    void Update()
    {
        if (!Application.isPlaying && !updateInEditMode)
            return;

        if (splineContainer == null)
            return;

        // NOTE: float3 version (Unity Splines new API)
        float3 position;
        float3 tangent;
        float3 up;

        splineContainer.Evaluate(t, out position, out tangent, out up);

        // Convert float3 -> Vector3 for transform
        transform.position = splineContainer.transform.TransformPoint((Vector3)position);

        if (faceDirection && !math.all(tangent == 0))
        {
            Vector3 worldTangent = splineContainer.transform.TransformDirection((Vector3)tangent);
            Vector3 worldUp = splineContainer.transform.TransformDirection((Vector3)up);

            transform.rotation = Quaternion.LookRotation(worldTangent, worldUp);
        }
    }
}