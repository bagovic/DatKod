using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DatKod
{
    class Encode
    {
        //Encode Klasse
        //Autor Jerome Hassgall

        //Initialisierung der Variablen
        byte b_Anzahl = 1, b_Zeichen1, b_Zeichen2, byteMarker;
        string s_dateipfad = "";
        string s_speicherort = "";
        FileStream fs_read;
        FileStream fs_write;
        BinaryWriter bw;
        BinaryReader br;
        public Encode(string Dateipfad, string Speicherort)
        {
            s_dateipfad = Dateipfad;
            s_speicherort = Speicherort;
            fs_read = new FileStream(s_dateipfad, FileMode.Open);
            br = new BinaryReader(fs_read);
            byteMarker = MarkerFinden(s_dateipfad);
            fs_write = new FileStream(s_speicherort, FileMode.Create);
            fs_write.Close();
            header_erstellen(byteMarker);
            fs_write = new FileStream(s_speicherort, FileMode.Append);
            bw = new BinaryWriter(fs_write);
            
        }
        private void header_erstellen(byte b_marker) 
        {
            Header o_header = new Header();
            o_header.header_erstellen(s_speicherort, s_dateipfad, b_marker);
        }
        private byte datei_auslesen()
        {
            byte byteInhalt = 0;
            if (fs_read.Position < fs_read.Length)
            {
                byteInhalt = br.ReadByte();
            }
            return byteInhalt;
        }
        public bool encoding()
        {
            try
            {
                fs_read.Position = 0;
                bool b_erstesmal = true;
                while (fs_read.Position < fs_read.Length)
                {
                    if (b_erstesmal) //wenn man vergleicht ob es  0 ist, wird das Zeichen NUL nicht erkannt
                    {
                        b_Zeichen1 = datei_auslesen();
                        b_erstesmal = false;
                    }
                    else 
                        b_Zeichen1 = b_Zeichen2; //Ab der 3. Stelle wird b_Zeichen1 �berschrieben f�r den Vergleich
                    if (fs_read.Length == 1) //Sollte die l�nge des Streams nur 1 sein wird 
                    {
                        if (b_Zeichen1 == byteMarker)
                        {
                            Ausgabe(byteMarker, 1, b_Zeichen1); //Der Marker ausgegeben
                        }
                        else
                            Ausgabe(byteMarker, 1, b_Zeichen1); //Das Zeichen ausgegeben
                    }
                    else
                    {
                        b_Zeichen2 = datei_auslesen(); //Auslesen des zum vergleichenden Zeichen
                    }
                    if (b_Anzahl == 255) //Sollte die Maximalgrenze von 255 Zeichen erreicht worden sein
                    {
                        Ausgabe(byteMarker, b_Anzahl, b_Zeichen1); //Wir ausgegeben und 
                        b_Anzahl = 1; //b_Anzahl wieder auf 1 gesetzt.
                    }
                    else
                    {
                        if (fs_read.Position == fs_read.Length) //Vergleich am Ende vom Stream
                        {
                            if (b_Zeichen1 == b_Zeichen2)
                            {
                                b_Anzahl++;
                                if (b_Anzahl >= 4) //Wenn b_Anzahl 4 oder gr��er ist wird dies ausgegeben
                                {
                                    Ausgabe(byteMarker, b_Anzahl, b_Zeichen1);
                                }
                                else //Ansonsten die Anzahl der Zeichen (1-3mal)
                                {
                                    for (int y = 0; y < b_Anzahl; y++)
                                        bw.Write(b_Zeichen1);
                                }
                            }
                            else
                            {
                                if (b_Anzahl >= 4)
                                {
                                    Ausgabe(byteMarker, b_Anzahl, b_Zeichen1); //Wenn b_Anzahl 4 oder gr��er ist wird dies ausgegeben
                                    bw.Write(b_Zeichen2); //das letzte Zeichen aber anders sein, wird dies dazu geschrieben
                                }
                                else
                                {
                                    for (int y = 0; y < b_Anzahl; y++) //Ansonsten die Anzahl der Zeichen (1-3mal)
                                        bw.Write(b_Zeichen1);
                                    bw.Write(b_Zeichen2); //und das letzte Zeichen wird hinzugef�gt, wenn dieses anders ist.
                                }
                            }
                        }
                        else
                        {
                            if (b_Zeichen1 == b_Zeichen2)
                                b_Anzahl++; //Anzahl erh�hen sobald Zeichen sich wiederholt
                            else
                            {
                                if (b_Anzahl >= 4)
                                {
                                    Ausgabe(byteMarker, b_Anzahl, b_Zeichen1); //Wenn b_Anzahl 4 oder gr��er ist wird dies ausgegeben 
                                    b_Anzahl = 1; // Anzahl auf 1 setzen
                                }
                                else
                                {
                                    if (b_Zeichen1 == byteMarker) //Sollte das Zeichen der Marker sein gibt er dies aus
                                    {
                                        Ausgabe(byteMarker, 1, b_Zeichen1);
                                    }
                                    else
                                    {
                                        for (int y = 0; y < b_Anzahl; y++) //Ansonsten die Anzahl der Zeichen (1-3mal)
                                            bw.Write(b_Zeichen1); 
                                        b_Anzahl = 1; //b_Anzahl wieder auf 1 um das vergleichen neu zu beginnen
                                    }
                                }
                            }
                        }

                    }
                }
                br.Close();
                bw.Close();
                fs_read.Close();
                fs_write.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void Ausgabe(byte b1, byte b2, byte b3)
        {
            bw.Write(b1); //Schreiben des Markers
            bw.Write(b2); //Schreiben der Anzahl
            bw.Write(b3); //Schreiben des Zeichen
        }
        private byte MarkerFinden(string s_Dateipfad_asdf)
        {
            int i_tmp = 0;
            int[] ai_array = new int[256];
            ai_array[0] = 10000; //Stelle [0] erh�hen, das diese aufjedenfall nicht als Marker genommen wird.
            for(int x = 0; x < fs_read.Length; x++)
            {
                i_tmp = (char)datei_auslesen(); //Auslesen jedes einzelnen Bytes
                ai_array[i_tmp] += 1; //An der Stelle des Bytes um +1 erh�hen
            }
            fs_read.Position = 1; //Zu beginn des Streams h�pfen
            byte b_rw = (byte)Array.IndexOf(ai_array, ai_array.Min()); //Auswertung des Index mit der niedrigsten Stelle 
            return (b_rw);
        }
    }
}
