using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyFisica : MonoBehaviour {
	public float massa;
	public float gravidade;
	public float ResistenciaAr;
	public float angularDrag;

	Collider collider;
	Vector3 gravityForce;
	Vector3 angularAceleration;
	Vector3 arForce;
	Vector3 inercia;
	Vector3 cineticEnergy;
	Vector3 velocity= Vector3.zero;
	Vector3 forcaResultante= Vector3.zero;

	List<Vector3> forcesAplied;

	float time=0;

	Vector3 initialPose;
	Vector3 finalPose;

	float velociade=0;
	Vector3 peso;

	public bool onGround=false;

	// Use this for initialization
	void Start () {
		gravityForce = Vector3.zero;
		gravityForce.y = -gravidade;
		peso = gravityForce * massa;
		forcesAplied = new List<Vector3> ();
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
	public void OnCollisionEnter(Collision collision)
	{
		Vector3 normalVector=Vector3.zero;
		//forces.Add (new Vector3(0,aceleration.x*-1,0));
		for (int i = 0; i < collision.contacts.Length; i++) {
			normalVector += collision.contacts [i].normal;
		}

		normalVector.Normalize ();

		Vector3 resultante = (massa * velocity)+(peso);
		Vector3 normalforce = (normalVector * resultante.magnitude);
		Vector3 friction;

		friction = velocity.magnitude*normalVector* Vector3.Dot (-normalVector, velocity.normalized);
		velocity += friction;
		//velocity.y = 0;
		onGround = true;
	}

	public void OnCollisionStay(Collision collision)
	{
		Vector3 normalVector=Vector3.zero;
		//forces.Add (new Vector3(0,aceleration.x*-1,0));
		for (int i = 0; i < collision.contacts.Length; i++) {
			normalVector += collision.contacts [i].normal;
		}
		/*
		Vector3 forceApplied = (massa * velocity /time ) + (massa * peso);

		Vector3 normalForce = normalVector.normalized * forceApplied.magnitude * Vector3.Dot (-normalVector, forceApplied.normalized);


		Vector3 forceAppliedWithEnergyTransfer = (massa * velocity /time) + (massa * peso);

		//Calculating bouciness
		Vector3 boucinessForce = (normalVector) * 0 * forceAppliedWithEnergyTransfer.magnitude;
		Vector3 aceleration = (normalForce+boucinessForce) / massa;

		velocity += aceleration*time;
		print (forceApplied);

		onGround = true;*/
		normalVector.Normalize ();

		Vector3 resultante = (massa * velocity)+(peso);
		Vector3 normalforce = (normalVector * resultante.magnitude);
		Vector3 friction;

		friction = velocity.magnitude*normalVector* Vector3.Dot (-normalVector, velocity.normalized);
		velocity += friction;
		//velocity.y = 0;
		onGround = true;
	}

	public void OnCollisionExit(Collision col)
	{
		onGround = false;
	}

	Vector3 SomatorioForces(){ // retorna o somatorio das forças exercidas nele
		if (!onGround) {
			forcaResultante += peso;
		}

		print (forcaResultante);

		return (forcaResultante) / massa;
	}

	public void AddForce(Vector3 f) // adiciona uma força 
	{
		forcaResultante += f;
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
	void LateUpdate () {
		time += Time.deltaTime;
		//velociade = aceleration.magnitude;

		if (velociade == 0) {
			initialPose = this.transform.position;
		}
		Vector3 aceleration =SomatorioForces ()*Time.deltaTime;

		velocity += aceleration;


		this.transform.position += velocity*Time.deltaTime;

		transform.rotation *= Quaternion.Euler (angularAceleration);
		forcaResultante = Vector3.zero;
	}
}
