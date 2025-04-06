using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    // List of all available skills
    public List<Skill> leftPaddleSkills = new List<Skill>();
    public List<Skill> rightPaddleSkills = new List<Skill>();

    public GameObject doubleBallSkillPrefab;
    public GameObject speedUpBallSkillPrefab;
    public GameObject slowDownBallSkillPrefab;
    public GameObject enlargePaddlePrefab;
    public GameObject shrinkPaddlePrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        AssignSkillsAtStart();
    }

    void Update()
    {
        // Activate skill based on input
        foreach (var skill in leftPaddleSkills)
        {
            if (Input.GetKeyDown(skill.leftPaddleActivationKey))
            {
                if (skill.name == "EnlargePaddle(Clone)" || skill.name == "ShrinkPaddle(Clone)")
                {
                    skill.Activate(1);
                }
                skill.Activate();
            }
        }
        foreach (var skill in rightPaddleSkills)
        {
            if (Input.GetKeyDown(skill.rightPaddleActivationKey))
            {
                if (skill.name == "EnlargePaddle(Clone)" || skill.name == "ShrinkPaddle(Clone)")
                {
                    skill.Activate(2);
                }
                skill.Activate();
            }
        }
    }

    private void AssignSkillsAtStart()
    {
        // Clear existing skills
        leftPaddleSkills.Clear();
        rightPaddleSkills.Clear();

        // Example of assigning DoubleBall skill to both paddles
        Skill leftPaddleSkill = Instantiate(doubleBallSkillPrefab).GetComponent<Skill>();
        Skill rightPaddleSkill = Instantiate(doubleBallSkillPrefab).GetComponent<Skill>();
        
        // Assign SpeedUpBall 
        Skill leftSpeedUpBall = Instantiate(speedUpBallSkillPrefab).GetComponent<Skill>();
        Skill rightSpeedUpBall = Instantiate(speedUpBallSkillPrefab).GetComponent<Skill>();
        
        // Assign SlowDown Ball
        Skill leftSlowDownBall = Instantiate(slowDownBallSkillPrefab).GetComponent<Skill>();
        Skill rightSlowDownBall = Instantiate(slowDownBallSkillPrefab).GetComponent<Skill>();
        
        // Assign Enlarge Paddle
        Skill enlargeLeftPaddle = Instantiate(enlargePaddlePrefab).GetComponent<Skill>();
        Skill enlargeRightPaddle = Instantiate(enlargePaddlePrefab).GetComponent<Skill>();

        // Assign Shrink Paddle
        Skill shrinkRightPaddle = Instantiate(shrinkPaddlePrefab).GetComponent<Skill>();
        Skill shrinkLeftPaddle = Instantiate(shrinkPaddlePrefab).GetComponent <Skill>();



        leftPaddleSkills.Add(leftPaddleSkill);
        rightPaddleSkills.Add(rightPaddleSkill);
        leftPaddleSkills.Add(leftSpeedUpBall);
        rightPaddleSkills.Add(rightSpeedUpBall);
        leftPaddleSkills.Add(leftSlowDownBall);
        rightPaddleSkills.Add(rightSlowDownBall);
        leftPaddleSkills.Add(enlargeLeftPaddle);
        rightPaddleSkills.Add(enlargeRightPaddle);
        leftPaddleSkills.Add(shrinkRightPaddle);
        rightPaddleSkills.Add(shrinkLeftPaddle);


    }
}
