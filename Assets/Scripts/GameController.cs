using System;
using UnityEngine;
using UnityEngine.UI;
using static System.Random;

public class GameController : MonoBehaviour
{

    public Text moneyText;
    public Text healthText;
    public Text damageText;
    public Text stageText;
    public Text killsText;
    public Text timerText;
    public Text offlineTimeText;
    public Text multiText;

    public GameObject back;
    public GameObject forward;
    public GameObject offlineBox;

    public double money;
    public double moneyPerSec
    { 
        get 
        { 
            return (Math.Ceiling(healthCap / 14) / healthCap) * dps;
        } 
    }
    public double multiValue;
    public double multiValueMoney;
    public double damage;
    public double health;
    public double dps; //damage per second
    public double healthCap
    {
        get
        {
            return 10 * System.Math.Pow(2, stage - 1) * isBoss;
        }
        set { }
    }

    public int stage;
    public int kills;
    public int killsMax;
    public int stageMax;
    public int isBoss;
    public int timerCap;
    public int offlineProgressCheck;
    public int offlineLoadCount;

    public float timer;
    public float idleTime;
    public float saveTime;

    public Image healthBar;
    public Image timerBar;

    public DateTime currentDate;
    public DateTime oldTime;

    public void Start()
    {
        offlineBox.gameObject.SetActive(false);
        Load();
        multiValue = new System.Random().Next(20, 100);
        // damage = 1;
        //dps = 1;
        //stage = 1;
        //stageMax = 1;
        killsMax = 10;
        //health = 10;
        //isBoss = 1;
        health = healthCap;
        IsBossChecker();
        timerCap = 30;
    }


    public void Update()
    {
        /*  if (offlineLoadCount==1)
          {
              LoadOfflineProuction();
          }
          offlineLoadCount += 1;*/
        multiValueMoney = multiValue * moneyPerSec;
        moneyText.text = "$" + money.ToString("F2");// ToString("F2") makes numer shorter
        damageText.text = damage + "Damage";
        stageText.text = "Stage-" + stage;
        killsText.text = kills + "/" + killsMax + "Kills";
        healthText.text = kills + "/" + healthCap + "HP";
        healthBar.fillAmount = (float)(health / healthCap);
        timerText.text = timer.ToString("F2") + "/" + timerCap;

        if (stage > 1)
        {
            back.gameObject.SetActive(true);
        }
        else
        {
            back.gameObject.SetActive(false);
        }
        if (stage != stageMax)
        {
            forward.gameObject.SetActive(true);
        }
        else
        {
            forward.gameObject.SetActive(false);
        }

        //healthCap=10*System.Math.Pow(2, stage-1)*isBoss;
        IsBossChecker();
        saveTime += Time.deltaTime;
        if (saveTime >= 5)
        {
            saveTime = 0;
            Save();
        }
    }
    public void IsBossChecker()
    {
        if (stage % 5 == 0)
        {

            isBoss = 10;
            stageText.text = "BOSS Stage-" + stage;
            timerText.text = "";
            killsMax = 1;
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                Back();
                // killsMax = 10;
                // stage = -1;
                //health = healthCap;
            }
            timerText.text = timer.ToString("F2") + "/" + timerCap;
            timerBar.gameObject.SetActive(true);
            timerBar.fillAmount = timer / timerCap;
        }
        else
        {
            isBoss = 1;
            stageText.text = "Stage-" + stage;
            timerText.text = "";
            timerBar.gameObject.SetActive(false);
            timer = 30;
            killsMax = 10;
        }
    }

    public void Back() //back button f.
    {
        stage -= 1;
        IsBossChecker();
        health = healthCap;
    }
    public void Forward() //forward button f.
    {
        stage += 1;
        IsBossChecker();
        health = healthCap;
    }
    public void Hit()
    {
        health -= damage;
        if (health <= 0)
        {
            money += Math.Ceiling(healthCap / 14); //adding money per kill
            if (stage == stageMax)
            {
                kills += 1;
                if (kills >= killsMax)
                {
                    kills = 0;
                    stage += 1;
                    stageMax += 1;
                }
            }
            IsBossChecker();
            health = healthCap;
            if (isBoss > 1)
            {
                timer = timerCap;
                killsMax = 10;
            }
        }
    }
    public void Save()
    {
        offlineProgressCheck = 1;
        PlayerPrefs.SetString("money", money.ToString());
        PlayerPrefs.SetString("damage", damage.ToString());
        PlayerPrefs.SetString("dps", dps.ToString());
        PlayerPrefs.SetInt("stage", stage);
        PlayerPrefs.SetInt("stageMax", stageMax);
        PlayerPrefs.SetInt("kills", kills);
        PlayerPrefs.SetInt("killsMax", kills);
        PlayerPrefs.SetInt("isBoss", isBoss);
        PlayerPrefs.SetInt("offlineProgressCheck", offlineProgressCheck);

        PlayerPrefs.SetString("offlineTime", DateTime.Now.ToBinary().ToString());//offline saving
    }
    public void Load()
    {
        money = double.Parse(PlayerPrefs.GetString("money", "0"));
        damage = double.Parse(PlayerPrefs.GetString("damage", "1"));
        dps = double.Parse(PlayerPrefs.GetString("dps", "1"));
        stage = PlayerPrefs.GetInt("stage", 1);
        stageMax = PlayerPrefs.GetInt("stageMax", 1);
        kills = PlayerPrefs.GetInt("kills", 0);
        killsMax = PlayerPrefs.GetInt("killsMax", 10);
        isBoss = PlayerPrefs.GetInt("isBoss", 1);
        offlineProgressCheck = PlayerPrefs.GetInt("offlineProgressCheck", 0);
        LoadOfflineProuction();
    }

    public void LoadOfflineProuction()
    {
        if (offlineProgressCheck == 1)
        {
            offlineBox.gameObject.SetActive(true);
            long previousTime = Convert.ToInt64(PlayerPrefs.GetString("offlineTime"));//offline save stuffs
            oldTime = DateTime.FromBinary(previousTime);
            currentDate = DateTime.Now;
            TimeSpan difference = currentDate.Subtract(oldTime);
            idleTime = (float)difference.TotalSeconds;
            var moneyToEarn = (Math.Ceiling(healthCap / 14) / healthCap) * (dps / 5) * idleTime;// every 5 secods adding mony while offline
            money += moneyToEarn;
            TimeSpan timer = TimeSpan.FromSeconds(idleTime);
            offlineTimeText.text = "You were offline for:" + timer.ToString(@"hh\:mm\:ss") + "\nYou earned:$" + moneyToEarn.ToString("F2");
        }
    }
    public void CloseOfflineBox()
    {
        offlineBox.gameObject.SetActive(false);
    }
}
