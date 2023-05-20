using UnityEngine;

public static class CameraExtensions
{
    /// <summary>
    /// Called to test a renderer if its visible based on the camera frustum.
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="testVisibleRenderer"></param>
    /// <returns></returns>
    public static bool IsRendererVisible(this Camera camera, Renderer testVisibleRenderer)
    {
        if (testVisibleRenderer == null)
            return false;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, testVisibleRenderer.bounds);
    }

}