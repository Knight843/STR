using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Camera playerCamera; // Referencia a la cámara del jugador
    public float normalFOV = 60f; // Campo de visión normal
    public float escalandoFOV = 80f; // Campo de visión al escalar
    public float transitionSpeed = 5f; // Velocidad de transición entre FOVs

    private playerScript player; // Referencia al script del jugador

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponent<Camera>();
        }

        player = FindObjectOfType<playerScript>(); // Encuentra el script del jugador en la escena
    }

    void Update()
    {
        if (player != null)
        {
            // Cambia el FOV dependiendo de si el jugador está escalando
            if (player.estaEscalando)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, escalandoFOV, Time.deltaTime * transitionSpeed);
            }
            else
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, normalFOV, Time.deltaTime * transitionSpeed);
            }
        }
    }
}
