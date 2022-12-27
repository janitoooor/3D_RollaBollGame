using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private float speed = 10;
    private float speedWalls = -0.5f;
    private float speedLift = -5;

    public TextMeshProUGUI countText;

    public ParticleSystem finishParticles;

    public GameObject winTextObject;
    public GameObject winerTextObject;

    public GameObject walls;
    public GameObject secondWall;
    public GameObject Finishwalls;
    public GameObject Lastwalls;

    public GameObject redButtonObject;
    public GameObject blueButtonObject;
    public GameObject whiteButtonObject;
    public GameObject lift;

    private AudioSource audioEffect;

    private int countToWin = 15;
    private int secondCountToWin = 20;
    private int timeCoroutine = 8;
    private int textTime = 6;
    public int count;

    private Rigidbody rb;

    private float movementX;
    private float movementY;

    private bool blueButton;
    private bool redButton;
    private bool finishButton;
    private bool lvlOne;

    private bool liftOnGround;

    private bool yellowSensor;

    private bool liftInFly;


    // Start is called before the first frame update
    void Start()
    {
        lvlOne = true;
        rb = GetComponent<Rigidbody>();
        audioEffect = GetComponent<AudioSource>();
        count = 0;

        SetCountText();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void PlayerMove()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        rb.AddForce(movement * speed);
    }

    IEnumerator TextFalse()
    {
        yield return new WaitForSeconds(textTime);
        winTextObject.SetActive(false);
        count = 0;
        countText.text = "Count: " + count.ToString() + "/" + secondCountToWin;
        lvlOne = false;
    }

    void SetCountText()
    {
        if(lvlOne)
        {
            countText.text = "Count: " + count.ToString() + "/" + countToWin;

            if (count >= countToWin)
            {
                winTextObject.SetActive(true);
                StartCoroutine(TextFalse());
            }
        }
        else if(!lvlOne)
        {
            countText.text = "Count: " + count.ToString() + "/" + secondCountToWin;
        }
        if(finishButton == true)
        {
            winerTextObject.SetActive(true);
            finishParticles.Play();
        }
    }

    IEnumerator ObjectFalse(GameObject gameObject)
    {
        yield return new WaitForSeconds(timeCoroutine);
        Destroy(gameObject);
    }

    void TranslateObject(GameObject gameObject, IEnumerator methodName, float speed)
    {
        if(gameObject != null)
        {
            gameObject.transform.Translate(Vector3.up * speed * Time.deltaTime);
            StartCoroutine(methodName);
        }
    }

    IEnumerator LiftMoveTime()
    {
        yield return new WaitForSeconds(timeCoroutine);
        liftOnGround = true;
    }

    IEnumerator LiftMoveUp()
    {
        yield return new WaitForSeconds(timeCoroutine);
        liftInFly = true;
    }
    void WallAndLiftMove()
    {
        if (!liftOnGround && redButton == true)
        {
            TranslateObject(secondWall, ObjectFalse(secondWall), speedWalls);
            TranslateObject(lift, LiftMoveTime(), speedLift);
        }
        else if (!liftInFly && blueButton == true)
        {
            TranslateObject(lift, LiftMoveUp(), -speedLift);
        }
        else if (count >= countToWin)
        {
            TranslateObject(walls, ObjectFalse(walls), speedWalls);
        }
        if(yellowSensor == true)
        {
            TranslateObject(Lastwalls, ObjectFalse(Lastwalls), speedWalls);
            if (count >= secondCountToWin)
            {
                TranslateObject(Finishwalls, ObjectFalse(Finishwalls), speedWalls);
            }
        }
    }

    void FixedUpdate()
    {
        PlayerMove();
        WallAndLiftMove();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
            audioEffect.Play();
        }
        else if(other.gameObject.CompareTag("Sensor"))
        {
            yellowSensor = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("RedButton") && !redButton)
        {
            redButtonObject.transform.Translate(Vector3.up * -0.17f);
            redButton = true;
        }
        else if(collision.gameObject.CompareTag("BlueButton") && !blueButton)
        {
            blueButtonObject.transform.Translate(Vector3.up * -0.17f);
            blueButton = true;
        }
        else if(collision.gameObject.CompareTag("FinishGame"))
        {
            whiteButtonObject.transform.Translate(Vector3.up * -0.17f);
            finishButton = true;
            SetCountText();
        }
    }
}
