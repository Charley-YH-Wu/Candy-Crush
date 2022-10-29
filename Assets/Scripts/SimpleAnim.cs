using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnim : MonoBehaviour
{
    public bool m_randomRotate = true;
    public float m_minSpeed = 0.8f;
    public float m_maxSpeed = 1.2f;
    public float m_minScale = 0.8f;
    public float m_maxScale = 1.2f;
    public float m_minVolume = 0.8f;
    public float m_maxVolume = 1.2f;
    public float m_minPitch = 0.8f;
    public float m_maxPitch = 1.2f;

    Animator m_anim;
    AudioSource m_audio;

    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_audio = GetComponent<AudioSource>();
        if (null != m_anim)
        {
            m_anim.speed = Random.Range(m_minSpeed, m_maxSpeed);
        }
        transform.localScale = Random.Range(m_minScale, m_maxScale) * Vector3.one;
        if (m_randomRotate)
        {
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        }
        {   // TODO randomize the volume and pitch of m_audio if there is one
            if (m_audio != null){
                m_audio.volume = Random.Range(m_minVolume,m_maxVolume);
                m_audio.pitch = Random.Range(m_minPitch,m_maxPitch);
                m_audio.Play();
            }
        }
        StartCoroutine(DeleteWhenDone());
    }

    IEnumerator DeleteWhenDone()
    {
        if (null != m_anim)
        {
            var state = m_anim.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(state.length * state.speed);
        }
        {   // TODO yield until the m_audio is finished playing
            if (null != m_audio){
                while (m_audio.isPlaying){
                    yield return null;
                }
            }
        }
        Destroy(gameObject);
    }
}
