using UnityEngine;
using System.Collections;

public class ProfessorRigidBody : MonoBehaviour {

	public Vector3 currentVelocity = Vector3.zero;//velocidade do deslocamento
	public Vector3 currentAngularvelocity = Vector3.zero;//velocidade do giro

	public float mass = 0.0f;

	public float drag = 0.0f;
	public float angularDrag = 0.1f;

	private Vector3 inertiaTensor = Vector3.zero;

	//Material information
	public float bouciness = 0.0f;
	public float staticFriction = 0.0f;
	public float dynamicFriction = 0.0f;

	private Vector3 forceToBeApplied = Vector3.zero;

	private float currentTime = 0.0001f;//Used to know the time passed

	private Vector3 collisionTransferVelocity = Vector3.zero;


	void Start()
	{
		//Do nothing
		CalculeInertiaTensor();

	}

	void CalculeInertiaTensor()
	{
		float rayPow = Mathf.Pow ((transform.lossyScale.x / 2.0f), 2.0f);

		float inertiaValue = (2.0f / 5.0f) * mass * rayPow;
		inertiaTensor = new Vector3 (inertiaValue, inertiaValue, inertiaValue);
	}

	Vector3 CalculateAngularAcceleration(Vector3 torque)
	{
		//from tangent acceleration to angular acceleration
		Vector3 angularAceleration = torque;

		//Calculating torque based in it's inertia
		angularAceleration.x /= inertiaTensor.x;
		angularAceleration.y /= inertiaTensor.y;
		angularAceleration.z /= inertiaTensor.z;

		return angularAceleration;
	}

	void ApplyTorqueByPointForce(Vector3 point, Vector3 force)
	{
		Vector3 torque = Vector3.Cross (point - transform.position, force);

		currentAngularvelocity += CalculateAngularAcceleration (torque);
	}

	public void ApplyTorque(Vector3 torque)
	{
		//Update teh angular velocity
		currentAngularvelocity += CalculateAngularAcceleration (torque);
	}

	public void ApplyForce(Vector3 force)
	{
		forceToBeApplied += force;
		print ("testes");
	}

	public void ApplyPointForce(Vector3 point, Vector3 force)
	{
		ApplyTorqueByPointForce (point, force);
		ApplyForce (force);
	}

	Vector3 calculateAccelerationLinear(Vector3 forceApplied)
	{
		Vector3 gravityAcceleration = Settings.getInstance ().gravityAcceleration;

		//Finding the resultance force based the force applied plus the gravity
		Vector3 resultantForce = forceApplied + (mass * gravityAcceleration);

		//Calculating the friction
		float airResistence = Settings.getInstance().airResistence;
		Vector3 resistence = resultantForce.normalized * airResistence;

		Vector3 acceleration = (resultantForce - resistence) / mass;

		return acceleration;
	}

	void UpdateAngularVelocity()
	{
		//Rotate the object in world space
		transform.Rotate(currentAngularvelocity * Time.deltaTime, Space.World);

		//Update torque
		currentAngularvelocity -= currentVelocity * angularDrag * Time.deltaTime;

		if (currentAngularvelocity.magnitude < 0.1f)
			currentAngularvelocity = Vector3.zero;
	}

	void UpdateVelocity()
	{
		Vector3 acceleration = calculateAccelerationLinear (forceToBeApplied) * Time.deltaTime;

		currentVelocity += acceleration;

		//Using the drag to brake the velocity
		currentVelocity -= currentVelocity * drag * Time.deltaTime;

		if (currentVelocity.magnitude < 0.5f)
		{
			currentTime = 0.0001f;
			currentVelocity = Vector3.zero;

		}
	}
	
	void Update()
	{
		//Updating teh position in world
		transform.position += currentVelocity * Time.deltaTime;
		
		//Update velocity and Torque
		UpdateVelocity();
		UpdateAngularVelocity();
		
		//When the object find the state of balance
		currentTime += Time.deltaTime;
		
		//Clear the impulse to be applied if no other force will act on the object
		forceToBeApplied = Vector3.zero;
	}

	void OnCollisionEnter(Collision collision)
	{
		//To forces applied (Energy Kinect and weight), When colliding we have the trnafer of energy of the objects colliding
		ProfessorRigidBody other = collision.gameObject.GetComponent<ProfessorRigidBody>();

		if (other == null)//other object has much mass
			collisionTransferVelocity = currentVelocity;
		else
			collisionTransferVelocity = currentVelocity + other.currentVelocity;

		currentTime = 0.0001f;
	}

	void OnCollsionStay(Collision collision)
	{
		Vector3 gravityAcceleration = Settings.getInstance ().gravityAcceleration;

		//I = variacao do momento
		//I = variacao da forca em relacao ao tempo
		//Vector3 impulse = mass * currentVelocity;
		//Vector3 forceApplied = impulse / currentTime + (mass * gravityAcceleration);

		//a = (Fr - f)/ m -> a = Fr / m -> (fr = a * m)
		//a = variacao da velocidade pela variacao do tempo
		//fr = (varicao da velocidade / varicao do tempo * massa)
		Vector3 forceApplied = (mass * currentVelocity / currentTime) + (mass * gravityAcceleration);

		//By simplification only use one avarage point of the collision
		Vector3 normalVector = Vector3.zero;
		Vector3 point = Vector3.zero;

		for (int i = 0; i < collision.contacts.Length; i++) {
			normalVector += collision.contacts [i].normal;
			point += collision.contacts [i].point;
		}

		point /= collision.contacts.Length;
		normalVector.Normalize ();

		//para achar o angulo
		//N = N x ||F|| x cos()
		//cos() = v * u / ||v||* ||u||
		Vector3 normalForce = normalVector * forceApplied.magnitude * Vector3.Dot (-normalVector, forceApplied.normalized);

		//Calculating the transfer of enrgy
		Vector3 forceAppliedWithEnergyTransfer = (mass * collisionTransferVelocity / currentTime) + (mass * gravityAcceleration);

		//Calculating bouciness
		Vector3 boucinessForce = (normalVector) * bouciness * forceAppliedWithEnergyTransfer.magnitude;

		//Update velocity
		Vector3 acceleration = (normalForce + boucinessForce) / mass;
		currentVelocity += acceleration * currentTime;

		//Calculating friction
		Vector3 friction;

		if (currentVelocity.magnitude < 1.0f)
			friction = -currentVelocity.normalized * (normalForce.magnitude) * staticFriction;
		else
			friction = -currentVelocity.normalized * (normalForce.magnitude) * dynamicFriction;

		currentVelocity += friction / mass * currentTime;

		//Update torque(When have bounciness the torque will accour only one time, then we must ignore deltaTime)
		ApplyTorqueByPointForce(point, friction + normalForce);

		currentTime = 0.0001f;
	}


}
