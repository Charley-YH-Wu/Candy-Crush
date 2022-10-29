using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Gem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    // TODO Add pointer down and pointer up handlers
    // TODO add drag handlers
{
    public int m_gemType;
    public GameObject m_popAnim;
    public float m_gravity = 10.0f;
    public float m_maxSpeed = 100.0f;

    GemGrid m_grid;
    int m_x, m_y;
    Vector2 m_targetPos;
    float m_speed = 0.0f;
    bool m_isFalling = false;
    bool m_isBroken = false;
    bool m_isDragging = false;

    RectTransform m_rect;
    Vector2 m_dragDelta;
    Animator m_anim;

    const float s_swipeDist = 25.0f;

    public void SetSlot(GemGrid grid, int x, int y)
    {
        m_grid = grid;
        m_x = x;
        m_y = y;
        m_targetPos = m_grid.GetGemPos(m_x, m_y);
        if (false == m_isFalling)
        {
            if (null != m_anim)
            {
                m_anim.SetTrigger("OnFall");
                m_anim.speed = Random.Range(0.8f, 1.4f);
            }
        }
        m_isFalling = true;
    }

    public bool IsFalling()
    {
        return m_isFalling;
    }

    public void BreakGem()
    {
        if (false == m_isBroken)
        {
            m_isBroken = true;
            if (null != m_popAnim)
            {
                GameObject popObj = Instantiate(m_popAnim, transform.parent);
                popObj.transform.localPosition = transform.localPosition;
            }
            m_grid.BreakGem(m_x, m_y);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_rect = GetComponent<RectTransform>();
        m_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isFalling)
        {
            Vector2 pos = m_rect.anchoredPosition;
            m_speed += m_gravity * Time.deltaTime;
            m_speed = Mathf.Min(m_speed, m_maxSpeed);
            Vector2 delta = m_targetPos - pos;
            float len = delta.magnitude;
            if (len <= m_speed * Time.deltaTime)
            {   // you've arrived
                pos = m_targetPos;
                m_isFalling = false;
                m_speed = 0.0f;
                if (null != m_anim)
                {
                    m_anim.SetTrigger("OnLand");
                }
            }
            else
            { 
                pos += m_speed * Time.deltaTime / len * delta;
            }
            m_rect.anchoredPosition = pos;
        }
    }

    // TODO OnPointerDown
    // If the grid is animating, don't do anything
    // If the grid is still, set the bool "Touched" on the animator to true
    public void OnPointerDown(PointerEventData eventData){
        m_anim.SetBool("Touched", true);

    }

    // TODO OnPointerUp
    // Set the "Touched" on the animator to false
    public void OnPointerUp(PointerEventData eventData){
        m_anim.SetBool("Touched", false);
    }

    // // TODO OnBeginDrag
    // // Set m_isDragging true
    // // Reset m_dragDelta to zero
    public void OnBeginDrag(PointerEventData eventData){
        m_isDragging = true;
        m_dragDelta = Vector2.zero;
    }

    // // TODO OnEndDrag
    // // Set m_isDragging to false
    // // Set the "Touched" bool on the animator to false
    public void OnEndDrag(PointerEventData eventData){
        m_isDragging = false;
        m_anim.SetBool("Touched", false);
    }    

    // // TODO OnDrag
    // // accumulate m_dragDelta
    // // if m_dragDelta accumulates more than s_swipeDist in any single direction,
    // // call m_grid.Swap()
    public void OnDrag(PointerEventData eventData){
        m_dragDelta+=eventData.delta;
        // swipe right 
        if (m_dragDelta.x >= s_swipeDist && m_x+1 < m_grid.m_width){
            m_grid.Swap(m_x,m_y,m_x+1, m_y);
            m_dragDelta = Vector2.zero;
            m_anim.SetBool("Touched", false);
        }
        // swipe left
        if (m_dragDelta.x <= -s_swipeDist && m_x-1 >= 0){
            m_grid.Swap(m_x,m_y,m_x-1, m_y);
            m_dragDelta = Vector2.zero;
            m_anim.SetBool("Touched", false);
        }
        // swipe up
        if (m_dragDelta.y >= s_swipeDist && m_y-1 >= 0){
            m_grid.Swap(m_x,m_y,m_x, m_y-1);
            m_dragDelta = Vector2.zero;
            m_anim.SetBool("Touched", false);
        }
        // swipe down
        if (m_dragDelta.y <= -s_swipeDist && m_y+1 < m_grid.m_height){
            m_grid.Swap(m_x,m_y,m_x, m_y+1);
            m_dragDelta = Vector2.zero;
            m_anim.SetBool("Touched", false);
        }        
    }
}
