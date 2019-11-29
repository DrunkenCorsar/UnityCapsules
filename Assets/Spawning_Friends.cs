using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawning_Friends : MonoBehaviour
{
    public Material friend_material, enemy_material, friend_light_material, enemy_light_material, damaging_light_material,
        healing_material, hat_material;
    public static int Started_Friend_Count = 10;
    public float force_tick_time = 0.5f;
    public int force_moving = 250, part_min = 10;

    //public Rigidbody player_rigidbody;
    public Transform player_transform;
    float time_count = 0;
    bool Is_Added = false;
    Rigidbody[] friends_rig = new Rigidbody[Started_Friend_Count];
    Rigidbody[] enemies_rig = new Rigidbody[Started_Friend_Count];
    GameObject[] friends = new GameObject[Started_Friend_Count];
    GameObject[] enemies = new GameObject[Started_Friend_Count];
    GameObject[] friends_hat = new GameObject[Started_Friend_Count];
    GameObject[] enemies_hat = new GameObject[Started_Friend_Count];
    GameObject[,] friend_friend_light = new GameObject[Started_Friend_Count, Started_Friend_Count];
    GameObject[,] enemy_enemy_light = new GameObject[Started_Friend_Count, Started_Friend_Count];
    GameObject[,] friend_enemy_light = new GameObject[Started_Friend_Count, Started_Friend_Count];
    GameObject[] healing_light = new GameObject[2 * Started_Friend_Count];
    //bool[,] friend_friend_is_light = new bool[Started_Friend_Count, Started_Friend_Count];
    int[] friends_forces_x = new int[Started_Friend_Count];
    int[] friends_forces_z = new int[Started_Friend_Count];
    int[] enemies_forces_x = new int[Started_Friend_Count];
    int[] enemies_forces_z = new int[Started_Friend_Count];
    int added_count = 0;

    // Variables for calculating HP
    float[] enemies_hp = new float[Started_Friend_Count];
    float[] friends_hp = new float[Started_Friend_Count];
    bool[] enemies_alive = new bool[Started_Friend_Count];
    bool[] friends_alive = new bool[Started_Friend_Count];
    const float
        friends_healing_speed = 0.5f,
        enemies_hitting_speed = 1.0f,
        healer_healing_speed = 1.9f,
        started_hp = 40.0f,
        maximum_hp = 60.0f,
        min_distance = 2.0f;


    // Start is called before the first frame update
    void Start()
    {
        // Creating HP scales
        for (int i = 0; i < Started_Friend_Count; i++)
        {
            enemies_hp[i] = started_hp;
            enemies_alive[i] = true;
            friends_hp[i] = started_hp;
            friends_alive[i] = true;
        }

        // Creating array of friend-friend lightnings
        if (Started_Friend_Count > 1)
            for (int i = 0; i < Started_Friend_Count - 1; i++)
            {
                for (int j = i + 1; j < Started_Friend_Count; j++)
                {
                    GameObject light = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    light.transform.localScale = new Vector3(0.05f, 1f, 0.05f);
                    light.transform.position = new Vector3(0, -5, 0);
                    MeshRenderer mr = light.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
                    mr.material = friend_light_material;
                    Collider cl = light.GetComponent(typeof(Collider)) as Collider;
                    Destroy(cl);
                    Light ll = light.AddComponent(typeof(Light)) as Light;
                    ll.range = 30;
                    ll.color = Color.red;
                    ll.intensity = 2;
                    light.SetActive(false);
                    friend_friend_light[i, j] = light;
                }
            }

        // Creating array of healing lightnings
        for (int i = 0; i < 2 * Started_Friend_Count; i++)
        {
            GameObject light = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            light.transform.localScale = new Vector3(0.05f, 1f, 0.05f);
            light.transform.position = new Vector3(0, -5, 0);
            MeshRenderer mr = light.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            mr.material = healing_material;
            Collider cl = light.GetComponent(typeof(Collider)) as Collider;
            Destroy(cl);
            Light ll = light.AddComponent(typeof(Light)) as Light;
            ll.range = 30;
            ll.color = Color.green;
            ll.intensity = 2;
            light.SetActive(false);
            healing_light[i] = light;
        }

        // Creating array of enemy-enemy lightnings
        if (Started_Friend_Count > 1)
            for (int i = 0; i < Started_Friend_Count - 1; i++)
            {
                for (int j = i + 1; j < Started_Friend_Count; j++)
                {
                    GameObject light = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    light.transform.localScale = new Vector3(0.05f, 1f, 0.05f);
                    light.transform.position = new Vector3(0, -5, 0);
                    MeshRenderer mr = light.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
                    mr.material = enemy_light_material;
                    Collider cl = light.GetComponent(typeof(Collider)) as Collider;
                    Destroy(cl);
                    Light ll = light.AddComponent(typeof(Light)) as Light;
                    ll.range = 30;
                    ll.color = Color.blue;
                    ll.intensity = 2;
                    light.SetActive(false);
                    enemy_enemy_light[i, j] = light;
                }
            }

        // Creating array of friend-enemy lightnings
        for (int i = 0; i < Started_Friend_Count; i++)
        {
            for (int j = 0; j < Started_Friend_Count; j++)
            {
                GameObject light = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                light.transform.localScale = new Vector3(0.05f, 1f, 0.05f);
                light.transform.position = new Vector3(0, -5, 0);
                MeshRenderer mr = light.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
                mr.material = damaging_light_material;
                Collider cl = light.GetComponent(typeof(Collider)) as Collider;
                Destroy(cl);
                Light ll = light.AddComponent(typeof(Light)) as Light;
                ll.range = 30;
                ll.color = Color.magenta;
                ll.intensity = 2;
                light.SetActive(false);
                friend_enemy_light[i, j] = light;
            }
        }

        // Random array for randomly spawning players
        System.Random rand = new System.Random();

        // Spawning friends
        for (int i = 0; i < Started_Friend_Count; i++)
        {
            float x = rand.Next(-23, 24), z = rand.Next(-23,1);
            GameObject friend = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            friend.transform.localScale = new Vector3(0.5f, 1, 0.5f);
            friend.transform.position = new Vector3(x, 1f, z);
            Rigidbody rb = friend.AddComponent(typeof(Rigidbody)) as Rigidbody;
            rb.angularDrag = 0.6f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            SphereCollider sc = friend.GetComponent(typeof(SphereCollider)) as SphereCollider;
            sc.radius = 0.25f;
            CapsuleCollider cc = friend.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider;
            cc.radius = 0.4f;
            MeshRenderer mr = friend.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            mr.material = friend_material;
            friends_rig[i] = rb;
            friends[i] = friend;
        }

        // Spawning enemies
        for (int i = 0; i < Started_Friend_Count; i++)
        {
            float x = rand.Next(-23, 24), z = rand.Next(0, 24);
            GameObject friend = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            friend.transform.localScale = new Vector3(0.5f, 1, 0.5f);
            friend.transform.position = new Vector3(x, 1f, z);
            Rigidbody rb = friend.AddComponent(typeof(Rigidbody)) as Rigidbody;
            rb.angularDrag = 0.6f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            SphereCollider sc = friend.GetComponent(typeof(SphereCollider)) as SphereCollider;
            sc.radius = 0.25f;
            CapsuleCollider cc = friend.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider;
            cc.radius = 0.4f;
            MeshRenderer mr = friend.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            mr.material = enemy_material;
            enemies_rig[i] = rb;
            enemies[i] = friend;
        }

        // Spawning hats for friends
        for (int i = 0; i < Started_Friend_Count; i++)
        {
            float x = friends[i].transform.position.x, z = friends[i].transform.position.z;
            GameObject hat = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hat.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            hat.transform.position = new Vector3(x, 1.9f, z);
            SphereCollider sc = hat.GetComponent(typeof(SphereCollider)) as SphereCollider;
            sc.radius = 0.5f;
            MeshRenderer mr = hat.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            mr.material = hat_material;
            friends_hat[i] = hat;
        }

        // Spawning hats for friends
        for (int i = 0; i < Started_Friend_Count; i++)
        {
            float x = enemies[i].transform.position.x, z = enemies[i].transform.position.z;
            GameObject hat = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hat.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            hat.transform.position = new Vector3(x, 1.9f, z);
            SphereCollider sc = hat.GetComponent(typeof(SphereCollider)) as SphereCollider;
            sc.radius = 0.5f;
            MeshRenderer mr = hat.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            mr.material = hat_material;
            enemies_hat[i] = hat;
        }

        // TODO
        time_count = force_tick_time;
    }

    // Update is called once per frame
    void Update()
    {
        // AI movement
        System.Random rand = new System.Random();
        time_count += Time.deltaTime;
        if (time_count >= force_tick_time)
        {
            time_count -= force_tick_time;
            for (int i = 0; i < Started_Friend_Count; i++)
            {
                if (friends_alive[i])
                {
                    if (friends[i].transform.position.x <= 0) friends_forces_x[i] = rand.Next(force_moving / part_min, force_moving + 1);
                    else friends_forces_x[i] = rand.Next(-force_moving, -force_moving / part_min + 1);
                    if (friends[i].transform.position.z <= 0) friends_forces_z[i] = rand.Next(force_moving / part_min, force_moving + 1);
                    else friends_forces_z[i] = rand.Next(-force_moving, -force_moving / part_min + 1);
                }
                if (enemies_alive[i])
                {
                    if (enemies[i].transform.position.x <= 0) enemies_forces_x[i] = rand.Next(force_moving / part_min, force_moving + 1);
                    else enemies_forces_x[i] = rand.Next(-force_moving, -force_moving / part_min + 1);
                    if (enemies[i].transform.position.z <= 0) enemies_forces_z[i] = rand.Next(force_moving / part_min, force_moving + 1);
                    else enemies_forces_z[i] = rand.Next(-force_moving, -force_moving / part_min + 1);
                }
            }
        }
        for (int i = 0; i < Started_Friend_Count; i++)
        {
            friends_rig[i].AddRelativeForce(friends_forces_x[i] * Time.deltaTime, 0, friends_forces_z[i] * Time.deltaTime);
            enemies_rig[i].AddRelativeForce(enemies_forces_x[i] * Time.deltaTime, 0, enemies_forces_z[i] * Time.deltaTime);
            if (friends[i].transform.position.y < 0.99f) KillUnit(true, i);
            if (enemies[i].transform.position.y < 0.99f) KillUnit(false, i);
        }

        // Hats movement
        for (int i = 0; i < Started_Friend_Count; i++)
        {
            friends_hat[i].transform.position = new Vector3(friends[i].transform.position.x, 1.9f, friends[i].transform.position.z);
            enemies_hat[i].transform.position = new Vector3(enemies[i].transform.position.x, 1.9f, enemies[i].transform.position.z);
            ///MeshRenderer mr_friend = friends_hat[i].GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            ///mr_friend.material.color = new Color(0.5f, 0.5f, 0.5f);
        }

        // Spawning friend's lightnings 
        if (Started_Friend_Count > 1)
            for (int i = 0; i < Started_Friend_Count - 1; i++)
            {
                for (int j = i + 1; j < Started_Friend_Count; j++)
                {
                    if (friends_alive[i] && friends_alive[j])
                    {
                        Transform player_pos = friends[i].transform, friend_pos = friends[j].transform;
                        GameObject light = friend_friend_light[i, j];
                        Transform lightning = light.transform;
                        float s = Mathf.Sqrt(((player_pos.position.x - friend_pos.position.x) * (player_pos.position.x - friend_pos.position.x)) +
                        ((player_pos.position.z - friend_pos.position.z) * (player_pos.position.z - friend_pos.position.z))) / 2;
                        if (s > min_distance)
                        {
                            light.SetActive(false);
                            friend_friend_light[i, j] = light;
                        }
                        else
                        {
                            // Case when RED & RED spheres are close to each other
                            light.SetActive(true);
                            float l_x = (player_pos.position.x + friend_pos.position.x) / 2f;
                            float l_y = (player_pos.position.y + friend_pos.position.y) / 2f;
                            float l_z = (player_pos.position.z + friend_pos.position.z) / 2f;
                            lightning.SetPositionAndRotation(new Vector3(l_x, l_y, l_z),
                                Quaternion.AngleAxis(90, new Vector3(
                                    l_z - friend_pos.position.z, 0, friend_pos.position.x - l_x)));
                            lightning.localScale = new Vector3(0.05f, s, 0.05f);
                            // Healing friends
                            friends_hp[i] += friends_healing_speed * Time.deltaTime;
                            friends_hp[j] += friends_healing_speed * Time.deltaTime;
                        }
                    }
                    else
                    {
                        friend_friend_light[i, j].SetActive(false);
                    }
                }
            }

        for (int i = 0; i < Started_Friend_Count; i++)
            for (int j = 0; j < Started_Friend_Count; j++)
            {
                if (friends_alive[j] && enemies_alive[i])
                {
                    Transform player_pos = enemies[i].transform, friend_pos = friends[j].transform;
                    GameObject light = friend_enemy_light[i, j];
                    Transform lightning = light.transform;
                    float s = Mathf.Sqrt(((player_pos.position.x - friend_pos.position.x) * (player_pos.position.x - friend_pos.position.x)) +
                    ((player_pos.position.z - friend_pos.position.z) * (player_pos.position.z - friend_pos.position.z))) / 2;
                    if (s > min_distance)
                    {
                        light.SetActive(false);
                        friend_enemy_light[i, j] = light;
                    }
                    else
                    {
                        // Case when RED & BLUE spheres are close to each other
                        light.SetActive(true);
                        float l_x = (player_pos.position.x + friend_pos.position.x) / 2f;
                        float l_y = (player_pos.position.y + friend_pos.position.y) / 2f;
                        float l_z = (player_pos.position.z + friend_pos.position.z) / 2f;
                        lightning.SetPositionAndRotation(new Vector3(l_x, l_y, l_z),
                            Quaternion.AngleAxis(90, new Vector3(
                                l_z - friend_pos.position.z, 0, friend_pos.position.x - l_x)));
                        lightning.localScale = new Vector3(0.05f, s, 0.05f);
                        // Hitting players
                        friends_hp[j] -= enemies_hitting_speed * Time.deltaTime;
                        enemies_hp[i] -= enemies_hitting_speed * Time.deltaTime;
                    }
                }
                else
                {
                    friend_enemy_light[i, j].SetActive(false);
                }
            }
        // Spawning enemies' lightnings
        
        if (Started_Friend_Count > 1)
            for (int i = 0; i < Started_Friend_Count - 1; i++)
            {
                for (int j = i + 1; j < Started_Friend_Count; j++)
                {
                    if (enemies_alive[i] && enemies_alive[j])
                    {
                        Transform player_pos = enemies[i].transform, friend_pos = enemies[j].transform;
                        GameObject light = enemy_enemy_light[i, j];
                        Transform lightning = light.transform;
                        float s = Mathf.Sqrt(((player_pos.position.x - friend_pos.position.x) * (player_pos.position.x - friend_pos.position.x)) +
                        ((player_pos.position.z - friend_pos.position.z) * (player_pos.position.z - friend_pos.position.z))) / 2;
                        if (s > min_distance)
                        {
                            light.SetActive(false);
                            enemy_enemy_light[i, j] = light;
                        }
                        else
                        {
                            // Case when BLUE & BLUE spheres are close to each other
                            light.SetActive(true);
                            float l_x = (player_pos.position.x + friend_pos.position.x) / 2f;
                            float l_y = (player_pos.position.y + friend_pos.position.y) / 2f;
                            float l_z = (player_pos.position.z + friend_pos.position.z) / 2f;
                            lightning.SetPositionAndRotation(new Vector3(l_x, l_y, l_z),
                                Quaternion.AngleAxis(90, new Vector3(
                                    l_z - friend_pos.position.z, 0, friend_pos.position.x - l_x)));
                            lightning.localScale = new Vector3(0.05f, s, 0.05f);
                            // Healing blue players
                            enemies_hp[i] += friends_healing_speed * Time.deltaTime;
                            enemies_hp[j] += friends_healing_speed * Time.deltaTime;
                        }
                    }
                    else
                    {
                        enemy_enemy_light[i, j].SetActive(false);
                    }
                }
            } 

        // Spawning healing lightnings for friends
        for (int i = 0; i < Started_Friend_Count; i++)
        {
            if (friends_alive[i])
            {
                Transform player_pos = player_transform, friend_pos = friends[i].transform;
                GameObject light = healing_light[i];
                Transform lightning = light.transform;
                float s = Mathf.Sqrt(((player_pos.position.x - friend_pos.position.x) * (player_pos.position.x - friend_pos.position.x)) +
                ((player_pos.position.z - friend_pos.position.z) * (player_pos.position.z - friend_pos.position.z))) / 2;
                if (s > min_distance)
                {
                    light.SetActive(false);
                    healing_light[i] = light;
                }
                else
                {
                    // Case when GREEN & RED spheres are close to each other
                    light.SetActive(true);
                    float l_x = (player_pos.position.x + friend_pos.position.x) / 2f;
                    float l_y = (player_pos.position.y + friend_pos.position.y) / 2f;
                    float l_z = (player_pos.position.z + friend_pos.position.z) / 2f;
                    lightning.SetPositionAndRotation(new Vector3(l_x, l_y, l_z),
                        Quaternion.AngleAxis(90, new Vector3(
                            l_z - friend_pos.position.z, 0, friend_pos.position.x - l_x)));
                    lightning.localScale = new Vector3(0.05f, s, 0.05f);
                    // Healing red players
                    friends_hp[i] += healer_healing_speed * Time.deltaTime;
                }
            }
            else
            {
                healing_light[i].SetActive(false);
            }
        }

        // Spawning healing lightnings for enemies
        for (int i = Started_Friend_Count; i < 2 * Started_Friend_Count; i++)
        {
            if (enemies_alive[i - Started_Friend_Count])
            {
                Transform player_pos = player_transform, friend_pos = enemies[i - Started_Friend_Count].transform;
                GameObject light = healing_light[i];
                Transform lightning = light.transform;
                float s = Mathf.Sqrt(((player_pos.position.x - friend_pos.position.x) * (player_pos.position.x - friend_pos.position.x)) +
                ((player_pos.position.z - friend_pos.position.z) * (player_pos.position.z - friend_pos.position.z))) / 2;
                if (s > min_distance)
                {
                    light.SetActive(false);
                    healing_light[i] = light;
                }
                else
                {
                    // Case when GREEN & BLUE spheres are close to each other
                    light.SetActive(true);
                    float l_x = (player_pos.position.x + friend_pos.position.x) / 2f;
                    float l_y = (player_pos.position.y + friend_pos.position.y) / 2f;
                    float l_z = (player_pos.position.z + friend_pos.position.z) / 2f;
                    lightning.SetPositionAndRotation(new Vector3(l_x, l_y, l_z),
                        Quaternion.AngleAxis(90, new Vector3(
                            l_z - friend_pos.position.z, 0, friend_pos.position.x - l_x)));
                    lightning.localScale = new Vector3(0.05f, s, 0.05f);
                    // Healing blue players
                    enemies_hp[i - Started_Friend_Count] += healer_healing_speed;
                }
            }
            else
            {
                healing_light[i].SetActive(false);
            }
        }

        // Manipulating with HPs
        for (int i = 0; i < Started_Friend_Count; i++)
        {
            // Controlling if HPs are in scale
            if (friends_hp[i] < 0) KillUnit(true, i);
            if (friends_hp[i] > maximum_hp) friends_hp[i] = maximum_hp;
            if (enemies_hp[i] < 0) KillUnit(false, i);
            if (enemies_hp[i] > maximum_hp) enemies_hp[i] = maximum_hp;
            // Painting hats
            float hat_color = 0;
            hat_color = friends_hp[i] / maximum_hp;
            MeshRenderer mr_friend = friends_hat[i].GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            mr_friend.material.color = new Color(hat_color, hat_color, hat_color);
            ///Debug.Log("FriendHP = " + friends_hp[i] + " Hat = " + hat_color);
            hat_color = enemies_hp[i] / maximum_hp;
            MeshRenderer mr_enemy = enemies_hat[i].GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            mr_enemy.material.color = new Color(hat_color, hat_color, hat_color);
            ///Debug.Log("EnemyHP = " + enemies_hp[i] + " Hat = " + hat_color);
        }

        // TODO
    }

    void KillUnit(bool isfriend, int num)
    {
        if (isfriend)
        {
            friends_hp[num] = 0;
            friends_alive[num] = false;
            MeshRenderer mr = friends[num].GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            mr.material.color = new Color(0, 0, 0);
            friends_forces_x[num] = 0;
            friends_forces_z[num] = 0;
        }
        else
        {
            enemies_hp[num] = 0;
            enemies_alive[num] = false;
            MeshRenderer mr = enemies[num].GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            mr.material.color = new Color(0, 0, 0);
            enemies_forces_x[num] = 0;
            enemies_forces_z[num] = 0;
        }
    }
}
