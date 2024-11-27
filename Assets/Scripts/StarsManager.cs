using UnityEngine;

public class StarsManager : MonoBehaviour
{
    [SerializeField] private Animator animatorStars;
    private int currentStars = 0;
    private int queueStars = 0;
    private bool cooldown = false;

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                AddToQueue();
            }
        }
    }
    public void AddToQueue()
    {
        if (currentStars >= 3) return;
        queueStars++;
        ProcessQueue();
    }

    public int getStars(){
        return queueStars + currentStars;
    }

    private void ProcessQueue()
    {
        if (cooldown || queueStars <= 0) return;

        cooldown = true;
        queueStars--;
        currentStars++;
        animatorStars.enabled = true;
        if (currentStars == 1) animatorStars.Play("FirstStar", -1, 0);
        else if (currentStars == 2) animatorStars.Play("SecondStar", -1, 0);
        else if (currentStars == 3) animatorStars.Play("ThirdStar", -1, 0);

        Invoke(nameof(ResetCooldown), 1.2f);
    }
    private void ResetCooldown()
    {
        cooldown = false;
        ProcessQueue();
    }
}
