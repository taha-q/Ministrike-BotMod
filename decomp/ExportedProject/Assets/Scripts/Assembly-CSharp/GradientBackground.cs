using UnityEngine;

public class GradientBackground : MonoBehaviour
{
	public Shader shader;

	public Color topColor = Color.blue;

	public Color bottomColor = Color.white;

	public int gradientLayer = 7;

	private Camera cam;

	private void Awake()
	{
		gradientLayer = Mathf.Clamp(gradientLayer, 0, 31);
		cam = GetComponent<Camera>();
		if (!cam)
		{
			Debug.LogError("Must attach GradientBackground script to the camera");
			return;
		}
		cam.clearFlags = CameraClearFlags.Depth;
		cam.cullingMask &= ~(1 << gradientLayer);
		Camera component = new GameObject("Gradient Cam", typeof(Camera)).GetComponent<Camera>();
		component.renderingPath = RenderingPath.VertexLit;
		component.depth = cam.depth - 1f;
		component.cullingMask = 1 << gradientLayer;
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[4]
		{
			new Vector3(-100f, 0.577f, 1f),
			new Vector3(100f, 0.577f, 1f),
			new Vector3(-100f, -0.577f, 1f),
			new Vector3(100f, -0.577f, 1f)
		};
		mesh.colors = new Color[4] { topColor, topColor, bottomColor, bottomColor };
		mesh.triangles = new int[6] { 0, 1, 2, 1, 3, 2 };
		Material material = new Material(shader);
		GameObject gameObject = new GameObject("Gradient Plane", typeof(MeshFilter), typeof(MeshRenderer));
		((MeshFilter)gameObject.GetComponent(typeof(MeshFilter))).mesh = mesh;
		gameObject.GetComponent<Renderer>().material = material;
		gameObject.layer = gradientLayer;
	}
}
