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
    public float velocidadEscalada = 5f; // Velocidad de escalada
    public float rollForce = 10f; // Fuerza de la rodadura
    public float rollCooldown = 1f; // Tiempo de espera entre rodaduras
    public Transform Cam;
    public Transform Spawn;
    public bool estaEscalando = false; // Indica si el jugador está escalando

    public bool IsGrounded;

    private Rigidbody rb;
    private Renderer playerRenderer; // Referencia al Renderer del jugador
    private Color originalColor; // Color original del jugador
    private float speed = 15f;
    private float RotationX;
    private float RotationY;
    private bool canRoll = true; // Controla si el jugador puede rodar

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerRenderer = GetComponent<Renderer>(); // Obtiene el Renderer del jugador
        originalColor = playerRenderer.material.color; // Guarda el color original del jugador

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

        if (Input.GetKeyDown(KeyCode.LeftControl) && canRoll) // Rodar con la tecla Ctrl
        {
            Roll(x, y);
        }

        if (!estaEscalando)
        {
            // Movimiento normal
            transform.Translate(new Vector3(x, 0, y) * Time.deltaTime * speed);
        }
        else
        {
            // Movimiento de escalada
            float inputVertical = Input.GetAxis("Vertical"); // W/S o ↑/↓
            float inputHorizontal = Input.GetAxis("Horizontal"); // A/D o ←/→
            Vector3 direccionEscalada = Vector3.up * inputVertical * velocidadEscalada;
            Vector3 movimientoHorizontal = transform.right * inputHorizontal * velocidadEscalada;
            rb.velocity = direccionEscalada + movimientoHorizontal; // Combina movimiento vertical y horizontal
        }

        RotationX += -Input.GetAxis("Mouse Y") * Sensibility;
        RotationX = Mathf.Clamp(RotationX, -LimitX, LimitX);
        Cam.localRotation = Quaternion.Euler(RotationX, 0, 0);
        transform.rotation *= (Quaternion.Euler(0, Input.GetAxis("Mouse X") * Sensibility, 0));
    }

    public void Jump()
    {
        if (IsGrounded && !estaEscalando)
        {
            rb.AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);
        }
    }

    private void Roll(float x, float y)
    {
        if (x == 0 && y == 0) return; // No rodar si no hay movimiento

        canRoll = false; // Desactiva la capacidad de rodar temporalmente

        // Cambia el color del jugador para indicar que está rodando
        playerRenderer.material.color = Color.red;

        // Calcula la dirección de la rodadura
        Vector3 rollDirection = transform.TransformDirection(new Vector3(x, 0, y).normalized);

        // Aplica una fuerza en la dirección de la rodadura
        rb.AddForce(rollDirection * rollForce, ForceMode.Impulse);

        // Inicia el cooldown para la siguiente rodadura
        StartCoroutine(RollCooldown());
    }

    private IEnumerator RollCooldown()
    {
        yield return new WaitForSeconds(rollCooldown);

        // Restaura el color original del jugador
        playerRenderer.material.color = originalColor;

        canRoll = true; // Permite rodar nuevamente después del cooldown
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            IsGrounded = true;
        }
        if (collision.gameObject.tag == "DeathZone")
        {
            transform.position = Spawn.position;
        }
        if (collision.gameObject.CompareTag("escalable"))
        {
            IniciarEscalada();
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
            DetenerEscalada();
        }
    }

    private void IniciarEscalada()
    {
        estaEscalando = true;
        rb.useGravity = false; // Desactiva la gravedad mientras escala
        rb.velocity = Vector3.zero; // Detiene cualquier movimiento previo
    }

    private void DetenerEscalada()
    {
        estaEscalando = false;
        rb.useGravity = true; // Reactiva la gravedad al dejar de escalar
    }
}
