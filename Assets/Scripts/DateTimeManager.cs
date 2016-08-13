using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Date : MonoBehaviour
{
    private Button _button;
    private DateData _data;

    private Text _dateText
    {
        get { return transform.GetChild(0).GetComponent<Text>(); }
    }

    private Text _dayText
    {
        get { return transform.GetChild(1).GetComponent<Text>(); }
    }

    void Awake()
    {
        _button = gameObject.AddComponent<Button>();
        _button.onClick.AddListener(()=> DateTimeManager.SelectDate(_data));
    }

    public void AssignData(DateData data)
    {
        _data = data;
        _dateText.text = data.Date;
        _dayText.text = data.Day;

        if(data.Day == "Sunday")
            _dayText.color = Color.red;
    }
}

public class DateData
{
    public string Day;
    public string Date;

    public DateData(string day, string date)
    {
        Day = day;
        Date = date;
    }
}

public class DateTimeManager : MonoBehaviour
{
    private static DateTimeManager _instance;

    public Text MonthName;
    public Transform DateContainer;
    public Transform DatePrefabs;
    public Text ChoosenDateText;

    private List<Transform> _dates = new List<Transform>(); 

    private int _currentMonth = 1;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        ShowMonth();
    }

    public void NextMonth()
    {
        if (_currentMonth > 1)
            _currentMonth--;
        else
            _currentMonth = 12;

        ShowMonth();
    }

    public void PreviousMonth()
    {
        if (_currentMonth < 12)
            _currentMonth++;
        else
            _currentMonth = 1;

        ShowMonth();
    }

    private void ShowMonth()
    {
        ClearMonth();
        List<DateTime> dates = GetDates();

        MonthName.text = dates[0].ToString("MMMM");

        for (int i = 0; i < (int)dates[0].DayOfWeek - 1; i++)
        {
            Date newDate = Instantiate(DatePrefabs).gameObject.AddComponent<Date>();
            newDate.AssignData(new DateData("", ""));

            newDate.transform.SetParent(DateContainer);

            _dates.Add(newDate.transform);
        }

        foreach (DateTime dateTime in dates)
        {
            Date newDate = Instantiate(DatePrefabs).gameObject.AddComponent<Date>();
            newDate.AssignData(new DateData(dateTime.ToString("dddd"), dateTime.Day.ToString()));

            newDate.transform.SetParent(DateContainer);

            _dates.Add(newDate.transform);
        }
    }

    private void ClearMonth()
    {
        ChoosenDateText.text = "";

        for (int i = 0; i < _dates.Count; i++)
        {
            GameObject.Destroy(_dates[i].gameObject);
        }

        _dates.Clear();
    }

    private List<DateTime> GetDates()
    {
        return Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, _currentMonth))  // Days: 1, 2 ... 31 etc.
                         .Select(day => new DateTime(DateTime.Now.Year, _currentMonth, day)) // Map each day to a date
                         .ToList(); // Load dates into a list
    }

    public static void SelectDate(DateData data)
    {
        _instance.ChoosenDateText.text = "Choosen Date = " + data.Date + ", " + _instance.MonthName.text + ", " + data.Day;
    }
}