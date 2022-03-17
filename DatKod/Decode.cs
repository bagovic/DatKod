using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DatKod
{
    public class Decode
    {
        //Decode Klasse
        //Autor Bermin Agovic
        byte byteMarker = 0;
        string s_dateipfad = "";
        string s_speicherort = "";
        FileStream fs_read;
        FileStream fs_write;
        BinaryWriter bw;
        BinaryReader br;
        public Decode(string Dateipfad, string Speicherort, long LesePosition, byte Marker)
        {
            byteMarker = Marker;
            s_dateipfad = Dateipfad;
            s_speicherort = Speicherort;
            fs_read = new FileStream(s_dateipfad, FileMode.Open);
            fs_read.Position = LesePosition;
            //Dieser Stream erstellt eine neue Datei bzw. leert sie falls schon was drinnen steht
            fs_write = new FileStream(s_speicherort, FileMode.Create); 
            fs_write.Close();
            fs_write = new FileStream(s_speicherort, FileMode.Append);
            br = new BinaryReader(fs_read);
            bw = new BinaryWriter(fs_write);
        }
        public bool enkodieren()
        {
            try
            {
                while (fs_read.Position < fs_read.Length)
                {
                    byte byteZeichen = datei_auslesen();
                    if (byteZeichen == byteMarker)
                    {
                        byte byteAnzahl = datei_auslesen(); //Anzahl wird ausgelesen
                        byteZeichen = datei_auslesen(); //Das Zeichen wird ausgelesen
                        for (int i = 0; i < byteAnzahl; i++)
                        {
                            bw.Write(byteZeichen);
                        }
                    }
                    else
                    {
                        bw.Write(byteZeichen);
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
        private byte datei_auslesen()
        {
            byte byteInhalt = 0;
            if (fs_read.Position < fs_read.Length)
            {
                byteInhalt = br.ReadByte();
            }
            return byteInhalt;
        }
    }

}