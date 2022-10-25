using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGameUIController : MonoBehaviour
{
    [SerializeField] private Text _enemyThinkingTextUI = null;

    [SerializeField] private RectTransform _playerUI = null;
    [SerializeField] private Text _playerTurnTextUI = null;

    private void OnEnable()
    {
        EnemyTurnCardGameState.EnemyTurnBegan += OnEnemyTurnBegan;
        EnemyTurnCardGameState.EnemyTurnEnded += OnEnemyTurnEnded;
        PlayerTurnCardGameState.PlayerTurnBegan += OnPlayerTurnBegan;
        PlayerTurnCardGameState.PlayerTurnEnded += OnPlayerTurnEnded;
    }

    private void OnDisable()
    {
        EnemyTurnCardGameState.EnemyTurnBegan -= OnEnemyTurnBegan;
        EnemyTurnCardGameState.EnemyTurnEnded -= OnEnemyTurnEnded;
        PlayerTurnCardGameState.PlayerTurnBegan -= OnPlayerTurnBegan;
        PlayerTurnCardGameState.PlayerTurnEnded -= OnPlayerTurnEnded;
    }

    private void Start()
    {
        this._enemyThinkingTextUI.gameObject.SetActive(false);
        this._playerUI.gameObject.SetActive(false);
        this._playerTurnTextUI.gameObject.SetActive(false);
    }

    void OnPlayerTurnBegan()
    {
        this._playerUI.gameObject.SetActive(true);
        this._playerTurnTextUI.gameObject.SetActive(true);
    }
    
    void OnPlayerTurnEnded()
    {
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
}
