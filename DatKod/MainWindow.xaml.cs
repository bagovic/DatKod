using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Microsoft.Win32;

namespace DatKod
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }
        #region Variablen
        private string s_dateipfad_encode = "";
        private string s_speicherort_decode = "";
        private string s_dateipfad_decode = "";
        private string s_speicherort_encode = "";
        Header o_header = new Header();
        #endregion

        private void Decode_start(object sender, RoutedEventArgs e)
        {
            bool b_dateipfad = false;
            bool b_speicherort = false;
            if (!string.IsNullOrWhiteSpace(s_dateipfad_decode)) b_dateipfad = true;
            else MessageBox.Show("FEHLER: Es wurde keine Datei zum enkodieren ausgewählt!");
            if (!string.IsNullOrWhiteSpace(s_speicherort_decode)) b_speicherort = true;
            else MessageBox.Show("FEHLER: Es wurde kein Speicherort ausgewählt!");
            if (b_dateipfad && b_speicherort)
            {
                byte byteMarker = o_header.marker_auslesen(s_dateipfad_decode);
                Decode o_decode = new Decode(s_dateipfad_decode, s_speicherort_decode, o_header.ReadPosition, byteMarker);
                bool b_decodeOK = o_decode.enkodieren();
                if (b_decodeOK)
                    MessageBox.Show("Die Datei wurde erfolgreich enkodiert.");
                else
                    MessageBox.Show("Beim enkodieren ist ein Fehler aufgetreten.");
            }
            Variablen_leeren(false); //wenn false, werden die Encode Pfade geleert.
        }
        private void Encode_start(object sender, RoutedEventArgs e)
        {
            bool b_dateipfad = false;
            bool b_speicherort = false;
            if (!string.IsNullOrWhiteSpace(s_dateipfad_encode)) b_dateipfad = true;
            else MessageBox.Show("FEHLER: Es wurde keine Datei zum kodieren ausgewählt!");
            if (!string.IsNullOrWhiteSpace(s_speicherort_encode)) b_speicherort = true;
            else MessageBox.Show("FEHLER: Es wurde kein Speicherort ausgewählt!");
            if (b_dateipfad && b_speicherort)
            {
                //o_header.header_erstellen(s_speicherort_encode, s_dateipfad_encode);
                Encode o_encode = new Encode(s_dateipfad_encode, s_speicherort_encode);
                bool b_encodeOK = o_encode.encoding();
                if (b_encodeOK)
                    MessageBox.Show("Die Datei wurde erfolgreich kodiert.");
                else
                    MessageBox.Show("Bein kodieren ist ein unbekannter Fehler aufgetreten.");
            }
            Variablen_leeren(true); //wenn true, werden die Decode Pfade gleert
        }
        private void FileManagerOpen(bool b_encode)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Datei auswählen!";
            //ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!b_encode) ofd.Filter = "BAJ-Datei (*.baj)|*.baj";
            bool b_DateiAusgewaehlt = (bool)ofd.ShowDialog();
            if (b_DateiAusgewaehlt && !string.IsNullOrWhiteSpace(ofd.FileName))
            {
                if (b_encode)
                {
                    s_dateipfad_encode = ofd.FileName;
                    tb_encodePfad.Text = ofd.FileName;
                    Variablen_leeren(b_encode);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(ofd.FileName))
                    {
                        if (!o_header.header_pruefen(ofd.FileName))
                        {
                            ofd.FileName = null;
                            MessageBox.Show("FEHLER: Die ausgewählte Datei ist keine gültige BAJ-Datei!");
                        }
                    }
                    s_dateipfad_decode = ofd.FileName;
                    tb_decodePfad.Text = ofd.FileName;
                    Variablen_leeren(b_encode);
                }
            }
            if (string.IsNullOrWhiteSpace(ofd.FileName))
            {
                Variablen_leeren(b_encode);
            }

        }
        private void FileManagerSave(bool b_encode)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Speicherort auswählen!";
            if (b_encode) sfd.Filter = "BAJ-Datei (*.baj)|*.baj";
            else if (!string.IsNullOrWhiteSpace(s_dateipfad_decode))
            {
                sfd.Filter = "Alle Dateien (*.*)|*.*|" +
                    "Textdateien (*.txt)|*.txt|" +
                    "Word-Dateien (*.docx)|*.docx|" +
                    "Word-Dateien (*.doc)|*.doc";
                sfd.FileName = o_header.header_auslesen(s_dateipfad_decode);
            }
            bool b_DateiAusgewaehlt = (bool)sfd.ShowDialog();
            if (b_DateiAusgewaehlt)
            {
                if (b_encode)
                {
                    s_speicherort_encode = sfd.FileName;
                    tb_speicherEncodePfad.Text = sfd.FileName;
                    Variablen_leeren(b_encode);
                }
                else
                {
                    s_speicherort_decode = sfd.FileName;
                    tb_speicherDecodePfad.Text = sfd.FileName;
                    Variablen_leeren(b_encode);
                }
            }
            else if (string.IsNullOrWhiteSpace(sfd.FileName))
            {
                Variablen_leeren(b_encode);
            }
        }
        private void Variablen_leeren(bool b_encode)
        {
            //wenn true, werden die Decode Pfade gleert
            //wenn false, werden die Encode Pfade geleert.
            if (b_encode)
            {
                s_dateipfad_decode = "";
                s_speicherort_decode = "";
                tb_decodePfad.Text = "Pfad auswählen";
                tb_speicherDecodePfad.Text = "Pfad auswählen";
            }
            else
            {
                s_dateipfad_encode = "";
                s_speicherort_encode = "";
                tb_encodePfad.Text = "Pfad auswählen";
                tb_speicherEncodePfad.Text = "Pfad auswählen";
            }
        }
        private void EncodeOpen(object sender, RoutedEventArgs e)
        {
            FileManagerOpen(true);
        }

        private void DecodeOpen(object sender, RoutedEventArgs e)
        {
            FileManagerOpen(false);
        }

        private void EncodeSave(object sender, RoutedEventArgs e)
        {
            FileManagerSave(true);
        }

        private void DecodeSave(object sender, RoutedEventArgs e)
        {
            FileManagerSave(false);
        }
        //private void EncodeHotkey(Object sender, RoutedEventArgs e)
        //{
        //    Encode_start(sender, e);
        //}
    }
}
