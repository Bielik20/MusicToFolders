using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace MusicToFolders.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public ICommand ChooseFolderCommand { get; private set; }
        public ICommand DoItCommand { get; private set; }

        //------------------------------------------------------------------------
        
        public string FolderPath
        {
            get { return folderPath; }
            private set
            {
                folderPath = value;
                OnPropertyChanged("FolderPath");
            }
        }
        private string folderPath;

        
        public Mp3Lib.Mp3File[] MusicList
        {
            get { return musicList; }
            private set
            {
                musicList = value;
                OnPropertyChanged("MusicList");
            }
        }
        private Mp3Lib.Mp3File[] musicList;


        public int CurrFile
        {
            get { return currFile; }
            set
            {
                currFile = value;
                OnPropertyChanged("CurrFile");
            }
        }
        private int currFile;


        public int NumOfFiles
        {
            get { return numOfFiles; }
            private set
            {
                numOfFiles = value;
                OnPropertyChanged("NumOfFiles");
            }
        }
        private int numOfFiles;

        //------------------------------------------------------------------------

        private string[] filePaths;
        private string[] fileNames;

        //------------------------------------------------------------------------

        public MainWindowViewModel()
        {
            ChooseFolderCommand = new RelayCommand(_ => ChooseFoler());
            DoItCommand = new RelayCommand(_ => DoIt());
        }

        //------------------------------------------------------------------------

        private void ChooseFoler()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                FolderPath = fbd.SelectedPath.ToString();
                filePaths = Directory.GetFiles(FolderPath, "*.mp3");

                NumOfFiles = filePaths.Length;
                MusicList = new Mp3Lib.Mp3File[NumOfFiles];
                fileNames = new string[NumOfFiles];   

                for (int i = 0; i < NumOfFiles; i++)
                {
                    MusicList[i] = new Mp3Lib.Mp3File(@filePaths[i]);
                    fileNames[i] = Path.GetFileName(filePaths[i]);      //Zwraca nazwy plików bez ścieżki
                }
            }
        }

        private void DoIt()
        {
            List<string> failList = new List<string>();
            for (CurrFile = 0; CurrFile < NumOfFiles; CurrFile++)
            {
                try
                {
                    Directory.CreateDirectory(@FolderPath + @"\" + MusicList[CurrFile].TagHandler.Artist + @"\" + MusicList[CurrFile].TagHandler.Album);
                    System.IO.File.Move(filePaths[CurrFile], @FolderPath + @"\" + MusicList[CurrFile].TagHandler.Artist + @"\" + MusicList[CurrFile].TagHandler.Album + @"\" + fileNames[CurrFile]);
                } catch (Exception) { failList.Add(fileNames[CurrFile]); }
            }


            var message = string.Join(Environment.NewLine, failList);
            MessageBox.Show("Done! Files that couldn't be completed: \n\n" + message);
            
        }


    }
}
