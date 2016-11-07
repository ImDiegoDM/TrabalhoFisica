using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour {

	static Settings gInstance;

	public Vector3 gravityAcceleration = Vector3.zero;

	public float airResistence = 0.0f;

	void Start()
	{
		gInstance = this;
	}

	public static Settings getInstance()
	{
		return gInstance;
	}
}
