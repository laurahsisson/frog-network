using UnityEngine;
using System.Collections;
using System;
using Random=UnityEngine.Random;
public class FrogMove2 : MonoBehaviour, IComparable {
	public NeuralNetwork2 nn;
	Room_Control rc;
	float roomSize;
	float direction;
	float speed=0;
	float jumpSpeed=5f;
	float maxSpeed=5f;
	float jumpTime=1f;
	float lastJump=0;
	float hiddenNeurons;
	public bool printInfo=false;
	public bool canJump = true;
	int points; //Gain ten points each time you eat a pellet, lose one each time you jump 
	//TODO: LOOK AT ADDING IN MUTATIONS
	//TODO: AFTER MUTATIONS, ADD BACK POINTS--
	// Use this for initialization
	void Start () {

		direction=Random.value*360;
	}
	// Update is called once per frame
	void Update () {
		move ();
		wrap ();
		lastJump+=Time.deltaTime;
		if (nn.getJump()&&canJump){
			jump(nn.getXForce(),nn.getYForce());
		}
	}
	void move(){
		if (speed > jumpSpeed/2){ //Getting bigger
			transform.localScale+=(Vector3)Vector2.one*.05f;
		}
		if (speed < jumpSpeed/2 && speed>0){ //Getting smaller
			transform.localScale-=(Vector3)Vector2.one*.05f;
		}
		if (direction > 360) {
			direction %= 360;
		}
		transform.rotation = Quaternion.Euler (0, 0, direction);
		if (speed > 0) { //Slow down
			speed -= jumpSpeed * jumpTime * Time.deltaTime; //Slow down over the course of jumpTime
		}
		if (speed < 0) { //Dont overshoot
			transform.localScale=Vector2.one*4;
			speed = 0;
		}
		transform.position = new Vector3 (transform.position.x + Mathf.Cos (Mathf.Deg2Rad * direction) * speed * Time.deltaTime, transform.position.y + Mathf.Sin (Mathf.Deg2Rad * direction) * speed * Time.deltaTime);

	}
	void wrap(){
		if (transform.position.x > roomSize) { //Wrap
			transform.position = new Vector3 (-roomSize, transform.position.y);
		}
		if (transform.position.x < -roomSize) {
			transform.position = new Vector3 (roomSize, transform.position.y);
		}
		if (transform.position.y > roomSize) {
			transform.position = new Vector3 (transform.position.x, -roomSize);
		}
		if (transform.position.y < -roomSize) {
			transform.position = new Vector3 (transform.position.x, roomSize);
		}
	}
	void jump(float x, float y){
		if (speed==0){
			//points--;
			lastJump=0;
			speed=nn.getSpeed()*maxSpeed; 
			speed=jumpSpeed;//So that we always go down to 0 over the same time.
			//Debug.Log(speed);
			direction=Mathf.Atan2(y,x)*Mathf.Rad2Deg;
		}
	} 
	public int GetPoints(){

		return (points>0) ? points : 0;
	}
	public void Setup(Room_Control rc, float roomSize, NeuralNetwork2 nn){
		this.rc=rc;
		this.roomSize=roomSize;
		this.nn=nn;
	}
	public float getLastJump(){
		return lastJump;
	}
	void OnTriggerStay2D (Collider2D co)
	{
		//Debug.Log (co.gameObject.name);
		if (printInfo){
			Debug.Log(speed);
		}
		if (co.gameObject.CompareTag("Food")&&speed<jumpSpeed/2){
			points+=10; 
			rc.RemovePellet (co.gameObject.transform);
		}
	}
	public int CompareTo(object o){
		return ((FrogMove2)o).points-points;
	}
}
