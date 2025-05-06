using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Gerenciador principal do jogo
public class GameManager : MonoBehaviour
{
    // Referência à câmera principal e controle de movimento/rotação
    public Transform CameraTransform;
    public bool CanMoveAndRotateCamera;

    // UI: Exibição do modo atual (câmera ou seleção) e do objeto selecionado
    [Space(8)]
    public TMP_Text ModeText;
    [Space(8)]
    public TMP_Text SelectedObjectText;

    // Controle de movimentação de objeto selecionado
    [Space(8)]
    public bool CanMoveSelectedObject;

    // Painel de instruções e controle de visibilidade
    [Space(8)]
    public GameObject InstructionPanel;
    private bool _hideInstructionPanel = true;

    // Lista de objetos que podem ser selecionados
    [Space(8)]
    public List<SelectableObject> SelectableObjects;

    // Objeto atualmente selecionado
    private GameObject _selectedObject;

    // Teclas de controle para mover objetos e câmera
    private KeyCode _leftKeyCode = KeyCode.A;
    private KeyCode _rightKeyCode = KeyCode.D;
    private KeyCode _forwardKeyCode = KeyCode.W;
    private KeyCode _backwardKeyCode = KeyCode.S;
    private KeyCode _upKeyCode = KeyCode.Q;
    private KeyCode _downKeyCode = KeyCode.E;

    // Tecla para alternar entre modos de câmera e seleção
    private KeyCode _canMoveCameraKeyCode = KeyCode.Space;

    // Velocidade de movimento da câmera e do objeto
    private float _cameraMovementSpeed = 5.0f;
    private float _selectedObjectSpeed = 2.5f;

    // Armazena os ângulos da câmera para manipulação da rotação
    public Vector3 _cameraEulerAngles;

    // Aumenta o tamanho do objeto selecionado (animação visual)
    public void ScaleSelectedObjectOnClick()
    {
        if (_selectedObject != null)
        {
            _selectedObject?.GetComponent<SelectableObject>().PulseScale();
        }
    }

    // Gira o objeto selecionado (animação visual)
    public void RotateSelectedObjectOnClick()
    {
        if (_selectedObject != null)
        {
            _selectedObject?.GetComponent<SelectableObject>().PulseRotation();
        }
    }

    // Mostra ou esconde o painel de instruções
    public void ShowAndHideInstructionPanel()
    {
        _hideInstructionPanel = !_hideInstructionPanel;

        if (_hideInstructionPanel)
        {
            InstructionPanel.SetActive(false);
        }
        else
        {
            InstructionPanel.SetActive(true);
        }
    }

    // Inicialização
    private void Start()
    {
        // Armazena os ângulos iniciais da câmera
        _cameraEulerAngles = CameraTransform.eulerAngles;

        // Encontra todos os objetos do tipo SelectableObject na cena
        var selectableObjectsComponents = FindObjectsOfType<SelectableObject>();

        // Adiciona os objetos encontrados à lista
        for (int i = 0; i < selectableObjectsComponents.Length; i++)
        {
            SelectableObjects.Add(selectableObjectsComponents[i]);
        }

        // Inscreve o método de callback para quando o objeto for clicado
        for (int i = 0; i < SelectableObjects.Count; i++)
        {
            SelectableObjects[i].OnObjectClick += SelectObjectCallBack;
        }
    }

    // Atualização por frame
    private void Update()
    {
        // Alterna entre modos de câmera e seleção ao pressionar a tecla configurada
        if (Input.GetKeyDown(_canMoveCameraKeyCode))
        {
            CanMoveAndRotateCamera = !CanMoveAndRotateCamera;
            if (CanMoveAndRotateCamera)
            {
                DeselectObject(); // Sai do modo de seleção ao ativar câmera
            }
        }

        // Atualiza o modo exibido e realiza ações de acordo com o modo atual
        if (CanMoveAndRotateCamera)
        {
            ModeText.text = "Modo Câmera";
            MoveAndRotateCamera();
        }
        else if (CanMoveSelectedObject)
        {
            MoveSelectedObject();
        }

        if (!CanMoveAndRotateCamera)
        {
            ModeText.text = "Modo Seleção";
        }
    }

    // Movimento e rotação da câmera baseado em teclado e mouse
    private void MoveAndRotateCamera()
    {
        Vector3 direction = Vector3.zero;

        // Direção da câmera com base nas teclas pressionadas
        direction.x -= Input.GetKey(_leftKeyCode) ? 1 : 0;
        direction.x += Input.GetKey(_rightKeyCode) ? 1 : 0;
        direction.z -= Input.GetKey(_backwardKeyCode) ? 1 : 0;
        direction.z += Input.GetKey(_forwardKeyCode) ? 1 : 0;
        direction.y -= Input.GetKey(_downKeyCode) ? 1 : 0;
        direction.y += Input.GetKey(_upKeyCode) ? 1 : 0;

        // Rotação da câmera com o movimento do mouse
        _cameraEulerAngles.y += Input.GetAxis("Mouse X");
        _cameraEulerAngles.x -= Input.GetAxis("Mouse Y");

        // Movimenta a câmera nas direções correspondentes
        CameraTransform.position += CameraTransform.right * direction.x * _cameraMovementSpeed * Time.deltaTime;
        CameraTransform.position += CameraTransform.up * direction.y * _cameraMovementSpeed * Time.deltaTime;
        CameraTransform.position += CameraTransform.forward * direction.z * _cameraMovementSpeed * Time.deltaTime;

        // Aplica a nova rotação
        CameraTransform.eulerAngles = _cameraEulerAngles;
    }

    // Move o objeto selecionado conforme o teclado
    private void MoveSelectedObject()
    {
        Vector3 direction = Vector3.zero;
        Vector3 movement;

        // Direção baseada nas teclas pressionadas
        direction.x -= Input.GetKey(_leftKeyCode) ? 1 : 0;
        direction.x += Input.GetKey(_rightKeyCode) ? 1 : 0;
        direction.z -= Input.GetKey(_backwardKeyCode) ? 1 : 0;
        direction.z += Input.GetKey(_forwardKeyCode) ? 1 : 0;
        direction.y -= Input.GetKey(_downKeyCode) ? 1 : 0;
        direction.y += Input.GetKey(_upKeyCode) ? 1 : 0;

        // Aplica movimento proporcional à velocidade e ao tempo
        movement = direction * _selectedObjectSpeed * Time.deltaTime;

        // Move o objeto selecionado
        _selectedObject.transform.position += movement;
    }

    // Callback ao clicar em um objeto selecionável
    private void SelectObjectCallBack(GameObject selectedObject)
    {
        // Ignora seleção se estiver no modo câmera
        if (CanMoveAndRotateCamera)
        {
            return;
        }

        // Alterna entre seleção e deseleção
        if (_selectedObject == selectedObject)
        {
            DeselectObject();
        }
        else
        {
            _selectedObject = selectedObject;
            SelectedObjectText.text = $"{selectedObject.name} Selecionado";
            CanMoveSelectedObject = true;
        }
    }

    // Deseleciona o objeto atual
    private void DeselectObject()
    {
        CanMoveSelectedObject = false;
        _selectedObject = null;
        SelectedObjectText.text = "Nenhum Objeto Selecionado";
    }
}
