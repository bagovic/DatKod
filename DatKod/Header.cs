using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DatKod
{
    class Header
    {
        //Header Klasse
        //Autor Bermin Agovic
        long l_streamlong = 1;
        long l_readpos = 0;
        public bool header_pruefen(string s_dateipfad)
        {
            //Überpruft ob die ersten 3 Bytes BAJ sind, wenn ja ist das eine von uns erstellte Datei
            l_readpos = 0;
            byte byteZeichen1 = datei_auslesen(s_dateipfad);
            byte byteZeichen2 = datei_auslesen(s_dateipfad);
            byte byteZeichen3 = datei_auslesen(s_dateipfad);
            if(byteZeichen1 == 66 && byteZeichen2 == 65 && byteZeichen3 == 74)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public long ReadPosition
        {
            get
            {
                //wird der Decode-Klasse übergeben, damit es weiß wo es mit dem Lesen anfange soll
                return l_readpos; 
            }
        }
        public void header_erstellen(string s_speicherort, string s_dateipfad, byte byteMarker)
        {
            //BAJ wird angefuegt
            datei_speichern(66, s_speicherort);
            datei_speichern(65, s_speicherort);
            datei_speichern(74, s_speicherort);
            datei_speichern(byteMarker, s_speicherort);
            //Der Dateiname wird aus dem Pfad ausgelesen
            string[] as_dateiname = dateiname_auslesen(s_dateipfad);
            //Name und Endung werden getrennt, um sie abschneiden zu können.
            string s_name_reverse = as_dateiname[0];
            string s_endung_reverse = as_dateiname[1];
            //Dateiname und -endung werden ab ihrer Grenze abgeschnitten und automatisch in richtiger Reihenfolge gebracht
            string s_name = string_abschneiden(s_name_reverse, 8);
            string s_endung = string_abschneiden(s_endung_reverse, 5);
            //Dateiname und -endung  werden angefuegt
            for (int i = 0; i < s_name.Length; i++)
            {
                byte byteZeichen = (byte)s_name[i];
                datei_speichern(byteZeichen, s_speicherort);
            }
            for (int i = 0; i < s_endung.Length; i++)
            {
                byte byteZeichen = (byte)s_endung[i];
                datei_speichern(byteZeichen, s_speicherort);
            }
            //Ein # wird angefuegt um das Ende des Headers zu kennzeichnen
            datei_speichern(27, s_speicherort); 
        }
        private string[] dateiname_auslesen(string s_pfad)
        {
            string s_name_reverse = "";
            string s_endung_reverse = "";
            bool b_name = false;
            for (int i = s_pfad.Length - 1; i > 0; i--)
            {
                if (s_pfad[i] == '\\')
                {
                    i = 0;
                }
                else if (s_pfad[i] == 46 && !b_name) //ist es ein Punkt, ist die Dateiendung zu Ende
                {
                    s_endung_reverse = s_endung_reverse + s_pfad[i].ToString();
                    b_name = true;
                }
                else if (!b_name) //wenn b_Name false ist, wird alles in das Endungs-Array gespeichert
                {
                    s_endung_reverse = s_endung_reverse + s_pfad[i].ToString();
                }
                else if (b_name) //wenn true wird alles in das Namen-Array gespeichert.
                {
                    s_name_reverse = s_name_reverse + s_pfad[i].ToString();
                }
            }
            string[] as_dateiname = new string[2] { s_name_reverse, s_endung_reverse };
            return as_dateiname;
        }
        private string string_abschneiden(string str, int i_grenze)
        {
            int i_abbruchbedingung = 0;
            string s_text = "";
            if (str.Length + 1 > i_grenze + 1) // + 1 fuer den punkt
            {
                i_abbruchbedingung = str.Length - i_grenze; // die Abbruchbedingung ist die Differenz zwischen der Grenze und der tatsächlichen Größe
            }
            for (int i = str.Length - 1; i >= i_abbruchbedingung; i--) // wenn i kleiner als die Abbruchbedingung ist, wurde die Grenze erreicht
            {
                s_text = s_text + str[i];
            }
            if(str.Length > i_grenze)
            {
                s_text = s_text + "~";
            }
            return s_text;
        }
        public byte marker_auslesen(string s_dateipfad)
        {
            long l_tmp = l_readpos;
            l_readpos = 3;
            byte byteMarker = datei_auslesen(s_dateipfad);
            l_readpos = l_tmp;
            return byteMarker;
        }
        public string header_auslesen(string s_dateipfad)
        {
            l_readpos = 4;
            string s_originalname = "";
            int i_groesse = dateinamegroesse(s_dateipfad);
            for (int i = 0; i < i_groesse; i++) //Dateiname kann (mit Punkt) nicht länger als 13 Bit sein.
            {
                char c_Inhalt = (char)datei_auslesen(s_dateipfad); //der Inhalt wird in einer Variable zu speichern, um zu ueberpruefen, ob das Zeichen ein # ist
                if (c_Inhalt == (byte)27) //wenn true, ist Ende des Dateinamens erreicht.
                {
                    i = i_groesse; //um abbruchbedingung zu erfüllen
                }
                else
                {
                    s_originalname = s_originalname + c_Inhalt.ToString();
                }
            }
            return s_originalname;
        }
        private int dateinamegroesse(string s_dateipfad)
        {
            bool b_endung = false;
            int i_groeese_datei = 0;
            int i_groesse_endung = 0;
            for (int i = 0; i < s_dateipfad.Length; i++)
            {
                if (s_dateipfad[i] == 46)
                {
                    b_endung = true;
                }
                else if (!b_endung)
                {
                    i_groeese_datei++;
                }
                else
                {
                    i_groesse_endung++;
                }
            }
            int i_groesse = i_groeese_datei + i_groesse_endung + 1;
            return i_groesse;
        }
        private byte datei_auslesen(string s_dateipfad)
        {
            byte byteInhalt = 0;
            FileStream fs = new FileStream(s_dateipfad, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            l_streamlong = fs.Length;
            fs.Position = l_readpos;
            if (fs.Position < fs.Length)
            {
                byteInhalt = br.ReadByte();
            }
            l_readpos = fs.Position; //Position wird zwischen gespeichert um nicht von neu anzufangen
            br.Close();
            fs.Close();
            return byteInhalt;
        }
        private void datei_speichern(byte byteInhalt, string s_speicherort)
        {
            FileStream fs = new FileStream(s_speicherort, FileMode.Append);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(byteInhalt);
            bw.Close();
            fs.Close();
        }
    }
}
