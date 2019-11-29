using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform lightning, friend_pos;
    public Transform player_pos;
    public Rigidbody rb;
    public GameObject light;
    public float go_forse, break_drag, back_force, ang_drag;
    public float rotation_speed;
    bool impact_was = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        float s = Mathf.Sqrt(((player_pos.position.x - friend_pos.position.x) * (player_pos.position.x - friend_pos.position.x)) +
            ((player_pos.position.z - friend_pos.position.z) * (player_pos.position.z - friend_pos.position.z))) / 2;
        if (s > 3) light.SetActive(false);
        else
        {
            light.SetActive(true);
            float l_x = (player_pos.position.x + friend_pos.position.x) / 2f;
            float l_y = (player_pos.position.y + friend_pos.position.y) / 2f;
            float l_z = (player_pos.position.z + friend_pos.position.z) / 2f;
            lightning.SetPositionAndRotation(new Vector3(l_x, l_y, l_z),
                Quaternion.AngleAxis(90, new Vector3(
                    l_z - friend_pos.position.z, 0, friend_pos.position.x - l_x)));
            lightning.localScale = new Vector3(0.05f, s, 0.05f);
        }
        */
        if (Input.GetKey("w") && !Input.GetKey("s") && !Input.GetKey("space"))
        {
            if (rb.velocity.z < 0)
                rb.drag = break_drag;
            rb.AddRelativeForce(0, 0, go_forse * Time.deltaTime);
        }
        if (Input.GetKey("s") && !Input.GetKey("w") && !Input.GetKey("space"))
        {
            if (rb.velocity.z > 0)
                rb.drag = break_drag;
            rb.AddRelativeForce(0, 0, -back_force * Time.deltaTime);
        }
        if (Input.GetKey("space") && !Input.GetKey("s") && !Input.GetKey("w"))
        {
            rb.drag = break_drag;
            rb.angularDrag = ang_drag;
        }
        else
        {
            rb.drag = 0;
            rb.angularDrag = 1;
        }
        if (Input.GetKey("a") && !Input.GetKey("d"))
            rb.angularVelocity = new Vector3(0, -rotation_speed, 0);
            //rb.MoveRotation(new Quaternion(rb.rotation.x, rb.rotation.y - (rotation_speed * Time.deltaTime), rb.rotation.z, rb.rotation.w));
            //rb.rotation = new Quaternion(rb.rotation.x, rb.rotation.y - (rotation_speed * Time.deltaTime), rb.rotation.z, rb.rotation.w);
        if (Input.GetKey("d") && !Input.GetKey("a"))
            rb.angularVelocity = new Vector3(0, rotation_speed, 0);
        /*
        if (!impact_was)
        {
            float cor_z = player_pos.position.z;
            if (cor_z >= 79)
                impact_was = true;
            else
            {
                if (cor_z < break_on)
                    rb.AddForce(0, 0, 1000 * Time.deltaTime);
                else
                {
                    if (rb.velocity.z > 0)
                        rb.AddForce(0, 0, -break_forse * Time.deltaTime);
                }
            }
        }
        */
    }
}
