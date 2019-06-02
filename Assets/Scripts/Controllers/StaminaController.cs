﻿using Assets.Scripts.BaseScripts;
using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    class StaminaController : BaseController
    {
        private PlayerCharacteristics playerCharacteristics;

        private PCInputController InputController;

        private MovementController movementController;

        public float Stamina;
        private float StaminaMaximum;

        public bool CanRun { get; private set; }
        private bool RunPress;

        private bool JumpPress;
        public bool CanJump { get; private set; }

        private bool RollPress;
        public bool CanRoll { get; private set; }

        private bool IsStanding;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Stamina">Ссылка на текущее значение стамины</param>
        /// <param name="playerCharacteristics">Ссылка на модель характеристик игрока</param>
        /// <param name="InputController">Ссылка на контроллер ввода</param>
        /// <param name="movementController">Ссылка на контроллер перемещения</param>
        public StaminaController(ref float Stamina, PlayerCharacteristics playerCharacteristics, 
            PCInputController InputController, MovementController movementController)
        {
            //Получаем ссылки
            this.Stamina = Stamina;

            this.playerCharacteristics = playerCharacteristics;

            this.movementController = movementController;

            this.InputController = InputController;

            StaminaMaximum = playerCharacteristics.StaminaMaximum;
        }


        private void GetInputsAndFlags()
        {
            RunPress = InputController.Run;
            JumpPress = InputController.Jump;
            RollPress = InputController.Roll;
            IsStanding = movementController.IsStanding;
        }


        public override void ControllerUpdate()
        {
            GetInputsAndFlags();

            if (IsStanding)
            {
                Regenerate();
            }

            RunStaminaDrain();
            JumpStaminaDrain();
            RollStaminaDrain();

            //Ограничиваем значения стамины
            playerCharacteristics.Stamina = Mathf.Clamp(Stamina, 0, StaminaMaximum);
        }

        /// <summary>
        /// Метод регенерации стамины
        /// </summary>
        private void Regenerate()
        {
            playerCharacteristics.Stamina += playerCharacteristics.StaminaRegenRate * Time.deltaTime;
        }

        /// <summary>
        /// Метод расхода стамины на прыжок
        /// </summary>
        /// <param name="JumpPress"></param>
        private void JumpStaminaDrain()
        {
            CanJump = (JumpPress & Stamina > playerCharacteristics.StaminaJumpCoast);

            if(CanJump & movementController.IsGrounded)
            {
                playerCharacteristics.Stamina -= playerCharacteristics.StaminaJumpCoast;
            }
        }

        /// <summary>
        /// Метод расхода стамины на бег
        /// </summary>
        /// <param name="RunPress"></param>
        private void RunStaminaDrain()
        {
            CanRun = ((RunPress & movementController.IsGrounded) & Stamina > 0);

            if(CanRun)
            {
                playerCharacteristics.Stamina -= playerCharacteristics.RunStaminaDrain * Time.deltaTime;
            }
        }

        /// <summary>
        /// Метод расхода стамины на кувырок
        /// </summary>
        private void RollStaminaDrain()
        {
            CanRoll = ((RollPress & movementController.IsGrounded) & Stamina > playerCharacteristics.StaminaRollCoast);

            if(CanRoll)
            {
                playerCharacteristics.Stamina -= playerCharacteristics.StaminaRollCoast;
            }
        }

    }
}
