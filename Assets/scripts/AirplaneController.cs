using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class AirplaneController : MonoBehaviour {

    public GameObject spawn;
    public Vector3 offsetRotate;
	private AirPlane airplane;
	public Slider acelerador;
	public Text velocidade, altitude;

    void Awake()
    {
		GameObject aircraft = Persistent.singleton.airplaneList[Persistent.singleton.indexAirplaneSelect];

		aircraft = (GameObject)Instantiate(aircraft, spawn.transform.position, Quaternion.identity);
		airplane = aircraft.GetComponent<AirPlane>();
		airplane.setFisica (true);
        aircraft.transform.Rotate(offsetRotate);
    }

	void Update()
	{
		airplane.forçaMotor (acelerador.value);
		velocidade.text = airplane.getVelocityInKmh ().ToString ("0") + " Km/h";
		altitude.text = airplane.getAltitude().ToString("0") + " m";
	}
}
