using Life;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LifeWpfClient.ViewModel
{
    public class MenuViewModel : BaseViewModel
    {
        private MainViewModel FMainViewModel;
        public MainViewModel MainViewModel
        {
            set { this.FMainViewModel = value; }
        }

        private DelegateCommand FSaveToFileCommand = new DelegateCommand(ExecSaveToFile, CanExecSaveToFile);
        public DelegateCommand SaveToFileCommand
        {
            get { return this.FSaveToFileCommand; }
            set { this.FSaveToFileCommand = value; }
        }

        private static bool CanExecSaveToFile(object aCommandData)
        {
            return true;
        }

        private static void ExecSaveToFile(object aCommandData)
        {
            MainViewModel _viewModel = (MainViewModel)aCommandData;
            _viewModel.RunCancel();
            if(_viewModel.GameFactory == null)
            {
                _viewModel.ErrorString = "Parameters has been changed. Create new game.";
                return;
            }
            GameData _game = new GameData() 
                { 
                  GameType = _viewModel.GameType, 
                  Game = _viewModel.Game,
                  GameFactory = _viewModel.GameFactory
                };
            try
            {
                XmlSerializer _xs = new XmlSerializer(typeof(GameData));

                using (FileStream _fs = new FileStream(_viewModel.FileName, FileMode.Create))
                {
                    _xs.Serialize(_fs, _game);
                }
            }
            catch (Exception exc)
            {
                StringBuilder _sb = GetErrorString(exc);
                _viewModel.ErrorString = _sb.ToString();
            }
        }

        private DelegateCommand FLoadGameFromFile = new DelegateCommand(ExecLoadGameFromFile, CanLoadGameFromFile);
        public DelegateCommand LoadGameFromFile
        {
            get { return this.FLoadGameFromFile; }
            set { this.FLoadGameFromFile = value; }
        }

        private static bool CanLoadGameFromFile(object arg)
        {
            return true;
        }

        private static void ExecLoadGameFromFile(object aCommandData)
        {
            MainViewModel _viewModel = (MainViewModel)aCommandData;
            _viewModel.RunCancel();

            try
            {
                XmlSerializer _xs = new XmlSerializer(typeof(GameData));

                using (FileStream _fs = new FileStream(_viewModel.FileName, FileMode.Open))
                {
                    GameData _gameData = (GameData)_xs.Deserialize(_fs);
                    if(_gameData.GameFactory.GameField is RectangleGameField)
                    {
                        RectangleGameField _rgf = (RectangleGameField)_gameData.GameFactory.GameField;
                        _viewModel.GameWidth = _rgf.GameWidth;
                        _viewModel.GameHeight = _rgf.GameHeight;
                        
                    }
                    _viewModel.GameType = _gameData.GameType;
                    
                    _viewModel.GameFactory = _gameData.GameFactory;

                    _viewModel.MaxAge = _gameData.Game.MaxAge;
                    _viewModel.MaxNeighbours = _gameData.Game.MaxNeighbours;
                    _viewModel.MinNeighbours = _gameData.Game.MinNeighbours;
                    _viewModel.NeighboursForNew = _gameData.Game.NeighboursForNew;                  

                    _viewModel.Game = _gameData.Game;
                }
            }
            catch (Exception exc)
            {
                StringBuilder _sb = GetErrorString(exc);
                _viewModel.ErrorString = _sb.ToString();
            }
        }

        public class GameData : IXmlSerializable
        {
            public GameType GameType { get; set; }
            public LifeGameFactory GameFactory { get; set; }
            public LifeGame Game { get; set; }
            public ItemData[] Position { get; set; }

            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                reader.ReadToFollowing("GameType");
                string _typeString = reader.ReadElementContentAsString();
                this.GameType = (GameType)Enum.Parse(typeof(GameType), _typeString);

                switch(this.GameType)
                {
                    case ViewModel.GameType.Hexagon :
                        {
                            XmlSerializer _xs = new XmlSerializer(typeof(HexagonLifeGameFacrory));
                            this.GameFactory = (HexagonLifeGameFacrory)_xs.Deserialize(reader);
                            break;
                        }
                    case ViewModel.GameType.Square :
                        {
                            XmlSerializer _xs = new XmlSerializer(typeof(SquareLifeGameFactory));
                            this.GameFactory = (SquareLifeGameFactory)_xs.Deserialize(reader);
                            break;
                        }
                    default: break;
                }

                XmlSerializer _gxs = new XmlSerializer(typeof(LifeGame));
                LifeGame _game = (LifeGame)_gxs.Deserialize(reader);
                _gxs = new XmlSerializer(typeof(ItemData[]));
                ItemData[] _position = (ItemData[])_gxs.Deserialize(reader);

                this.Game = this.GameFactory.GetLifeGame(0);
                this.Game.MaxAge = _game.MaxAge;
                this.Game.MaxNeighbours = _game.MaxNeighbours;
                this.Game.MinNeighbours = _game.MinNeighbours;
                this.Game.NeighboursForNew = _game.NeighboursForNew;
                this.Game.SetPosition(_position);
            }

            public void WriteXml(XmlWriter writer)
            {
                writer.WriteElementString("GameType", Enum.GetName(typeof(GameType), this.GameType));
                XmlSerializer _xs = new XmlSerializer(this.GameFactory.GetType());
                _xs.Serialize(writer, this.GameFactory);
                _xs = new XmlSerializer(typeof(LifeGame));
                _xs.Serialize(writer, this.Game);
                _xs = new XmlSerializer(typeof(ItemData[]));
                _xs.Serialize(writer, this.Game.CurrentPosition);
            }
        }
    }
}
