using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyFisica : MonoBehaviour {
	public float massa;
	public float gravidade;
	public float ResistenciaAr;
	public float angularDrag;
	public float drag;

	Collider collider;
	Vector3 gravityForce;
	Vector3 angularVelocity=Vector3.zero;
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
	void Awake () {
		gravityForce = Vector3.zero;
		gravityForce.y = -gravidade;
		peso = gravityForce * massa;
		forcesAplied = new List<Vector3> ();
		CalculaInercia ();
	}

	void CalculaInercia()// calcula inercia com base no tipo de colider
	{
		BoxCollider box = GetComponent<BoxCollider> ();
		inercia = new Vector3 ((1f/12f)*massa*(Mathf.Pow(box.size.y,2f)+Mathf.Pow(box.size.z,2f)),(1f/12f)*massa*(Mathf.Pow(box.size.x,2f)+Mathf.Pow(box.size.z,2f)),(1f/12f)*massa*(Mathf.Pow(box.size.x,2f)+Mathf.Pow(box.size.y,2f)));

	}

	public void ApplyTorqueByPointForce(Vector3 point, Vector3 force)
	{
		Vector3 torque = Vector3.Cross (point - transform.position, force);

		angularVelocity += CalculateAngularAcceleration (torque);
	}

	Vector3 CalculateAngularAcceleration(Vector3 torque)
	{
		//from tangent acceleration to angular acceleration
		Vector3 angularAceleration = torque;

		//Calculating torque based in it's inertia
		angularAceleration.x /= inercia.x;
		angularAceleration.y /= inercia.y;
		angularAceleration.z /= inercia.z;

		return angularAceleration;
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

	public Vector3 getVelocity(){
		return velocity;
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

		//print (forcaResultante);

		return (forcaResultante) / massa;
	}

	public void AddForce(Vector3 f) // adiciona uma força 
	{
		forcaResultante += f;
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

		transform.Rotate(angularVelocity * Time.deltaTime,Space.World);

		//Update torque
		angularVelocity -= angularVelocity * angularDrag * Time.deltaTime;

		if (angularVelocity.magnitude < 0.1f)
			angularVelocity = Vector3.zero;

		Vector3 aceleration =SomatorioForces ()*Time.deltaTime;

		velocity += aceleration;

		velocity -= velocity * drag * Time.deltaTime;


		this.transform.position += velocity*Time.deltaTime;

		forcaResultante = Vector3.zero;
	}
}
