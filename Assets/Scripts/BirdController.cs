using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{

    public bool haveKey = false;
    public float influence_num = 0;
    public float repulsive_radius = 0;
    public List<Vector3> velocity_list;
    public Vector2 ave_velocity = new Vector2(0, 0);
    public float repulsive_velocity = 0;
    public float timer = 0;
    public float frame_inteval = 0;
    public Vector2 target_v = new Vector2(0, 0);
    public Sprite bird_sprite;
    public Color dead_color;
    public bool is_dead = false;

    private AudioSource audio_source;
    public AudioClip dead_clip;
    public AudioClip shit_clip;
    // Start is called before the first frame update
    void Start() {
        audio_source = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (is_dead) {
            return;
        }

        //print(Mathf.Atan2(this.GetComponent<Rigidbody2D>().velocity.y, this.GetComponent<Rigidbody2D>().velocity.x));
    }


    public void Dead() {
        is_dead = true;
        audio_source.clip = dead_clip;
        audio_source.Play();
        this.GetComponent<SpriteRenderer>().color = dead_color;
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        this.GetComponent<Rigidbody2D>().gravityScale = 1;

        is_dead = true;
        this.GetComponent<SpriteRenderer>().color = dead_color;
        this.GetComponent<Rigidbody2D>().gravityScale = 1;

        for (int i = 0; i < FlockManager.instance.bird_list.Count; ++i) {
            if (FlockManager.instance.bird_list[i].GetComponent<BirdController>().is_dead == true) {
                this.GetComponent<CircleCollider2D>().enabled = false;
                FlockManager.instance.bird_list.RemoveAt(i);
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == Tags.Key) {
            Destroy(collision.gameObject);
            FlockManager.instance.haveKey = true;
        }
    }
    public void play_shit() {
        audio_source.clip = shit_clip;
        audio_source.Play();
    }
}