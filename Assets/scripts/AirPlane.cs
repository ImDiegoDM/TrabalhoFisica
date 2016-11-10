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
    public float ArConstant;

	//---- variaveis de animação------
	float aileronL=0;
	float aileronR=0;
	float profundorUp=0;
	float profundorDown=0;
	float lemeR=0;
	float lemeL=0;
	int helice=0;
	Animator anim;



    Vector3 oldPostion=Vector3.zero;

	float superficeFront;//valo da area da superfice quando estiver reto


	// Use this for initialization
	void Awake () {
		anim = this.GetComponentInChildren<Animator> ();
		fisica = this.GetComponent<MyFisica> ();
		superficeFront = areaDeSuperficie;
	}

	void setArForce(){ // seta a força do ar com base na altitude
		try{
			forceAr=(35f/altitude);
		}
		catch {
			forceAr=(35f/altitude+0.01f);
		}
        if (forceAr < 0.01f) {
			forceAr = 0.01f;
		}
	}

	public void setFisica(bool value)
	{
		fisica.fisica = value;
	}

	void CalculaAltitude() 
	{
		altitude = this.transform.position.y;
	}

    Vector3 CalculaVelocidade()
    {
        Vector3 variacaoPosi = oldPostion - this.transform.position;
        oldPostion = this.transform.position;
        return variacaoPosi/Time.fixedDeltaTime;
    }


	void setForces()// calcula as forças exercidar no avião e seta elas na fisica
	{
		CalculaAltitude ();//calcula a altitude do avião
		setArForce ();// seta a densidade do ar


		Vector3 force = transform.forward;
        force *= (MotorForce * motorFactor);// calcula a força com base no motor e o qual aquele motor está sendo usado
		force.y=0;

		float superficieAtual=areaDeSuperficie;// seta a area de superfice do avião
		Vector3 arFoce;
		if (this.transform.eulerAngles.x < 10 || this.transform.eulerAngles.x > 180) {
			arFoce = transform.up;
		} else {
			arFoce = transform.up*-1;
		}


		arFoce *= ArConstant*(forceAr)*superficieAtual*Mathf.Pow ((fisica.getVelocity().magnitude),2)/2;// calcula a força do ar com base na velocidade do avião
		//se estiver muito inclinado a potencia do motor cai para 40% e a superfice de contato do avião aumenta drasticamente
		print(this.transform.eulerAngles.x);
		if ((this.transform.eulerAngles.x < 330f && this.transform.eulerAngles.x > 210f) || (this.transform.eulerAngles.x < 150f && this.transform.eulerAngles.x > 30f)) {
            //force = 70f * force/ 100f;
			superficieAtual *= 5f;
            print("muito inclinado");
		}

		// se o angulo de atack for muito inclinado o força do motor cai para 5%
		if ((this.transform.eulerAngles.x < 290f && this.transform.eulerAngles.x > 250f) || (this.transform.eulerAngles.x < 100f && this.transform.eulerAngles.x > 70f)) {
            force = 5f * force/ 100f;
			print("muito inclinado mesmo");
		}

		//ponto onde ira acontecer uma leve inclinação se o aviao não estiver em equilibrio
		Vector3 point = this.transform.position;
		point.z += 0.1f;

		// se o avião estiver mais de 10 angulos do ponto inicial dele ele não esta em equilibrio
		if (!((this.transform.eulerAngles.x<360f&&this.transform.eulerAngles.x>350f ) || (this.transform.eulerAngles.x<10&&this.transform.eulerAngles.x>0 ))) {// angulo de ataque faz com que ele fique em equilibrio
			fisica.ApplyTorqueByPointForce (point,arFoce);
		}

		//calcula a força do ar que colide com a frente do avião
		float drag = coeficienteAeroDinamica * (forceAr / 2f) * superficieAtual * Mathf.Pow ((fisica.getVelocity().magnitude),2);

        fisica.AddForce(force);
        fisica.AddForce (arFoce);
        fisica.AddForce (drag*fisica.getVelocity().normalized*-1);

	}

	/// <summary>
	/// Aciona os Airelons fazendo o avião rodar para a esquerda
	/// </summary>
	public void AileronsLeft(){
		fisica.ApplyTorqueByPointForce (aileronsRightPosition.position,transform.up*aileronsForce);
		StartAnim (ref aileronL,ref aileronR,"AileronL","AileronR",0.05f);
	}


	/// <summary>
	/// Aciona os Ailerons fazendo o avião rodar para a direira
	/// </summary>
	public void AileronsRight(){
		fisica.ApplyTorqueByPointForce (aileronsLeftPosition.position,transform.up*aileronsForce);
		StartAnim (ref aileronR,ref aileronL,"AileronR","AileronL",0.05f);
	}

	/// <summary>
	/// Aciona os Profundores fazendo ele levantar o bico do avião
	/// </summary>
	public void ProfundoresUP(){
		fisica.ApplyTorqueByPointForce (profundoresPosition.position,(transform.up*-1)*profundoresForce);
		StartAnim (ref profundorUp,ref profundorDown,"ProfundorUp","ProfundorD",0.05f);
	}


	/// <summary>
	/// Aciona os Profundores fazendo ele abaixar o bico do avião
	/// </summary>
	public void ProfundoresDown(){
		Vector3 point = this.transform.position;
		point.z -= 1.5f;
		fisica.ApplyTorqueByPointForce (profundoresPosition.position,transform.up*profundoresForce);
		StartAnim (ref profundorDown,ref profundorUp,"ProfundorD","ProfundorUp",0.05f);
	}

	/// <summary>
	/// Aciona os lemes fazendo o avião virar a esquerda
	/// </summary>
	public void LemeLeft(){
		fisica.ApplyTorqueByPointForce (lemePosition.position,transform.right*-1*lemeforce);
		StartAnim (ref lemeL,ref lemeR,"LemeL","LemeR",0.05f);
	}

	/// <summary>
	/// Aciona os lemes fazendo o avião virar a direita
	/// </summary>
	public void LemeRigth(){
		Vector3 point = this.transform.position;
		point.z += 1.5f;
		fisica.ApplyTorqueByPointForce (lemePosition.position,transform.right*lemeforce);
		StartAnim (ref lemeR,ref lemeL,"LemeR","LemeL",0.05f);
	}

	void StartAnim(ref float variavelAdd,ref float variavelSub,string nomeParAdd,string nomeParSub,float fator){
		variavelAdd += fator;
		if (variavelAdd >= 1f) {
			variavelAdd = 1f;
		}

		variavelSub -= fator;
		if (variavelSub <= 0f) {
			variavelSub = 0f;
		}
		anim.SetFloat (nomeParAdd,variavelAdd);
		anim.SetFloat (nomeParSub,variavelSub);
	}

	/// <summary>
	/// faz com que o motor do avião rode na potencia passada por parametro, sendo 0 = 0% e 1=100%
	/// </summary>
	/// <param name="fator">Fator do motor que ira usar</param>
	public void forçaMotor(float fator)
	{
		anim.SetFloat ("heliceSpeed",fator);
		if(motorFactor<fator)
		{
			motorFactor += 0.05f * Time.deltaTime;
		}
		else
		{
			motorFactor = fator;
		}
	}

	void parametroBackNormal(ref float parametro,float fator,string nameAnimPar){
		parametro -= fator;
		if (parametro <= 0f) {
			parametro = 0f;
		}
		anim.SetFloat (nameAnimPar, parametro);
	}

	public float getAltitude()
	{
		return altitude;
	}

	public float getVelocityInKmh()
	{
        return fisica.getVelocity().magnitude;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.UpArrow)) {
			ProfundoresUP ();
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			ProfundoresDown ();
		} else {
			parametroBackNormal (ref profundorUp,0.05f,"ProfundorUp");
			parametroBackNormal (ref profundorDown,0.05f,"ProfundorD");
		}

		if (Input.GetKey (KeyCode.A)) {
			LemeLeft ();
		} else if (Input.GetKey (KeyCode.D)) {
			LemeRigth ();
		} else {
			parametroBackNormal (ref lemeL,0.05f,"LemeL");
			parametroBackNormal (ref lemeR,0.05f,"LemeR");
		}
			

		if (Input.GetKey (KeyCode.LeftArrow)) {
			AileronsLeft ();
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			AileronsRight ();
		} else {
			parametroBackNormal (ref aileronL,0.05f,"AileronL");
			parametroBackNormal (ref aileronR,0.05f,"AileronR");
		}
		setForces ();
	
	}
}
