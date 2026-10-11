using UnityEngine;

namespace FypGreybox
{
    [RequireComponent(typeof(CharacterController))]
    public class GreyboxMotor : MonoBehaviour
    {
        public GreyboxWorkspace workspace;
        public string lastPlatform;
        public bool externalDrive;
        CharacterController controller;
        TemplateRules rules;
        float vertical, coyote;
        public bool Grounded => controller != null && controller.isGrounded;
        void Awake() { controller = GetComponent<CharacterController>(); rules = TemplateRules.Load(); }
        void Update()
        {
            if (externalDrive || workspace == null || workspace.completed) return;
            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Tick(InputDirection(input), Input.GetKeyDown(KeyCode.Space), Time.deltaTime);
        }
        // This template's route runs along +Z. A fixed route-aligned camera and
        // stable world axes make W/Up forward, including during camera smoothing.
        public static Vector3 InputDirection(Vector2 input) => Vector3.ClampMagnitude(new Vector3(input.x, 0, input.y), 1);
        // Tests and human input drive exactly this CharacterController movement path.
        public void Tick(Vector3 direction, bool jump, float dt)
        {
            if (workspace == null || workspace.completed) return;
            dt = Mathf.Clamp(dt, 0, .05f);
            if (controller.isGrounded) { coyote = .10f; if (vertical < 0) vertical = -2f; }
            else coyote -= dt;
            if (jump && coyote > 0) { vertical = Mathf.Sqrt(-2 * rules.gravity * rules.jumpHeight); coyote = 0; }
            vertical += rules.gravity * dt;
            Vector3 velocity = Vector3.ClampMagnitude(new Vector3(direction.x, 0, direction.z), 1) * rules.moveSpeed;
            var collision = controller.Move((velocity + Vector3.up * vertical) * dt);
            if ((collision & CollisionFlags.Above) != 0 && vertical > 0) vertical = 0;
            if (controller.isGrounded && vertical <= 0 && Physics.Raycast(transform.position + Vector3.up * .2f, Vector3.down, out var hit, .5f))
            {
                var marker = hit.collider.GetComponent<PlatformMarker>();
                if (marker != null) workspace.Land(marker.id);
            }
            if (transform.position.y < -8f) workspace.Fall();
        }
        public void Teleport(Vector3 position)
        {
            controller.enabled = false; transform.position = position; controller.enabled = true;
            vertical = 0; coyote = 0;
        }
    }
}
