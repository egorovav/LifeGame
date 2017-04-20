using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using LifeWpfClient.ViewModel;
using Microsoft.Win32;
using System.IO;

namespace LifeWpfClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.MenuViewModel.MainViewModel = this.ViewModel;
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == MainViewModel.GameFieldPropertyName)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    lock (MainViewModel.GameFieldLocker)
                    {
                        this.FGameFieldVisual.DrawItems();
                        this.ViewModel.FieldIsDrew = true;

                        //if (this.FGameFieldVisual.MaxX > 0)
                        if(this.ViewModel.IsSavingFrames && this.FGameFieldVisual.ActualWidth > 0)
                        {
                            RenderTargetBitmap _bitmap = CreateVisualBitmap();
                            _encoder.Frames.Add(BitmapFrame.Create(_bitmap));
                        }
                    }
                });
            }

            if (e.PropertyName == BaseViewModel.ErrorStringPropertyName)
                MessageBox.Show(this.ViewModel.ErrorString);

            if (e.PropertyName == MainViewModel.GameFactoryPropertyName)
            {
				this.FGameFieldVisual = this.ViewModel.Visual;

				this.FGameFieldVisual.MouseDown += FGameField_MouseDown;
                this.cField.Children.Clear();
                this.cField.Children.Add(this.FGameFieldVisual);
                this.cField.Width = this.FGameFieldVisual.Width; 
                this.cField.Height = this.FGameFieldVisual.Height;
            }

            if(e.PropertyName == MainViewModel.IsRunPropertyName)
            {
                if (this.ViewModel.IsRun)
                {
                    this._encoder = new GifBitmapEncoder();
                    this.FGameFieldVisual.DrawHeader(String.Format("age={0},max={1},min={2},new={3}",
                        this.ViewModel.MaxAge, this.ViewModel.MaxNeighbours, this.ViewModel.MinNeighbours, this.ViewModel.NeighboursForNew));
                }
            }
        }

        GifBitmapEncoder _encoder = new GifBitmapEncoder();

        GameFieldFrameworkElement FGameFieldVisual;

        private  MainViewModel ViewModel
        {
            get { return (MainViewModel)this.gMain.DataContext; }
        }

        private MenuViewModel MenuViewModel
        {
            get { return (MenuViewModel)this.mMain.DataContext; }
        }

        void FGameField_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point _position = e.GetPosition(this.FGameFieldVisual);
            _position.Offset(-this.FGameFieldVisual.Margin.Left, -this.FGameFieldVisual.Margin.Top);
            int _index = this.FGameFieldVisual.GetCellIndexByPoint(_position);

            if (_index >= 0)
            {
                this.ViewModel.ChangeCellItem(_index);
            }
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog _ofd = new OpenFileDialog();
            _ofd.Filter = "Life Game Files | *.lgf";
            _ofd.AddExtension = true;
            _ofd.CheckFileExists = false;
            if (_ofd.ShowDialog() == true)
            {
                this.ViewModel.FileName = _ofd.FileName;
                this.MenuViewModel.SaveToFileCommand.Execute(this.ViewModel);
            }
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog _ofd = new OpenFileDialog();
            _ofd.Filter = "Life Game Files | *.lgf";

            if (_ofd.ShowDialog() == true)
            {
                this.ViewModel.FileName = _ofd.FileName;
                this.MenuViewModel.LoadGameFromFile.Execute(this.ViewModel);
            }
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private RenderTargetBitmap CreateVisualBitmap()
        {
            PresentationSource _ps = PresentationSource.FromVisual(this.FGameFieldVisual);
            Matrix _m = _ps.CompositionTarget.TransformToDevice;
            double _dpiX = _m.M11 * 96;
            double _dpiY = _m.M22 * 96;
            RenderTargetBitmap _bitmap = null;

            _bitmap = new RenderTargetBitmap(
                (int)this.FGameFieldVisual.ActualWidth,
                (int)this.FGameFieldVisual.ActualHeight,
                //(int)this.FGameFieldVisual.MaxX,
                //(int)this.FGameFieldVisual.MaxY,
                _dpiX, _dpiY, PixelFormats.Default);
            _bitmap.Render(this.FGameFieldVisual);
            return _bitmap;
        }

        private void SaveAsGif_Click(object sender, RoutedEventArgs e)
        {
                SaveFileDialog _sfd = new SaveFileDialog();
                _sfd.Title = "Save Game As GIF.";
                _sfd.Filter = "GIF Files (*.gif) | *.gif";

                if (_sfd.ShowDialog() == true)
                {
                    using (FileStream _fs = new FileStream(_sfd.FileName, FileMode.Create, FileAccess.Write))
                    {
                        try
                        {
                            _encoder.Save(_fs);
                        }
                        catch (Exception _exc)
                        {
                            this.ViewModel.ErrorString = _exc.Message;
                        }
                        _fs.Flush();
                        _fs.Close();
                    }
                }

                _encoder = new GifBitmapEncoder();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Properties.Resources.Description, "Description");
        }
    }
}
