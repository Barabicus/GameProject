using UnityEngine;
using System.Collections;

public class DayCycleManager : MonoBehaviour {

    public Light directionalLight;
    public Color dayColorAmbientLight;
    public Color duskColorAmbientLight;
    public Color nightColorAmbientLight;
    public Color dawnColorAmbientLight;
    public Color dayColor;
    public Color duskColor;
    public Color nightColor;
    public Color dawnColor;
    public float secondsPerDay = 1000;
    public float secondsPerDusk = 100;
    public float secondsPerNight = 1000;
    public float secondsPerDawn = 100;

    private float _currentTime = 0;
    private float _maxCachedTime;
    private DayCycle _dayState = DayCycle.Day;

    enum DayCycle
    {
        Day,
        Dusk,
        Night,
        Dawn
    }

    void Start()
    {
        Debug.Log(getPercent(100, 200));
        _maxCachedTime = secondsPerDay + secondsPerDusk + secondsPerNight + secondsPerDawn;
        StartCoroutine(TimeIncrementor());
    }
	
	// Update is called once per frame
    void Update()
    {
        switch (_dayState)
        {
            case DayCycle.Day:
                directionalLight.color = Color.Lerp(dayColor, duskColor, getPercent(_currentTime, secondsPerDay));
                RenderSettings.ambientLight = Color.Lerp(dayColorAmbientLight, duskColorAmbientLight, getPercent(_currentTime, secondsPerDay));
                break;
            case DayCycle.Dusk:
                directionalLight.color = Color.Lerp(duskColor, nightColor, getPercent(_currentTime, secondsPerDusk));
                RenderSettings.ambientLight = Color.Lerp(duskColorAmbientLight, nightColorAmbientLight, getPercent(_currentTime, secondsPerDusk));
                break;
            case DayCycle.Night:
                directionalLight.color = Color.Lerp(nightColor, dawnColor, getPercent(_currentTime, secondsPerNight));
                RenderSettings.ambientLight = Color.Lerp(nightColorAmbientLight, dawnColorAmbientLight, getPercent(_currentTime, secondsPerNight));
                break;
            case DayCycle.Dawn:
                directionalLight.color = Color.Lerp(dawnColor, dayColor, getPercent(_currentTime, secondsPerDawn));
                RenderSettings.ambientLight = Color.Lerp(dawnColorAmbientLight, dayColorAmbientLight, getPercent(_currentTime, secondsPerDawn));
                break;
        }
    }

    IEnumerator TimeIncrementor()
    {
        while (true)
        {
            _currentTime++;
            switch (_dayState)
            {
                case DayCycle.Day:
                    if (_currentTime >= secondsPerDay) { _currentTime = 0; _dayState = DayCycle.Dusk; }
                    break;
                case DayCycle.Dusk:
                    if (_currentTime >= secondsPerDawn) { _currentTime = 0; _dayState = DayCycle.Night; }
                    break;
                case DayCycle.Night:
                    if (_currentTime >= secondsPerNight) { _currentTime = 0; _dayState = DayCycle.Dawn; }
                    break;
                case DayCycle.Dawn:
                    if (_currentTime >= secondsPerDawn) { _currentTime = 0; _dayState = DayCycle.Day; }
                    break;
            }
            yield return new WaitForSeconds(1);
        }
    }

    float getPercent(float current, float max)
    {
        return ((current / max) * 100) / 100;
    }
}
