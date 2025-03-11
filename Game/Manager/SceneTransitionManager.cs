using System;
using UnityEngine;
using QFramework;
using DG.Tweening;
using UnityEngine.PlayerLoop;

namespace TPL.PVZR
{
	public partial class SceneTransitionManager : MonoSingleton<SceneTransitionManager>, IController
	{
		public IArchitecture GetArchitecture()
		{
			return PVZRouge.Interface;
		}
		private UISceneTransitionPanel _UISceneTransitionPanel;

		private enum TransitionState
		{
			Hiding, Hide, Displaying, Display
		}

		private void Update()
		{
			_currentState.Update();
		}

		private void Awake()
		{
			_UISceneTransitionPanel = UIKit.OpenPanel<UISceneTransitionPanel>(UILevel.PopUI);

			_currentState = new FSM<TransitionState>();
			_currentState.State(TransitionState.Display)
				.OnUpdate(() =>
				{
					if (_targetState == TransitionState.Hide)
					{
						_currentState.ChangeState(TransitionState.Hiding);
					}
				});
			// 隐藏
			_currentState.State(TransitionState.Hiding)
				.OnEnter(() =>
				{
					_UISceneTransitionPanel.Bg.DOFade(1, 0.2f).OnComplete(() =>
					{
						_currentState.ChangeState(TransitionState.Hide);
					});
				});
			//
			_currentState.State(TransitionState.Hide)
				.OnUpdate(() =>
				{
					if (_targetState == TransitionState.Display)
					{
						_currentState.ChangeState(TransitionState.Displaying);
					}
				});
			// 显示
			_currentState.State(TransitionState.Displaying)
				.OnEnter(() =>
				{

					_UISceneTransitionPanel.Bg.DOFade(0, 0.2f).OnComplete(() =>
					{
						_currentState.ChangeState(TransitionState.Display);
					});
				});
			
			_currentState.StartState(TransitionState.Display);
		}

		private TransitionState _targetState;
		private FSM<TransitionState> _currentState;

		public static void Hide()
		{
			Instance._targetState = TransitionState.Hide;
			if (Instance._currentState.CurrentStateId == TransitionState.Display)
			{
				Instance._currentState.ChangeState(TransitionState.Hiding);
			}
		}

		public static void Display()
		{
			Instance._targetState = TransitionState.Display;
			if (Instance._currentState.CurrentStateId == TransitionState.Hide)
			{
				Instance._currentState.ChangeState(TransitionState.Displaying);
			}
		}

		private float _timer;
	}
}
