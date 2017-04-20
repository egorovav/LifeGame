using Life;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LifeWpfClient.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public static string GameFactoryPropertyName = "GameFactory";
        private LifeGameFactory FGameFactory;
        public LifeGameFactory GameFactory
        {
            get { return this.FGameFactory; }
            set
            {
                this.FGameFactory = value;
                this.NotifyPropertyChanged(GameFactoryPropertyName);
            }
        }
          
        public static string GamePropertyName = "Game";
        private LifeGame FGame = new LifeGame();
        public LifeGame Game
        {
            get { return this.FGame; }
            set 
            { 
                this.FGame = value;
                this.FGame.MaxAge = this.MaxAge;
                this.FGame.MaxNeighbours = this.MaxNeighbours;
                this.FGame.MinNeighbours = this.MinNeighbours;
                this.FGame.NeighboursForNew = this.NeighboursForNew;

                GameFieldChanged();

                //this.FRunCommand.CanExecute(this);
                //this.FStepCommand.CanExecute(this);
                this.FResetCommand.CanExecute(this);
                NotifyPropertyChanged(GamePropertyName);
            }
        }

        public string FileName
        {
            get;
            set;
        }

        #region = Properties = 

        public static string GameHeightPropertyName = "GameHeight";
        private int FGameHeight = 74;
        public int GameHeight
        {
            get { return this.FGameHeight; }
            set
            {
                this.FGameHeight = value;
                this.FGameFactory = null;
                NotifyPropertyChanged(GameHeightPropertyName);
            }
        }

        public static string GameWidthPropertyName = "GameWidth"; 
        private int FGameWidth = 74;
        public int GameWidth
        {
            get { return this.FGameWidth; }
            set 
            { 
                this.FGameWidth = value;
                this.FGameFactory = null;
                NotifyPropertyChanged(GameWidthPropertyName);
            }
        }

        public static string StartItemCountPropertyName = "StartItemCount";
        private int FStartItemCount= 0;
        public int StartItemCount
        {
            get { return this.FStartItemCount; }
            set 
            {
                this.FStartItemCount = value;
                NotifyPropertyChanged(StartItemCountPropertyName);
            }
        }

        private string[] FGameTypes = Enum.GetNames(typeof(GameType));
        public string[] GameTypes
        {
            get { return this.FGameTypes; }
        }

        public static string GameTypePropertyName = "GameType";
        private GameType FGameType = LifeWpfClient.ViewModel.GameType.Hexagon;
        public GameType GameType
        {
            get { return this.FGameType; }
            set
            {
                this.FGameType = value;
                this.FGameFactory = null;
                NotifyPropertyChanged(GameTypePropertyName);
            }

        }

        public static string MaxAgePropertyName = "MaxAge";
        private int FMaxAge = 0;
        public int MaxAge
        {
            get { return this.FMaxAge; }
            set
            {
                this.FMaxAge = value;
                this.Game.MaxAge = value;
                NotifyPropertyChanged(MaxAgePropertyName);
            }
        }

        public static string MaxNeighboursPropertyName = "MaxNeighbours";
        private int FMaxNeighbours = 4;
        public int MaxNeighbours
        {
            get { return this.FMaxNeighbours; }
            set
            {
                this.FMaxNeighbours = value;
                this.Game.MaxNeighbours = value;
                NotifyPropertyChanged(MaxNeighboursPropertyName);
            }
        }

        public static string MinNeighboursPropertyName = "MinNeighbours";
        private int FMinNeighbours = 3;
        public int MinNeighbours
        {
            get { return this.FMinNeighbours; }
            set
            {
                this.FMinNeighbours = value;
                this.Game.MinNeighbours = value;
                NotifyPropertyChanged(MinNeighboursPropertyName);
            }
        }

        public static string NeighboursForNewPropertyName = "NeighboursForNew";
        private int FNeighboursForNew = 2; 
        public int NeighboursForNew
        {
            get { return this.FNeighboursForNew; }
            set
            {
                this.FNeighboursForNew = value;
                this.Game.NeighboursForNew = value;
                NotifyPropertyChanged(NeighboursForNewPropertyName);
            }
        }

        public static string IsSavingFramesPropertyName = "IsSavingFrames";
        private bool FIsSavingFrames = false;
        public bool IsSavingFrames
        {
            get { return this.FIsSavingFrames; }
            set
            {
                this.FIsSavingFrames = value;
                NotifyPropertyChanged(IsSavingFramesPropertyName);
            }
        }

        #endregion

        public static string StepCountPropertyName = "StepCount";
        public int StepCount
        {
            get { return this.Game.StepCount; }
        }

        public static string ItemCountPropertyName = "ItemCount";
        public int ItemCount
        {
            get { return this.Game.ItemCount; }
        }

        public static string IsRunPropertyName = "IsRun";
        private bool FIsRun = false;
        public bool IsRun
        {
            get { return this.FIsRun; }
            set 
            { 
                this.FIsRun = value;
                this.FRunCommand.CanExecute(this);
                this.FStopCommand.CanExecute(this);
                this.FResetCommand.CanExecute(this);
                this.FStepCommand.CanExecute(this);
                this.FCreateGameCommand.CanExecute(this);
                NotifyPropertyChanged(IsRunPropertyName);
            }
        }

        public static object GameFieldLocker = new object();

        private bool FFieldIsDrawed;
        public bool FieldIsDrew
        {
            get { return this.FFieldIsDrawed; }
            set 
            { 
                this.FFieldIsDrawed = value;
                if (!value)
                {
                    this.FRenderingWatcher.Reset();
                    FRenderingWatcher.Start();
                }
                else
                {
                    FRenderingWatcher.Stop();
                    NotifyPropertyChanged(RenderingTimePropertyName);
                }
            }
        }

        public static string RenderingTimePropertyName = "RenderingTime";
        public int RenderingTime
        {
            get { return (int)this.FRenderingWatcher.ElapsedMilliseconds; }
        }

        CancellationTokenSource FRunCancelationTokenSource;
        private Stopwatch FRenderingWatcher = new Stopwatch();
        private int FMinWaitTime = 200;

        public async void Go()
        {
            if (this.IsRun)
                return;

            this.FRunCancelationTokenSource = new CancellationTokenSource();
            this.FRunCancelationTokenSource.Token.Register(() => this.IsRun = false);
            this.IsRun = true;
            this.IsRun = await Task.Run<bool>(() =>
            {
                try
                {
                    while (!this.FRunCancelationTokenSource.Token.IsCancellationRequested)
                    {
                        if (this.FieldIsDrew)
                        {
                            this.GameStep();

                            if (this.Game.ItemCount == 0)
                                return false;

                            int _waitTime = this.FMinWaitTime - this.RenderingTime;
                            _waitTime = _waitTime > 0 ? _waitTime : 0;
                            Thread.Sleep(_waitTime);
                        }
                    }
                    return false;
                }
                catch (Exception exc)
                {
                    StringBuilder _sb = new StringBuilder(exc.Message);
                    Exception _temp = exc;
                    while (_temp.InnerException != null)
                    {
                        _temp = _temp.InnerException;
                        _sb.AppendLine(_temp.Message);
                    }
                    _sb.AppendLine(exc.StackTrace);
                    return false;
                }

            }, this.FRunCancelationTokenSource.Token);
        }

        public static string GameFieldPropertyName = "GameField";
        public void GameStep()
        {
            lock (GameFieldLocker)
            {
                this.Game.Step();
                this.FieldIsDrew = false;
            }

            GameFieldChanged();
        }

        private void GameFieldChanged()
        {
            NotifyPropertyChanged(GameFieldPropertyName);
            NotifyPropertyChanged(StepCountPropertyName);
            NotifyPropertyChanged(ItemCountPropertyName);

            this.FRunCommand.CanExecute(this);
            this.FStepCommand.CanExecute(this);
        }

        public void ChangeCellItem(int aCellIndex)
        {
            lock (GameFieldLocker)
            {
                Life.Cell _selectedCell = this.Game.GameField[aCellIndex];
                if (_selectedCell.CellItem == null)
                    this.Game.InitialCell(aCellIndex);
                else
                    _selectedCell.Clear();
            }

            GameFieldChanged();
            //this.FRunCommand.CanExecute(this);
            //this.FStepCommand.CanExecute(this);
        }

		private int FItemSize = 10;
		public GameFieldFrameworkElement Visual
		{
			get { return this.GameFactory.GetGameFieldFrameworkElement(this.FItemSize); }
		}

        #region = Commands =

        private DelegateCommand FRunCommand = new DelegateCommand(ExecuteRun, CanExecuteRun);
        public DelegateCommand RunCommand
        {
            get { return this.FRunCommand; }
        }

        private static void ExecuteRun(object aCommandData)
        {
            MainViewModel _model = (MainViewModel)aCommandData;
            _model.Go();
        }

        private static bool CanExecuteRun(object aCommandData)
        {
            if (aCommandData == null)
                return false;

            MainViewModel _model = (MainViewModel)aCommandData;
            return !_model.IsRun && _model.Game.GameField != null && _model.ItemCount > 0;
        }

        private DelegateCommand FStopCommand = new DelegateCommand(ExecuteStop, CanExecuteStop);
        public DelegateCommand StopCommand
        {
            get { return this.FStopCommand; }
        }

        private static void ExecuteStop(object aCommandData)
        {
            MainViewModel _model = (MainViewModel)aCommandData;
            _model.RunCancel();
        }

        private static bool CanExecuteStop(object aCommandData)
        {
            if (aCommandData == null)
                return false;

            MainViewModel _model = (MainViewModel)aCommandData;
            return _model.IsRun;
        }

        private DelegateCommand FStepCommand = new DelegateCommand(ExecuteStep, CanExecuteStep);
        public DelegateCommand StepCommand
        {
            get { return this.FStepCommand; }
        }

        private static void ExecuteStep(object aCommandData)
        {
            MainViewModel _model = (MainViewModel)aCommandData;
            _model.GameStep();
        }

        private static bool CanExecuteStep(object aCommandData)
        {
            return CanExecuteRun(aCommandData);
        }

        private DelegateCommand FResetCommand = new DelegateCommand(ExecuteReset, CanExecuteReset);
        public DelegateCommand ResetCommand
        {
            get { return this.FResetCommand; }
        }

        private static void ExecuteReset(object aCommandData)
        {
            MainViewModel _model = (MainViewModel)aCommandData;
            _model.Game.Reset();
            _model.GameFieldChanged();
        }

        private static bool CanExecuteReset(object aCommandData)
        {
            if (aCommandData == null)
                return false;

            MainViewModel _model = (MainViewModel)aCommandData;
            return !_model.IsRun && _model.Game.GameField != null;
        }

        private DelegateCommand FCreateGameCommand = new DelegateCommand(ExecuteCreateGame, CanExecuteCreateGame);
        public DelegateCommand CreateGameCommand
        {
            get { return this.FCreateGameCommand; }
            set { this.FCreateGameCommand = value; }
        }

        private static bool CanExecuteCreateGame(object aCommandData)
        {
            MainViewModel _viewModel = (MainViewModel)aCommandData;
            return aCommandData == null || !_viewModel.IsRun;
        }

        private static void ExecuteCreateGame(object aCommandData)
        {
            MainViewModel _model = (MainViewModel)aCommandData;
            ExecuteStop(aCommandData);

            if (_model.GameFactory == null)
            {
                switch (_model.GameType)
                {
                    case GameType.Hexagon:
                        _model.GameFactory = new HexagonLifeGameFacrory(_model.GameWidth, _model.GameHeight);
                        break;
                    case GameType.Square:
                        _model.GameFactory = new SquareLifeGameFactory(_model.GameWidth, _model.GameHeight);
                        break;
                    default: break;
                }
            }        
            _model.Game = _model.GameFactory.GetLifeGame(_model.StartItemCount);
        }

		DelegateCommand FIncrementFieldSize = new DelegateCommand((object data) =>
		{
			var _viewModel = (MainViewModel)data;
			//if (_viewModel.Game.GameField is RectangleGameField)
			//{
			//	var _rectField = (RectangleGameField)_viewModel.Game.GameField;
			//	_rectField.SetSize(_rectField.GameWidth + 1, _rectField.GameHeight + 1);
			_viewModel.FItemSize++;
				_viewModel.NotifyPropertyChanged(MainViewModel.GameFactoryPropertyName);
			//}
		});
		public DelegateCommand IncrementFieldSize 
		{
			get { return this.FIncrementFieldSize; }
		}

		DelegateCommand FDecrementFieldSize = new DelegateCommand((object data) =>
		{
			var _viewModel = (MainViewModel)data;
			//if (_viewModel.Game.GameField is RectangleGameField)
			//{
			//var _rectField = (RectangleGameField)_viewModel.Game.GameField;
			//_rectField.SetSize(_rectField.GameWidth - 1, _rectField.GameHeight - 1);
			_viewModel.FItemSize--;
				_viewModel.NotifyPropertyChanged(MainViewModel.GameFactoryPropertyName);
			//}
		});
		public DelegateCommand DecrementFieldSize
		{
			get { return this.FDecrementFieldSize; }
		}

        #endregion


        public void RunCancel()
        {
            if (this.FRunCancelationTokenSource != null)
            {
                this.FRunCancelationTokenSource.Cancel();
            }
        }
    }

    public enum GameType
    {
        Hexagon,
        Square
    }
}
