using System;
using UnityEngine;

public class Terminal : MonoBehaviour, IInteractable
{
    public event Action<ItemTypeSO> OnItemPurchased;
    public event Action OnPurchaseBlocked;        // sem dinheiro
    public event Action OnWaterUpgradePurchased;

    [Header("Terminal")]
    [SerializeField] private TerminalModeController terminalModeController;
    [SerializeField] private CurrencyManager currencyManager;

    [Header("Shop")]
    [SerializeField] private ShoppingCart shoppingCart;

    [Header("Water")]
    [SerializeField] private WaterTank waterTank;

    private void Awake()
    {
        if (terminalModeController == null)
            Debug.LogError($"{name}: TerminalModeController faltando, o terminal não vai abrir.", this);

        if (currencyManager == null)
            Debug.LogError($"{name}: CurrencyManager faltando, compras e upgrades desativados.", this);

        if (shoppingCart == null)
            Debug.LogError($"{name}: ShoppingCart faltando, os itens comprados não vão ser entregues.", this);

        if (waterTank == null)
            Debug.LogError($"{name}: WaterTank faltando, o upgrade de água não vai funcionar.", this);
    }

    public void EnterTerminalMode()
    {
        if (terminalModeController == null)
            return;

        terminalModeController.EnterTerminalMode();
    }

    public void ExitTerminalMode()
    {
        if (terminalModeController == null)
            return;

        terminalModeController.ExitTerminalMode();
    }


    //metodo geral para comprar qualquer item
    public void BuyItem(ItemTypeSO item)
    {
        if (currencyManager == null || shoppingCart == null)
            return;

        if (item == null)
        {
            Debug.LogError($"{name}: BuyItem chamado sem ItemTypeSO (botão sem item configurado).", this);
            return;
        }

        if (item.ItemPrefab == null)
        {
            Debug.LogError($"{name}: o item '{item.name}' não tem prefab.", this);
            return;
        }

        // confere se o carrinho está funcionando ANTES de cobrar.
        // Por quê: o dinheiro só pode ser gasto se o item puder ser entregue.
        if (!shoppingCart.IsReady)
            return;

        // sem dinehiro
        if (!currencyManager.TrySpendCurrency(item.ItemPrice))
        {
            OnPurchaseBlocked?.Invoke(); // evento
            return;
        }

        shoppingCart.Enqueue(item);

        OnItemPurchased?.Invoke(item); // evento
    }

    // Compra o próximo nível de upgrade do abastecimento de água.
    // Chamado pelo TerminalButton da tela de água (UnityEvent onClick).
    public void BuyWaterUpgrade()
    {
        if (waterTank == null || currencyManager == null)
            return;

        if (!waterTank.TryGetNextLevelData(out WaterTank.WaterUpgradeLevel nextLevelData))
            return;

        // O dinheiro só é gasto depois de confirmar que o próximo nível existe,
        // para nunca cobrar por um upgrade que não pode ser aplicado.
        if (!currencyManager.TrySpendCurrency(nextLevelData.cost))
        {
            OnPurchaseBlocked?.Invoke(); //(aqui também vai a mensagem no monitor, no futuro)
            return;
        }

        waterTank.SetUpgradeLevel(waterTank.CurrentLevel + 1);

        OnWaterUpgradePurchased?.Invoke();
    }

    public void Interact(Player player)
    {
        if (player.IsHoldingSomething)
            return;

        EnterTerminalMode();
    }
}