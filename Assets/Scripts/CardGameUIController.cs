using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGameUIController : MonoBehaviour
{
    [SerializeField] private MeshRenderer _enemyTurnIndicator = null;
    [SerializeField] private MeshRenderer _playerTurnIndicator = null;

    [SerializeField] private RectTransform _playerUI = null;
    [SerializeField] private Text _playerTurnTextUI = null;
    [SerializeField] private GameObject _playerHandSizeUIContainer = null;
    [SerializeField] private Text _playerHandSizeUI = null;
    [SerializeField] private GameObject _enemyHandSizeUIContainer = null;
    [SerializeField] private Text _enemyHandSizeUI = null;
    [SerializeField] private RectTransform _gameOverUI = null;
    [SerializeField] private RectTransform _mainMenuUI = null;

    private void OnEnable()
    {
        EnemyTurnCardGameState.EnemyTurnBegan += OnEnemyTurnBegan;
        EnemyTurnCardGameState.EnemyTurnEnded += OnEnemyTurnEnded;
        PlayerTurnCardGameState.PlayerTurnBegan += OnPlayerTurnBegan;
        PlayerTurnCardGameState.PlayerTurnEnded += OnPlayerTurnEnded;
        SetupCardGameState.GameBegan += OnGameBegan;
        SetupCardGameState.GameEnded += OnGameEnded;
        GameOverCardGameState.OnGameOverEntered += OnGameOverEntered;
        GameOverCardGameState.OnGameOverExited += OnGameOverExited;
    }

    private void OnDisable()
    {
        EnemyTurnCardGameState.EnemyTurnBegan -= OnEnemyTurnBegan;
        EnemyTurnCardGameState.EnemyTurnEnded -= OnEnemyTurnEnded;
        PlayerTurnCardGameState.PlayerTurnBegan -= OnPlayerTurnBegan;
        PlayerTurnCardGameState.PlayerTurnEnded -= OnPlayerTurnEnded;
        SetupCardGameState.GameBegan -= OnGameBegan;
        SetupCardGameState.GameEnded -= OnGameEnded;
        GameOverCardGameState.OnGameOverEntered -= OnGameOverEntered;
        GameOverCardGameState.OnGameOverExited -= OnGameOverExited;
    }

    private void Start()
    {
        this._enemyTurnIndicator.material.SetColor("_EmissionColor", Color.black);
        this._playerTurnIndicator.material.SetColor("_EmissionColor", Color.black);
        this._playerUI.gameObject.SetActive(false);
    }

    void OnEnemyTurnBegan()
    {
        this._enemyTurnIndicator.material.SetColor("_EmissionColor", new Color(0f, 0.7916667f, 1f));
    }

    void OnEnemyTurnEnded()
    {
        this._enemyTurnIndicator.material.SetColor("_EmissionColor", Color.black);
    }

    void OnPlayerTurnBegan()
    {
        this._playerTurnIndicator.material.SetColor("_EmissionColor", new Color(0f, 0.7916667f, 1f));
    }

    void OnPlayerTurnEnded()
    {
        this._playerTurnIndicator.material.SetColor("_EmissionColor", Color.black);
    }

    void OnGameBegan()
    {
        this._playerHandSizeUIContainer.gameObject.SetActive(true);
        this._enemyHandSizeUIContainer.gameObject.SetActive(true);
        this._mainMenuUI.gameObject.SetActive(false);
        this._playerTurnIndicator.transform.parent.parent.gameObject.SetActive(true);
    }

    void OnGameEnded()
    {
        this._playerHandSizeUIContainer.gameObject.SetActive(false);
        this._enemyHandSizeUIContainer.gameObject.SetActive(false);
        this._playerTurnIndicator.transform.parent.parent.gameObject.SetActive(false);
    }

    void OnGameOverEntered()
    {
        this._gameOverUI.gameObject.SetActive(true);
        this._playerHandSizeUIContainer.gameObject.SetActive(false);
        this._enemyHandSizeUIContainer.gameObject.SetActive(false);
    }

    void OnGameOverExited()
    {
        this._gameOverUI.gameObject.SetActive(false);
        this._mainMenuUI.gameObject.SetActive(true);
    }
}
