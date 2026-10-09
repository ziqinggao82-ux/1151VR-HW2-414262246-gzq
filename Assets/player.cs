using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 8f;

    Rigidbody2D rb;
    AudioSource audioSource;

    [SerializeField] AudioClip moveSound;
    [SerializeField] AudioClip hitSound1;
    [SerializeField] AudioClip hitSound2;
    [SerializeField] AudioClip finishSound;

    bool isWaiting = false;

    // 起點
    Vector2 startPosition = new Vector2(-8, -2);

    // 終點
    Vector2 endPosition = new Vector2(10, 1);

    // 方向陣列
    Vector2[] directions =
    {
        new Vector2(0, 1),
        new Vector2(0, -1),
        new Vector2(-1, 0),
        new Vector2(1, 0)
    };

    void Start()
    {
        transform.position = startPosition;

        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 等待期間不能操作
        if (isWaiting)
            return;

        // 左右移動
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(directions[2] * moveSpeed * Time.deltaTime);
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(directions[3] * moveSpeed * Time.deltaTime);
            transform.localScale = new Vector3(1, 1, 1);
        }

        // 跳
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 下蹲
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.localScale = new Vector3(transform.localScale.x, 0.5f, 1);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, 1, 1);
        }

        // 限制邊界
        float x = Mathf.Clamp(transform.position.x, -9.5f, 10.5f);
        float y = Mathf.Clamp(transform.position.y, -3.8f, 3.8f);

        transform.position = new Vector2(x, y);

        // 到達終點
        if (Vector2.Distance(transform.position, endPosition) < 1.5f)
        {
            StartCoroutine(ReachFinish());
        }

        // 移動音效
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = moveSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }

    // 碰到障礙物
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isWaiting)
            return;

        if (collision.gameObject.CompareTag("Obstacle1"))
        {
            StartCoroutine(HitObstacle(hitSound1));
        }

        if (collision.gameObject.CompareTag("Obstacle2"))
        {
            StartCoroutine(HitObstacle(hitSound2));
        }
    }

    // 撞到障礙物
    IEnumerator HitObstacle(AudioClip hitSound)
    {
        isWaiting = true;

        // 停止移動音效
        audioSource.Stop();

        // 播放撞擊音效
        audioSource.PlayOneShot(hitSound);

        // 停在原地1秒
        yield return new WaitForSeconds(1f);

        // 1秒後回到起點
        transform.position = startPosition;

        // 清除原本的速度
        rb.linearVelocity = Vector2.zero;

        isWaiting = false;
    }

    // 到達終點
    IEnumerator ReachFinish()
    {
        isWaiting = true;

        // 停止移動音效
        audioSource.Stop();

        // 播放終點音效
        audioSource.PlayOneShot(finishSound);

        // 停在終點1秒
        yield return new WaitForSeconds(1f);

        // 1秒後回到起點
        transform.position = startPosition;

        // 清除速度
        rb.linearVelocity = Vector2.zero;

        isWaiting = false;
    }
}