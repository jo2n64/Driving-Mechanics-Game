using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCarDamage : MonoBehaviour
{
	[SerializeField] private float maxMoveDelta = 1.0f;
	[SerializeField] private float maxCollisionStrength = 50.0f;
	[SerializeField] private float yForceDamp = 0.1f;
	[SerializeField] private float demulitionRange = 0.5f;
	[SerializeField] private float impactDirManipulator = 0.0f;
	[SerializeField] private MeshFilter[] meshList;
	[SerializeField] private LayerMask collisionToIgnore;

	private MeshFilter[] meshFilters;
	private float sqrDemRange;

	private struct PermaVertsColl
	{
		public Vector3[] permaVerts;
	}
	private PermaVertsColl[] originalMeshData;
	int i;

	private void Start()
	{
		if (meshList.Length > 0)
		{
			meshFilters = meshList;
		}
		else
		{
			meshFilters = GetComponentsInChildren<MeshFilter>();
		}
		sqrDemRange = demulitionRange * demulitionRange;
		LoadOriginalMeshData();
	}


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R)) Repair();
	}

	private void LoadOriginalMeshData()
	{
		originalMeshData = new PermaVertsColl[meshFilters.Length];
		for (int i = 0; i < meshFilters.Length; i++)
		{
			originalMeshData[i].permaVerts = meshFilters[i].mesh.vertices;
		}
	}

	private void Repair()
	{
		Debug.Log("cock repaired");
		for (int i = 0; i < meshFilters.Length; i++)
		{
			meshFilters[i].mesh.vertices = originalMeshData[i].permaVerts;
			meshFilters[i].mesh.RecalculateNormals();
			meshFilters[i].mesh.RecalculateBounds();
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (IsInLayerMask(other.gameObject, collisionToIgnore))
		{
			Debug.Log($"holy moly we collided{other.gameObject.name}");
			Vector3 colRelVel = other.relativeVelocity;
			colRelVel.y *= yForceDamp;
			Vector3 colPointToMe = transform.position - other.contacts[0].point;
			float colStrength = colRelVel.magnitude * Vector3.Dot(other.contacts[0].normal, colPointToMe.normalized);
			OnMeshForce(other.contacts[0].point, Mathf.Clamp01(colStrength / maxCollisionStrength));
		}
	}

	private void OnMeshForce(Vector4 originPosAndForce)
	{
		OnMeshForce((Vector3)originPosAndForce, originPosAndForce.w);
	}

	private void OnMeshForce(Vector3 originPos, float force)
	{

		force = Mathf.Clamp01(force);
		for (int j = 0; j < meshFilters.Length; ++j)
		{
			Vector3[] verts = meshFilters[j].mesh.vertices;
			for (int i = 0; i < verts.Length; ++i)
			{
				Vector3 scaledVert = Vector3.Scale(verts[i], transform.localScale);
				Vector3 vertWorldPos = meshFilters[j].transform.position + (meshFilters[j].transform.rotation * scaledVert);
				Vector3 originToMeDir = vertWorldPos - originPos;
				Vector3 flatVertToCenterDir = transform.position - vertWorldPos;
				flatVertToCenterDir.y = 0.0f;
				if (originToMeDir.sqrMagnitude < sqrDemRange)
				{
					float dist = Mathf.Clamp01(originToMeDir.sqrMagnitude / sqrDemRange);
					float moveDelta = force * (1.0f - dist) * maxMoveDelta;
					Vector3 moveDir = Vector3.Slerp(originToMeDir, flatVertToCenterDir, impactDirManipulator).normalized * moveDelta;
					verts[i] += Quaternion.Inverse(transform.rotation) * moveDir;

				}
			}
			meshFilters[j].mesh.vertices = verts;
			meshFilters[j].mesh.RecalculateBounds();

		}
	}


	public bool IsInLayerMask(GameObject obj, LayerMask layer)
	{
		return ((layer.value & (1 << obj.layer)) > 0);
	}
}
