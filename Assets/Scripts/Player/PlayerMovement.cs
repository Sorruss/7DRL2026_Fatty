using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour {

    public InputActionReference movementAction;
    public InputActionReference dashAction;
    public float speed;
    public float health;
    public float maxHealth;
    public bool canMove = true;
    public bool isDead;
    Transform body;

    bool canDash = true;
    bool isDashing;
    float dashTimer;
    public float dashingPower;
    public float dashingTime;
    public float dashingCooldown;
    TrailRenderer trailRenderer;

    public Slider healthSlider;
    public Slider dashSlider;
    public GameObject healthTextPrefab;
    public Transform textParent;

    Rigidbody2D rb;
    [Header("Screen Shake Settings")]
    public float shakeAmount;
    public float shakeTime;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        body = GetComponentInChildren<SpriteRenderer>().transform;
        //trailRenderer = GetComponent<TrailRenderer>();
        //dashSlider.GetComponent<CanvasGroup>().alpha = 0;

        //healthSlider.maxValue = maxHealth;
        //healthSlider.value = health;
    }

    public void UpdateSliders() {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
    }

    private void Update() {
        if (canMove && !isDashing) {
            // player look
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            body.up = mousePos - new Vector2(body.position.x, body.position.y);

            // player move
            //var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); old
            var input = movementAction.action.ReadValue<Vector2>();
            rb.linearVelocity = input.normalized * speed;

            if (dashAction.action.WasPressedThisFrame() && dashingPower != 0 && canDash) {
                StartCoroutine(Dash());
            }
        }
        if (!canDash) {
            dashTimer -= Time.deltaTime;
            //dashSlider.value = dashTimer;
    }
    }

    IEnumerator Dash() {
        //dashSlider.GetComponent<CanvasGroup>().alpha = 1;
        //dashSlider.maxValue = dashingCooldown;
        dashTimer = dashingCooldown;
        canDash = false;
        isDashing = true;
        rb.linearVelocity = new Vector2(body.up.x * dashingPower, body.up.y * dashingPower);
        //trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        //trailRenderer.emitting = false;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
        //dashSlider.GetComponent<CanvasGroup>().alpha = 0;
    }
    void InstantiateTextPopUp(Color color, string message) {
        GameObject GO = Instantiate(healthTextPrefab, transform.position, Quaternion.identity, textParent);
        GO.GetComponent<Popuptext>().textcolour = color;
        GO.GetComponent<Popuptext>().SetText(message);
    }

    public void TakeHealth(float amount) {
        CameraShake.Instance.ShakeCamera(shakeAmount, shakeTime);
        health -= amount;
        healthSlider.value = health;
        InstantiateTextPopUp(Color.red, amount.ToString("F0"));
        if (health <= 0) Die();
    }
    public void AddHelth(float amount) {
        health += amount;
        if(health > maxHealth) health = maxHealth;
        healthSlider.value = health;
        InstantiateTextPopUp(Color.green, amount.ToString("F0"));
    }

    void Die() {
        isDead = true;
        canMove = false;
        //Destroy(gameObject);
    }
}
