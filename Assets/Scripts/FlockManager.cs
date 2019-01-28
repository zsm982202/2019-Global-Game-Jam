using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{

    public static FlockManager instance;
    public bool haveKey = false;
    public float timer = 0f;
    public float interval_time = 0f;
    public int birds_num = 0;
    public float outter_radius = 0;
    public float min_velocity = 0;
    public float max_velocity = 0;
    public float bird_speed = 0f;
    public float influence_num = 0;
    public float repulsive_radius = 0;
    public float repulsive_speed = 0;
    public float to_flock_center_speed = 0;
    public List<Vector2> target_velocity_list;
    public List<Transform> bird_list;
    public Transform bird_prefab;
    private Transform flock_transform;
    public Transform shit_prefabs;
    public Color grenn_color;
    public SpriteRenderer circle_render;
    private void Awake() {
        instance = this;
        haveKey = false;
        flock_transform = this.GetComponent<Transform>();
    }
    // Start is called before the first frame update

    void Start() {
        float tmp_len = 0;
        float tmp_radian = 0;
        for (int i = 0; i < birds_num; ++i) {
            tmp_len = Random.Range(0, outter_radius);
            tmp_radian = Random.Range(0, 2f * Mathf.PI);
            Vector3 tmp_offset = new Vector3(Mathf.Cos(tmp_radian) * tmp_len, Mathf.Sin(tmp_radian) * tmp_len, 0);

            float tmp_rotation_radian = Random.Range(0f, 2f * Mathf.PI);
            Transform tmp_bird = Instantiate(bird_prefab, flock_transform.position + tmp_offset, Quaternion.Euler(new Vector3(0, 0, 180 / Mathf.PI * tmp_rotation_radian - 90)));
            float tmp_velocity = Random.Range(min_velocity, max_velocity);
            Vector3 tmp_rotation = new Vector2(Mathf.Cos(tmp_rotation_radian), Mathf.Sin(tmp_rotation_radian));
            target_velocity_list.Add(tmp_rotation * tmp_velocity);
            tmp_bird.GetComponent<Rigidbody2D>().velocity = target_velocity_list[i];
            tmp_bird.SetParent(this.transform);
            bird_list.Add(tmp_bird);
        }
    }

    // Update is called once per frame
    void Update() {
        if (bird_list.Count == 0) {
            UIManager.instance.human_win = true;
        }
        timer += Time.deltaTime;
        Vector2 new_velocity = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W)) {
            new_velocity += new Vector2(0, bird_speed);
        }
        if (Input.GetKey(KeyCode.S)) {
            new_velocity += new Vector2(0, -bird_speed);

        }
        if (Input.GetKey(KeyCode.A)) {
            new_velocity += new Vector2(-1 * bird_speed, 0);

        }
        if (Input.GetKey(KeyCode.D)) {
            new_velocity += new Vector2(bird_speed, 0);
        }
        this.GetComponent<Rigidbody2D>().velocity = new_velocity;
        if (Input.GetKeyDown(KeyCode.W)) {
            for (int i = 0; i < target_velocity_list.Count; ++i) {
                target_velocity_list[i] += new Vector2(0, bird_speed);
            }
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            for (int i = 0; i < target_velocity_list.Count; ++i) {
                target_velocity_list[i] += new Vector2(0, -bird_speed);
            }
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            for (int i = 0; i < target_velocity_list.Count; ++i) {
                target_velocity_list[i] += new Vector2(-bird_speed, 0);
            }
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            for (int i = 0; i < target_velocity_list.Count; ++i) {
                target_velocity_list[i] += new Vector2(bird_speed, 0);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (bird_list.Count != 0) {
                for (int i = 0; i < bird_list.Count / 3 + 1; ++i) {
                    int random_index = Random.Range(0, bird_list.Count);
                    bird_list[random_index].GetComponent<BirdController>().play_shit();
                    Instantiate(shit_prefabs, bird_list[random_index].position, Quaternion.identity);
                }
            }


        }



        if (timer > interval_time) {
            timer = 0;
            float[,] dis_matrix = new float[bird_list.Count, bird_list.Count];
            for (int i = 0; i < bird_list.Count; ++i) {
                for (int j = 0; j < bird_list.Count; ++j) {
                    float tmp_dis = Vector3.Distance(bird_list[i].position, bird_list[j].position);
                    dis_matrix[i, j] = tmp_dis;
                    dis_matrix[j, i] = tmp_dis;
                }
            }


            for (int i = 0; i < bird_list.Count; ++i) {
                Vector2 repulsive_velocity = new Vector2(0, 0);
                List<Transform> tmp_list = new List<Transform>(this.bird_list);

                for (int j = 0; j < tmp_list.Count; ++j) {
                    if (i == j) {
                        continue;
                    }
                    if (dis_matrix[i, j] < repulsive_radius) {
                        Vector2 tmp_dir = (bird_list[i].position - tmp_list[j].position).normalized;
                        repulsive_velocity += tmp_dir * repulsive_speed * 1 / Mathf.Pow(dis_matrix[i, j], 2);
                    }
                }

                Vector3 center_pos = new Vector3(0, 0, 0);
                Vector2 ave_velocity = target_velocity_list[i];
                float found_num = 0;
                for (int j = 0; j < influence_num; ++j) {
                    if (found_num >= influence_num || influence_num > bird_list.Count) {
                        break;
                    }
                    float min_dis = Mathf.Infinity;
                    int min_index = 0;
                    tmp_list = new List<Transform>(this.bird_list);
                    for (int k = 0; k < tmp_list.Count; ++k) {
                        if (tmp_list[k] == null) {
                            continue;
                        }
                        if (dis_matrix[j, k] < min_dis) {
                            min_dis = dis_matrix[j, k];
                            min_index = k;
                        }
                    }
                    //print(min_index);
                    //print(tmp_list[min_index].position);
                    center_pos += tmp_list[min_index].position;

                    ave_velocity += tmp_list[min_index].GetComponent<Rigidbody2D>().velocity;
                    tmp_list[min_index] = null;
                    found_num++;
                }
                ave_velocity /= influence_num;
                center_pos /= influence_num;
                Vector2 center_dir = (center_pos - bird_list[i].position).normalized;

                Vector2 to_flock_center_dir = this.transform.position - bird_list[i].position;
                float to_flock_center_dis = to_flock_center_dir.magnitude;
                if (to_flock_center_dis > 0.5f * outter_radius) {
                    to_flock_center_dis = to_flock_center_dir.magnitude;
                } else {
                    to_flock_center_dis = 0;
                }

                target_velocity_list[i] = (center_dir + ave_velocity.normalized).normalized * ave_velocity.magnitude +
                                          repulsive_velocity +
                                          to_flock_center_dir * to_flock_center_dis * to_flock_center_speed;
                print("cen+ave" + ((center_dir + ave_velocity.normalized).normalized * ave_velocity.magnitude).magnitude / target_velocity_list[i].magnitude);
                print("rep" + repulsive_velocity / target_velocity_list[i].magnitude);
                print("fock" + to_flock_center_dir * to_flock_center_dis * to_flock_center_dis * to_flock_center_speed / target_velocity_list[i].magnitude);
                if (target_velocity_list[i].magnitude > max_velocity) {
                    float ratio = max_velocity / target_velocity_list[i].magnitude;
                    target_velocity_list[i] *= ratio;
                }
            }
        }

        for (int i = 0; i < bird_list.Count; ++i) {
            bird_list[i].GetComponent<Rigidbody2D>().velocity = Vector2.Lerp(bird_list[i].GetComponent<Rigidbody2D>().velocity, target_velocity_list[i], 0.07f);
            bird_list[i].rotation = Quaternion.Euler(new Vector3(0, 0,
               180 * Mathf.Atan2(bird_list[i].GetComponent<Rigidbody2D>().velocity.y, bird_list[i].GetComponent<Rigidbody2D>().velocity.x) / Mathf.PI - 90f));
        }

        if (haveKey) {
            this.circle_render.color = grenn_color;

        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == Tags.Door) {
            if (haveKey) {
                this.circle_render.color = grenn_color;
                Destroy(collision.gameObject);
                UIManager.instance.ge_win = true;
            }
        }
    }
}