using UnityEngine;
[RequireComponent (typeof(Rigidbody))]
public class TreeFall : MonoBehaviour
{    
    private bool hasFallen = false;
    [Header("Fall Setings")]
    [SerializeField] private float m_fallForce = 5f; 
    [SerializeField] private Vector3 m_fallDirection = new Vector3(1, 0, 0); 
    [SerializeField] private Rigidbody m_rb;

    [Header("SFX")]
    [SerializeField] private AudioClip m_clipFall;
    [SerializeField] private AudioSource m_sourse;



    void Start()
    {       
        m_rb.isKinematic = true; 
    }

    private void Fall()
    {
        if (hasFallen) return;

        hasFallen = true;
        m_rb.isKinematic = false; 
       
        m_rb.AddForce(m_fallDirection * m_fallForce, ForceMode.Impulse);

        m_sourse.clip = m_clipFall;
        m_sourse.Play();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Robot")
        {
            Fall();
            Debug.Log("Fall");
        }
    }
}