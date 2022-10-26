using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGameUIController : MonoBehaviour
{
    [SerializeField] private Text _enemyThinkingTextUI = null;

    [SerializeField] private RectTransform _playerUI = null;
    [SerializeField] private Text _playerTurnTextUI = null;
    [SerializeField] private Text _playerHandSizeUI = null;
    [SerializeField] private Text _enemyHandSizeUI = null;
    [SerializeField] private RectTransform _gameOverUI = null;

    private void OnEnable()
    {
        EnemyTurnCardGameState.EnemyTurnBegan += OnEnemyTurnBegan;
        EnemyTurnCardGameState.EnemyTurnEnded += OnEnemyTurnEnded;
        SetupCardGameState.GameBegan += OnGameBegan;
        SetupCardGameState.GameEnded += OnGameEnded;
        GameOverCardGameState.OnGameOverEntered += OnGameOverEntered;
        GameOverCardGameState.OnGameOverExited += OnGameOverExited;
    }

    private void OnDisable()
    {
        EnemyTurnCardGameState.EnemyTurnBegan -= OnEnemyTurnBegan;
        EnemyTurnCardGameState.EnemyTurnEnded -= OnEnemyTurnEnded;
        SetupCardGameState.GameBegan -= OnGameBegan;
        SetupCardGameState.GameEnded -= OnGameEnded;
        GameOverCardGameState.OnGameOverEntered -= OnGameOverEntered;
        GameOverCardGameState.OnGameOverExited -= OnGameOverExited;
    }

    private void Start()
    {
        this._enemyThinkingTextUI.gameObject.SetActive(false);
        this._playerUI.gameObject.SetActive(false);
        this._playerTurnTextUI.gameObject.SetActive(false);
    }

    void OnEnemyTurnBegan()
    {
        this._enemyThinkingTextUI.gameObject.SetActive(true);
    }

    void OnEnemyTurnEnded()
    {
        this._enemyThinkingTextUI.gameObject.SetActive(false);
    }

    void OnGameBegan()
    {
        this._playerHandSizeUI.gameObject.SetActive(true);
        this._enemyHandSizeUI.gameObject.SetActive(true);
    }

    void OnGameEnded()
    {
        this._playerHandSizeUI.gameObject.SetActive(false);
        this._enemyHandSizeUI.gameObject.SetActive(false);
    }

    void OnGameOverEntered()
    {
        this._gameOverUI.gameObject.SetActive(true);
    }

    void OnGameOverExited()
    {
        this._gameOverUI.gameObject.SetActive(false);
    }
}
