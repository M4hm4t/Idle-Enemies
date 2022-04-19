using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public Text moneyText;
    public Text healthText;
    public Text damageText;
    public Text dpsText;
    public Text stageText;
    public Text killsText;
    public Text timerText;
    public Text offlineTimeText;
    public Text multiText;

    public GameObject back;
    public GameObject forward;
    public GameObject offlineBox;
    public GameObject multiBox;
    public GameObject boss;

    public double money;
    public double moneyPerSec
    {
        get
        {
            return Math.Ceiling(healthCap / 14) / healthCap * dps;
        }
    }
    public double multiValue;
    public double multiValueMoney;
    public double damage;//dpc
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
    public int newPlayer;
    public GameObject userNameBox;

    public float timer;
    public float idleTime;
    public float saveTime;
    public float multiTimer;
    public float multiTimerCap;

    public Image healthBar;
    public GameObject bossImage;
    public GameObject enemyImage;
    public Image timerBar;
    public Image bgBoss;//boss background

    public DateTime currentDate;
    public DateTime oldTime;

    public TMP_InputField userNameInput;
    public string username;
    public Text usernameText;

    //Upgrades
    public Text pCostText;
    public Text pLevelText;
    public Text pPowerText;

    public double pCost
    {
        get
        {
            return 10 * Math.Pow(1.07, pLevel);
        }
    }
    public int pLevel;
    public double pPower
    {
        get
        {
            return 5 * pLevel;
        }
    }
    public Text cCostText;
    public Text cLevelText;
    public Text cPowerText;
    public double cCost
    {
        get
        {
            return 10 * Math.Pow(1.07, cLevel);
        }
    }
    public int cLevel;
    public double cPower
    {
        get
        {
            return 2 * cLevel;
        }
    }
    

    public void Start()
    {
        
        if (username == "User Name")//not working
        {
            userNameBox.gameObject.SetActive(true);
        }
        else
        {
            userNameBox.gameObject.SetActive(false);
        }
        multiBox.gameObject.SetActive(false);
        offlineBox.gameObject.SetActive(false);
        Load();
        multiValue = new System.Random().Next(20, 100);
        multiTimerCap = new System.Random().Next(5, 10);
        multiTimer = multiTimerCap;
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
        if (health <= 0)
        {
            Kill();
        }
        else
        {
            health -= dps * Time.deltaTime;//idle damage
        }
        multiValueMoney = multiValue * moneyPerSec;
        multiTimer -= Time.deltaTime;
        multiText.text = "$" + WordNotation(multiValueMoney,"F2");
        if (multiTimer <= 0)
        {
           // multiValue = new System.Random().Next(5, 10);
            //multiTimer = multiTimerCap;
            multiBox.gameObject.SetActive(true);

        }
        else
        {
            multiTimer -= Time.deltaTime;
        }
        moneyText.text = "$" + WordNotation(money,"F2");// ToString("F2") makes numer shorter
        damageText.text = WordNotation(damage,"F2") + "Damage Per Click";
        dpsText.text = WordNotation(dps,"F2") + "Damage Per Second";
        stageText.text = "Stage-" + stage;
        killsText.text = kills + "/" + killsMax + "Kills";
        healthText.text = WordNotation(health,"F2") + "/" + WordNotation(healthCap,"F2") + "HP";
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
        //Username();
        usernameText.text = username;
        //healthCap=10*System.Math.Pow(2, stage-1)*isBoss;
        IsBossChecker();
        Upgrades();
        saveTime += Time.deltaTime;
        if (saveTime >= 5)
        {
            saveTime = 0;
            Save();
        }
    }
  
    public void BuyUpgrade(string id)
    {
        switch (id)
        {
            case "p1":
                if (money >= pCost)
                {
                    UpgradeDefaults(ref pLevel, pCost);
                }
                break;
            case "c1":
                if (money >= cCost)
                {
                    UpgradeDefaults(ref cLevel, pCost);
                }
                break;
        }
    }

    public void Upgrades()
    {
        cCostText.text = "Cost:$" + WordNotation(cCost,"F2");
        cLevelText.text = "Level:" + cLevel;
        cPowerText.text = "+2 per hit";
        pCostText.text = "Cost:$" + WordNotation(pCost,"F2");
        pLevelText.text = "Level:" + pLevel;
        pPowerText.text = "+5 per second";
        dps = pPower;
        damage = 1 + cPower;
    }
    public void UpgradeDefaults(ref int level,double cost)
    {
        money -= cost;
        level++;
    }
    public string WordNotation(double number,string digits) //converting big number to ABC
    {
        double digitsTemp = Math.Floor(Math.Log10(number));
        IDictionary<double, string> prefixes=new Dictionary<double, string>()
        {   
            {3, "K"},
            {6, "M"}, 
            {9, "B"},
            {12, "T"},
            {15, "Qa"},     
            {18, "Qi"},
            {21, "Se"},
            {24, "Sep"},
           
        };
        double digitsEvery3 = 3 * Math.Floor(digitsTemp / 3);
        if (number >= 1000)
        {
            return (number/Math.Pow(10, digitsEvery3)).ToString(digits)+prefixes[digitsEvery3];
        }
        return number.ToString(digits);
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
            bgBoss.gameObject.SetActive(true);
            boss.gameObject.SetActive(true);
            bossImage.gameObject.SetActive(true);
            enemyImage.gameObject.SetActive(false);
        }
        else
        {
            isBoss = 1;
            stageText.text = "Stage-" + stage;
            timerText.text = "";
            timerBar.gameObject.SetActive(false);   
            timer = 30;
            killsMax = 10;
            bgBoss.gameObject.SetActive(false);
            boss.gameObject.SetActive(false);
            bossImage.gameObject.SetActive(false);
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
        Kill();
    }
    public void Save()
    {
        // newPlayer = 1;
        offlineProgressCheck = 1;
        PlayerPrefs.SetString("money", money.ToString());
        PlayerPrefs.SetString("damage", damage.ToString());
        PlayerPrefs.SetString("dps", dps.ToString());
        PlayerPrefs.SetString("username", username);
        PlayerPrefs.SetInt("stage", stage);
        PlayerPrefs.SetInt("stageMax", stageMax);
        PlayerPrefs.SetInt("kills", kills);
        PlayerPrefs.SetInt("killsMax", kills);
        PlayerPrefs.SetInt("isBoss", isBoss);
        PlayerPrefs.SetInt("pLevel", pLevel);
        PlayerPrefs.SetInt("cLevel", cLevel);
        PlayerPrefs.SetInt("offlineProgressCheck", offlineProgressCheck);

        PlayerPrefs.SetString("offlineTime", DateTime.Now.ToBinary().ToString());//offline saving
    }
    public void Load()
    {
        money = double.Parse(PlayerPrefs.GetString("money", "0"));
        username = PlayerPrefs.GetString("username");
        damage = double.Parse(PlayerPrefs.GetString("damage", "1"));
        dps = double.Parse(PlayerPrefs.GetString("dps", "0"));
        stage = PlayerPrefs.GetInt("stage", 1);
        stageMax = PlayerPrefs.GetInt("stageMax", 1);
        kills = PlayerPrefs.GetInt("kills", 0);
        killsMax = PlayerPrefs.GetInt("killsMax", 10);
        isBoss = PlayerPrefs.GetInt("isBoss", 1);
        pLevel = PlayerPrefs.GetInt("pLevel", 0);
        cLevel = PlayerPrefs.GetInt("cLevel", 0);
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

    public void OpenMulti()
    {
        multiBox.gameObject.SetActive(false);
        multiTimerCap = new System.Random().Next(5, 10);
        multiValue = new System.Random().Next(20, 100);
        multiTimer = multiTimerCap;
        money += multiValueMoney;
    }
    public void Kill()
    {
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
    public void Username()
    {
        username = userNameInput.text;

    }
    public void CloseUserNameBox()
    {
        userNameBox.gameObject.SetActive(false);
    }
}
