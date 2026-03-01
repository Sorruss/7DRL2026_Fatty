using UnityEngine;
using UnityEngine.InputSystem;

namespace FG
{
    public class UICustomCursor : MonoBehaviour
    {
        private void Start()
        {
            Cursor.visible = false;
        }

        private void Update()
        {
            transform.position = Mouse.current.position.ReadValue();
        }
    }
}
