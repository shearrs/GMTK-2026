using System.Collections;
using Shears;
using Shears.Tweens;
using Shears.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class MenuManager : UIElement
{
    [SerializeField]
    [AutoEvent(nameof(UIButton.Clicked), nameof(OnStartButtonClicked))]
    private UIButton startButton;

    [SerializeField]
    [AutoEvent(nameof(UIButton.Clicked), nameof(OnEndButtonClicked))]
    private UIButton quitButton;

    [SerializeField]
    private Transform sunTransform;

    [SerializeField]
    private Transform risenSunTransform;

    [SerializeField]
    private Transform directionalLightTransform;

    [SerializeField]
    private Transform risenDirectionalLightTransform;

    [SerializeField]
    private GameObject sign;

    [SerializeField]
    private TextMeshProUGUI dayTextMesh;

    [SerializeField]
    private float fadeTweenDuration;

    [SerializeField]
    private TweenData sunriseTween;

    [SerializeField]
    private TweenData simpleTween;

    private Coroutine sunriseCoroutine;

    private bool startingGame = false;

    private void Start()
    {
        sunriseCoroutine = StartCoroutine(DoSunrise());
    }

    private void Update()
    {
        sign.transform.Rotate(0, 10 * Time.deltaTime, 0);
    }

    private IEnumerator DoSunrise()
    {
        yield return new WaitForSeconds(1f);

        var sunTween = StoreTween(
            sunTransform.DoMoveTween(risenSunTransform.position, sunriseTween)
        );
        StoreTween(
            directionalLightTransform.DoRotateTween(
                risenDirectionalLightTransform.rotation,
                true,
                simpleTween
            )
        );

        sunTween.Completed += () =>
        {
            startButton.DoFadeTween(1.0f, simpleTween);
            quitButton.DoFadeTween(1.0f, simpleTween);
        };
    }

    private void OnStartButtonClicked()
    {
        if (!startingGame)
        {
            startingGame = true;
            StartCoroutine(DoLevelTransition());
        }
    }

    private IEnumerator DoLevelTransition()
    {
        var fadeTween = StoreTween(startButton.DoFadeTween(0.0f, simpleTween));
        quitButton.DoFadeTween(0.0f, simpleTween);

        fadeTween.Completed += () =>
        {
            dayTextMesh.DoFadeTween(1.0f, simpleTween);
        };

        yield return new WaitForSeconds(6f);

        SceneManager.LoadScene("Game Scene");
    }

    private void OnEndButtonClicked()
    {
        Application.Quit();
    }
}
