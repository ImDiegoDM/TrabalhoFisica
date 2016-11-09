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

	float superficeFront;//valo da area da superfice quando estiver reto


	// Use this for initialization
	void Awake () {
		fisica = this.GetComponent<MyFisica> ();
		superficeFront = areaDeSuperficie;
	}

	void setArForce(){ // seta a força do ar com base na altitude
		forceAr=1f-(altitude/800f);
		if (forceAr < 0f) {
			forceAr = 0;
		}
	}

	public void setFisica(bool value)
	{
		fisica.fisica = value;
	}

	void CalculaAltitude() 
	{
		altitude = this.transform.position.y*10f;
	}



	void setForces()// calcula as forças exercidar no avião e seta elas na fisica
	{
		CalculaAltitude ();//calcula a altitude do avião
		setArForce ();// seta a densidade do ar


		Vector3 force = transform.forward;
		force *= (MotorForce * motorFactor);// calcula a força com base no motor e o qual aquele motor está sendo usado

		float superficieAtual=areaDeSuperficie;// seta a area de superfice do avião

		Vector3 arFoce = transform.up;
		arFoce *= ((MotorForce * motorFactor) * forceAr) * fisica.getVelocity ().magnitude/fisica.massa;// calcula a força do ar com base na velocidade do avião
		//se estiver muito inclinado a potencia do motor cai para 40% e a superfice de contato do avião aumenta drasticamente
		if ((this.transform.eulerAngles.x < 330f && this.transform.eulerAngles.x > 210f) || (this.transform.eulerAngles.x < 150f && this.transform.eulerAngles.x > 30f)) {
			force = 40f * force/ 100f;
			superficieAtual *= 30f;
		}

		// se o angulo de atack for muito inclinado o força do motor cai para 5%
		if ((this.transform.eulerAngles.x < 290f && this.transform.eulerAngles.x > 250f) || (this.transform.eulerAngles.x < 100f && this.transform.eulerAngles.x > 70f)) {
			force = 5f * force/ 100f;
		}

		//ponto onde ira acontecer uma leve inclinação se o aviao não estiver em equilibrio
		Vector3 point = this.transform.position;
		point.z += 0.1f;

		// se o avião estiver mais de 10 angulos do ponto inicial dele ele não esta em equilibrio
		if (!((this.transform.eulerAngles.x<360f&&this.transform.eulerAngles.x>350f ) || (this.transform.eulerAngles.x<10&&this.transform.eulerAngles.x>0 ))) {// angulo de ataque faz com que ele fique em equilibrio
			fisica.ApplyTorqueByPointForce (point,arFoce);
		}

		//calcula a força do ar que colide com a frente do avião
		float drag = coeficienteAeroDinamica * (forceAr / 2) * superficieAtual * Mathf.Pow (fisica.getVelocity().magnitude,2);

		fisica.AddForce(force);
		fisica.AddForce (arFoce);
		fisica.AddForce (drag*fisica.getVelocity().normalized*-1);

	}

	/// <summary>
	/// Aciona os Airelons fazendo o avião rodar para a esquerda
	/// </summary>
	public void AileronsLeft(){
		fisica.ApplyTorqueByPointForce (aileronsRightPosition.position,transform.up*aileronsForce);
	}


	/// <summary>
	/// Aciona os Ailerons fazendo o avião rodar para a direira
	/// </summary>
	public void AileronsRight(){
		fisica.ApplyTorqueByPointForce (aileronsLeftPosition.position,transform.up*aileronsForce);
	}

	/// <summary>
	/// Aciona os Profundores fazendo ele levantar o bico do avião
	/// </summary>
	public void ProfundoresUP(){
		fisica.ApplyTorqueByPointForce (profundoresPosition.position,(transform.up*-1)*profundoresForce);
	}


	/// <summary>
	/// Aciona os Profundores fazendo ele abaixar o bico do avião
	/// </summary>
	public void ProfundoresDown(){
		Vector3 point = this.transform.position;
		point.z -= 1.5f;
		fisica.ApplyTorqueByPointForce (profundoresPosition.position,transform.up*profundoresForce);
	}

	/// <summary>
	/// Aciona os lemes fazendo o avião virar a esquerda
	/// </summary>
	public void LemeLeft(){
		fisica.ApplyTorqueByPointForce (lemePosition.position,transform.right*-1*lemeforce);
	}

	/// <summary>
	/// Aciona os lemes fazendo o avião virar a direita
	/// </summary>
	public void LemeRigth(){
		Vector3 point = this.transform.position;
		point.z += 1.5f;
		fisica.ApplyTorqueByPointForce (lemePosition.position,transform.right*lemeforce);
	}

	/// <summary>
	/// faz com que o motor do avião rode na potencia passada por parametro, sendo 0 = 0% e 1=100%
	/// </summary>
	/// <param name="fator">Fator do motor que ira usar</param>
	public void forçaMotor(float fator)
	{
		if(motorFactor<fator)
		{
			motorFactor += 0.1f * Time.deltaTime;
		}
		else
		{
			motorFactor = fator;
		}
	}

	public float getAltitude()
	{
		return altitude;
	}

	public float getVelocityInKmh()
	{
		return fisica.getVelocity ().magnitude;
	}
	
	// Update is called once per frame
	void Update () {

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
