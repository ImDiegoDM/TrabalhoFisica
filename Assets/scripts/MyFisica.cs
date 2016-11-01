using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyFisica : MonoBehaviour {
	public float massa;
	public float gravidade;
	public float ResistenciaAr;
	public float angularDrag;

	Collider collider;
	List<Vector3> forces;
	Vector3 gravityForce;
	Vector3 aceleration;
	Vector3 angularAceleration;
	Vector3 arForce;
	Vector3 inercia;
	Vector3 cineticEnergy;

	Vector3 initialPose;
	Vector3 finalPose;

	float velociade=0;
	Vector3 peso;

	bool onGround=false;

	// Use this for initialization
	void Start () {
		forces = new List<Vector3> ();
		gravityForce = Vector3.zero;
		gravityForce.y = -gravidade;
		peso = gravityForce * massa;
	}

	void CalculaInercia()// calcula inercia com base no tipo de colider
	{
		BoxCollider box = GetComponent<BoxCollider> ();
		if (box == null) {
			SphereCollider sphere = GetComponent<SphereCollider> ();
			float i = (2.0f / 3f) * massa * (sphere.radius * sphere.radius);
			inercia = new Vector3 (i,i,i);
			return;
		}
		inercia = new Vector3 ((1/12)*massa*(Mathf.Pow(box.size.y,2)+Mathf.Pow(box.size.z,2)),(1/12)*massa*(Mathf.Pow(box.size.x,2)+Mathf.Pow(box.size.z,2)),(1/12)*massa*(Mathf.Pow(box.size.x,2)+Mathf.Pow(box.size.y,2)));
	}

	public void OnCollisionStay(Collision col)
	{
		onGround = true;
	}

	public void OnCollisionExit(Collision col)
	{
		onGround = false;
	}

	Vector3 SomatorioForces(){ // retorna o somatorio das forças exercidas nele
		if (!onGround) {// se estiver no chão não calcula gravidade
			forces.Add (peso);
		}
		Vector3 resultante=Vector3.zero;
		for (int i = 0; i < forces.Count; i++) {
			resultante += forces [i];
		}
		forces.Clear ();
		return (resultante) / massa;
	}

	public void AddForce(Vector3 f) // adiciona uma força 
	{
		forces.Add (f);
	}

	void AngularAceleration(Vector3 torque)// calcula a aceleração angular com base no torque
	{
		angularAceleration = new Vector3(torque.x / inercia.x, torque.y / inercia.y, torque.z / inercia.z);
		angularAceleration -= angularAceleration * angularDrag * Time.deltaTime;
	}

	Vector3 ElevaVetor(Vector3 vetor, float indice)// metodo para elevar um vetor a um numero
	{
		return new Vector3 (Mathf.Pow (vetor.x, indice), Mathf.Pow (vetor.y, indice), Mathf.Pow (vetor.z, indice));
	}

	// Update is called once per frame
	void Update () {
		velociade = aceleration.magnitude;

		if (velociade == 0) {
			initialPose = this.transform.position;
		}

		cineticEnergy = 0.5f * massa * ElevaVetor(aceleration,2);
		aceleration =SomatorioForces ()*Time.deltaTime;

		this.transform.position += aceleration;
		transform.rotation *= Quaternion.Euler (angularAceleration);
	}
}
