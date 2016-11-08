using UnityEngine;
using System.Collections;
[RequireComponent(typeof(MyFisica))]
public class AirPlane : MonoBehaviour {
	MyFisica fisica;
	ProfessorRigidBody f;
	float forceAr;// força do ar com base na altitude
	float altitude;
	public Transform aileronsLeftPosition;
	public Transform aileronsRightPosition;
	public Transform profundoresPosition;
	public Transform lemePosition;

	public float MotorForce; // força maxima que o motor alcança
	float motorFactor; // força atula do motor varia de 0 a 1;
	public float areaDeSuperficie;// area de superfice frontal do avião
	public float coeficienteAeroDinamica;// coeficiente de aeroDinamica
	public float aileronsForce;
	public float profundoresForce;
	public float lemeforce;


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
		setArForce ();// seta a densidade do ar


		Vector3 force = transform.forward;
		force *= (MotorForce * motorFactor);// calcula a força com base no motor e o qual aquele motor está sendo usado
		fisica.AddForce(force);


		Vector3 arFoce = transform.up;
		arFoce *= ((MotorForce * motorFactor) * forceAr) * fisica.getVelocity ().magnitude/fisica.massa;
		fisica.AddForce (arFoce);


		Vector3 point = this.transform.position;
		point.z += 0.1f;
		//fisica.ApplyTorqueByPointForce (point,arFoce);

		float drag = coeficienteAeroDinamica * forceAr / 2 * areaDeSuperficie * Mathf.Pow (fisica.getVelocity().magnitude,2);
		fisica.AddForce (drag*fisica.getVelocity().normalized*-1);
	}

	public void AileronsLeft(){
		fisica.ApplyTorqueByPointForce (aileronsLeftPosition.position,transform.up*aileronsForce);
	}

	public void AileronsRight(){
		fisica.ApplyTorqueByPointForce (aileronsRightPosition.position,transform.up*aileronsForce);
	}

	public void ProfundoresUP(){
		fisica.ApplyTorqueByPointForce (profundoresPosition.position,(transform.up*-1)*profundoresForce);
	}

	public void ProfundoresDown(){
		Vector3 point = this.transform.position;
		point.z -= 1.5f;
		fisica.ApplyTorqueByPointForce (profundoresPosition.position,transform.up*profundoresForce);
	}

	public void LemeLeft(){
		fisica.ApplyTorqueByPointForce (lemePosition.position,transform.right*-1*lemeforce);
	}

	public void LemeRigth(){
		Vector3 point = this.transform.position;
		point.z += 1.5f;
		fisica.ApplyTorqueByPointForce (lemePosition.position,transform.right*lemeforce);
	}

	void Acelera()
	{
		if(motorFactor<1f)
		{
			motorFactor += 0.1f * Time.deltaTime;
		}

	}

	void Desacelera()
	{
		if(motorFactor>0f)
		{
			motorFactor -= 0.1f * Time.deltaTime;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Space)) {
			Acelera ();
		} 
		else {
			Desacelera ();
		}

		if (Input.GetKey (KeyCode.UpArrow)) {
			ProfundoresUP ();
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			ProfundoresDown ();
		}

		if (Input.GetKey (KeyCode.A)) {
			LemeLeft ();
		} else if (Input.GetKey (KeyCode.D)) {
			LemeRigth ();
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {
			AileronsLeft ();
		}
		else if (Input.GetKey (KeyCode.RightArrow)) {
			AileronsRight ();
		}
		setForces ();
	
	}
}
