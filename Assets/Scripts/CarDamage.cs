using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDamage : MonoBehaviour
{
	[SerializeField] private float deformRadius = 0.2f;
	[SerializeField] private float maxDeform = 0.1f;
	[SerializeField] private float damageFalloff = 1f;
	[SerializeField] private float damageMultiplier = 1f;
	[SerializeField] private float minDamage = 1f;

	[SerializeField] private MeshFilter filter;
	[SerializeField] private Rigidbody rb;
	[SerializeField] private MeshCollider col;
	[SerializeField] private Vector3[] startingVertices, meshVertices;
	// Start is called before the first frame update

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		col = GetComponent<MeshCollider>();
		startingVertices = filter.mesh.vertices;
		meshVertices = filter.mesh.vertices;
	}

	private void OnCollisionEnter(Collision other)
	{
		float colPower = other.impulse.magnitude;
		if (colPower > minDamage)
		{
			foreach (ContactPoint point in other.contacts)
			{
				for (int i = 0; i < meshVertices.Length; i++)
				{
					Vector3 vertexPos = meshVertices[i];
					Vector3 pointPos = transform.InverseTransformPoint(point.point);
					float distanceFromCollision = Vector3.Distance(vertexPos, pointPos);
					float distanceFromOriginal = Vector3.Distance(startingVertices[i], vertexPos);
					if (distanceFromCollision < deformRadius && distanceFromOriginal < maxDeform)
					{
						float falloff = 1 - (distanceFromCollision / deformRadius) * damageFalloff;

						float xDeform = pointPos.x * falloff;
						float yDeform = pointPos.y * falloff;
						float zDeform = pointPos.z * falloff;

						xDeform = Mathf.Clamp(xDeform, 0, maxDeform);
						yDeform = Mathf.Clamp(yDeform, 0, maxDeform);
						zDeform = Mathf.Clamp(zDeform, 0, maxDeform);

						Vector3 deform = new Vector3(xDeform, yDeform, zDeform);
						meshVertices[i] -= deform * damageMultiplier;
					}
				}
			}
		}
		UpdateMeshVertices();
	}

	private void UpdateMeshVertices()
	{
		filter.mesh.vertices = meshVertices;
		col.sharedMesh = filter.mesh;
	}
}
