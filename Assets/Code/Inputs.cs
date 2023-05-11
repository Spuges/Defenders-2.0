using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Defender
{
    [DefaultExecutionOrder(-200)]
    public class Inputs : MonoBehaviour
    {
        public static Inputs I { get; private set; }

        public InputActionAsset input_values;

        //public InputAction move_action;
        //public InputAction fire_action; // This does not trigger, better to use the Asset :\

        private void Awake()
        {
            I = this;
        }

        //// Alternatative way of enabling / disabling user inputs o_O
        //private void OnEnable()
        //{
        //    //move_action.Enable();
        //    //fire_action.Enable();
        //}

        //private void OnDisable()
        //{
        //    //move_action.Disable();
        //    //fire_action.Disable();
        //}
    }
}
