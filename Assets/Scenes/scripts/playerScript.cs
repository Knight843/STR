using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour


{
    public float MinSpeed = 15f;
    public float MaxSpeed = 25f;
    public float JumpForce = 2f;
    public float Sensibility = 2f;
    public float LimitX = 45;
    public float velocidadEscalada = 3f;
    public float gravedad = 9.8f;
    public KeyCode teclaEscalar = KeyCode.E;
    public Transform Cam;
    public Transform Spawn;

    public bool IsGrounded;

    private Rigidbody rb;
    private float speed = 15f;
    private float RotationX;
    private float RotationY;
    private bool estaCercaDeEscalable = false;
    private bool estaEscalando = false;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb.useGravity = true;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = MaxSpeed;
        }
        else
        {
            speed = MinSpeed;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        transform.Translate(new Vector3(x, 0, y) * Time.deltaTime * speed);

        RotationX += -Input.GetAxis("Mouse Y") * Sensibility;
        RotationX = Mathf.Clamp(RotationX, -LimitX, LimitX);
        Cam.localRotation = Quaternion.Euler(RotationX, 0, 0);
        transform.rotation *= (Quaternion.Euler(0, Input.GetAxis("Mouse X") * Sensibility, 0));

        if (estaCercaDeEscalable && Input.GetKey(teclaEscalar))
        {
            IniciarEscalada();
        }
        else
        {
            DetenerEscalada();
        }

        if (estaEscalando)
        {
            float inputVertical = Input.GetAxis("Vertical"); // W/S o ↑/↓
            Vector3 direccionSubida = Vector3.up * inputVertical * velocidadEscalada;
            rb.velocity = new Vector3(rb.velocity.x, direccionSubida.y, rb.velocity.z);
        }
    }
    public void Jump()
    {
        if (IsGrounded)
        {
            rb.AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);
        }
    }
    void IniciarEscalada()
    {
        estaEscalando = true;
        rb.useGravity = false;
    }
    void DetenerEscalada()
    {
        if (estaEscalando)
        {
            estaEscalando = false;
            rb.useGravity = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            IsGrounded = true;
        }
        if(collision.gameObject.tag == "DeathZone") 
        {
            transform.position = Spawn.position;
        }
        if (collision.gameObject.CompareTag("escalable"))
        {
         estaCercaDeEscalable = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            IsGrounded = false;
        }
        if (collision.gameObject.CompareTag("escalable"))
        {
         estaCercaDeEscalable = false;
         DetenerEscalada();
        }
    }
    
}
