using UnityEngine;
using System.Collections;
[RequireComponent(typeof(MyFisica))]
public class AirPlane : MonoBehaviour {
	MyFisica fisica;
	ProfessorRigidBody f;
	float forceAr;// força do ar com base na altitude
	float altitude;
	public float MotorForce; // força maxima que o motor alcança
	float motorFactor; // força atula do motor varia de 0 a 1;


	// Use this for initialization
	void Awake () {
		fisica = this.GetComponent<MyFisica> ();
	}

	void setArForce(){ // seta a força do ar com base na altitude
		forceAr=1-(altitude/1500);
	}

	void CalculaAltitude() 
	{
		altitude = this.transform.position.y*10;
	}



	void setForces()// calcula as forças exercidar no avião e seta elas na fisica
	{
		CalculaAltitude ();
		setArForce ();
		Vector3 force = Vector3.forward;
		force *= (MotorForce * motorFactor);// calcula a força com base no motor e o qual aquele motor está sendo usado
		fisica.AddForce(force);
		Vector3 arFoce = Vector3.up;
		arFoce *= ((MotorForce * motorFactor) * forceAr);
		fisica.AddForce (arFoce);
	}

	void Acelera()
	{
		if(motorFactor<1f)
		{
			motorFactor += 0.3f * Time.deltaTime;
		}
		setForces ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.UpArrow)) {
			Acelera ();
		}
	
	}
}
