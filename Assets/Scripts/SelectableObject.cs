using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectableObject : MonoBehaviour
{
    // Indica se o mouse está sobre o objeto
    public bool MouseOn;

    // Evento disparado ao clicar no objeto, passa o próprio GameObject como parâmetro
    public UnityAction<GameObject> OnObjectClick;

    [Space(8)]
    [SerializeField] private AnimationCurve _scaleCurve;        // Curva de animação para escala (deformação)
    [SerializeField] private float _pulseDuration = 1.0f;       // Duração da animação de escala
    [SerializeField] private float _scaleMultiplier = 1.5f;     // Fator de ampliação máxima da escala

    [Space(8)]
    [SerializeField] private AnimationCurve _rotationCurve;     // Curva de animação para rotação
    [SerializeField] private float _rotationDuration = 1.0f;    // Duração da animação de rotação
    [SerializeField] private float _rotationAngle = 360f;       // Ângulo máximo de rotação em graus

    private int _mouseButton = 0; // Botão do mouse que será usado (0 = botão esquerdo)

    // Inicia a animação de escala
    public void PulseScale()
    {
        StopAllCoroutines(); // Interrompe qualquer animação anterior
        StartCoroutine(PulseScaleCoroutine());
    }

    // Inicia a animação de rotação
    public void PulseRotation()
    {
        StopAllCoroutines(); // Interrompe qualquer animação anterior
        StartCoroutine(PulseRotationZCoroutine());
    }

    // Corrutina para animar a escala usando a curva definida
    private IEnumerator PulseScaleCoroutine()
    {
        float time = 0f;
        Vector3 originalScale = transform.localScale;

        while (time < _pulseDuration)
        {
            float normalizedTime = time / _pulseDuration;
            float curveValue = _scaleCurve.Evaluate(normalizedTime);
            float scaleFactor = Mathf.Lerp(1f, _scaleMultiplier, curveValue);

            transform.localScale = originalScale * scaleFactor;

            time += Time.deltaTime;
            yield return null;
        }

        // Retorna à escala original após a animação
        transform.localScale = originalScale;
    }

    // Corrutina para animar rotação no eixo Z usando a curva definida
    private IEnumerator PulseRotationZCoroutine()
    {
        float time = 0f;
        float baseZ = transform.eulerAngles.z;

        while (time < _rotationDuration)
        {
            float normalizedTime = time / _rotationDuration;
            float curveValue = _rotationCurve.Evaluate(normalizedTime);

            float angleZ = baseZ + curveValue * _rotationAngle;
            transform.rotation = Quaternion.Euler(0f, 0f, angleZ);

            time += Time.deltaTime;
            yield return null;
        }

        // Retorna à rotação original após a animação
        transform.rotation = Quaternion.Euler(0f, 0f, baseZ);
    }

    private void Update()
    {
        // Se o mouse está sobre o objeto e clicou, dispare o evento de clique
        if (MouseOn)
        {
            if (Input.GetMouseButtonDown(_mouseButton))
            {
                OnObjectClick?.Invoke(gameObject);
            }
        }
    }

    // Detecta quando o mouse entra na área do objeto
    private void OnMouseEnter()
    {
        MouseOn = true;
    }

    // Detecta quando o mouse sai da área do objeto
    private void OnMouseExit()
    {
        MouseOn = false;
    }
}
