using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayersNoMesmoTeclado : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private PlayerInput playerInput;

    private void Awake()
    {
        if (Keyboard.current == null)
            return;

        InputUser.PerformPairingWithDevice(Keyboard.current, playerInput.user);
    }
}
