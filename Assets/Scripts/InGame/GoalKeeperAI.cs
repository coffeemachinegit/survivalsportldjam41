﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeeperAI : MonoBehaviour {

	private Transform ballTransform;

	private float deltaX;
	private bool ballCatch = false;
	
	public float speed;
	public float yTop,yDown;

	private Vector2 newPos;
	public float oldPosY;
	public PlayerRotation rotation;
	public Transform playerGoal;
	public Transform ballPoss;

	public AudioSource source;
	public AudioClip kickSFX;
	public Animator animator;

	private bool _isCoroutineRunning = false;

	
	void Start () {
		ballTransform = GameManager.Instance.ballPosition;
		newPos = new Vector2(transform.position.x,0);
	}
	
	
	void Update () {
		if(!GameManager.Instance.canPlay)
			return;
			
		deltaX = ballTransform.position.x - gameObject.transform.position.x;
		if(deltaX < 5 && !ballCatch){
			newPos.y = Mathf.Clamp(ballTransform.position.y,yDown,yTop);
			oldPosY = transform.position.y;
			if(newPos.y != oldPosY){
				animator.SetBool("isWalking",true);
			}else{
				animator.SetBool("isWalking",false);
			}
			transform.position = Vector2.MoveTowards(transform.position,newPos,speed * Time.deltaTime);
		}else
			animator.SetBool("isWalking",false);
	}

	private void OnCollisionEnter2D(Collision2D other) {
		if(!GameManager.Instance.canPlay)
			return;

		if(other.gameObject.tag == "Ball"){
			if(Random.Range(0,2) == 1 && !_isCoroutineRunning){
				ballCatch = true;
				rotation.ballPostion = playerGoal;
				other.gameObject.transform.SetParent(gameObject.transform);
				other.gameObject.transform.position = ballPoss.position;
				GameManager.Instance.ballRB.velocity = Vector2.zero;
				other.gameObject.GetComponent<CircleCollider2D>().enabled = false;
				StartCoroutine(KickBall(other.gameObject));
			}
		}
	}
	
	IEnumerator KickBall(GameObject ball){
		_isCoroutineRunning = true;
		yield return new WaitForSeconds(1);
		Vector2 newVelocity = new Vector2(Random.Range(-12,-30),Random.Range(-11,12));
		GameManager.Instance.ballRB.AddForce(newVelocity,ForceMode2D.Impulse);
		ball.GetComponent<CircleCollider2D>().enabled = true;
		ball.transform.SetParent(null);
		rotation.ballPostion = ball.transform;
		ballCatch = false;
		source.PlayOneShot(kickSFX);
		_isCoroutineRunning = false;

	}
}
