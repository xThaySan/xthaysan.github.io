namespace Tenet
{
    public class Cryptographie
    {
        public static string ByteArrayToString(byte[] ba)
        {
            System.Text.StringBuilder hex =
                new System.Text.StringBuilder(ba.Length * 2);
            foreach (byte b in ba) hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            bytes[i / 2] = System.Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string
        DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                return "[···xopɐɹɐd ǝɯᴉʇ···]";
            if (Key == null || Key.Length <= 0) return "[···xopɐɹɐd ǝɯᴉʇ···]";
            if (IV == null || IV.Length <= 0) return "[···xopɐɹɐd ǝɯᴉʇ···]";

            string plaintext = null;

            using (
                System.Security.Cryptography.Aes aesAlg =
                    System.Security.Cryptography.Aes.Create()
            )
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                System.Security.Cryptography.ICryptoTransform decryptor =
                    aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (
                    System.IO.MemoryStream msDecrypt =
                        new System.IO.MemoryStream(cipherText)
                )
                {
                    using (
                        System.Security.Cryptography.CryptoStream csDecrypt =
                            new System.Security.Cryptography.CryptoStream(msDecrypt,
                                decryptor,
                                System
                                    .Security
                                    .Cryptography
                                    .CryptoStreamMode
                                    .Read)
                    )
                    {
                        using (
                            System.IO.StreamReader srDecrypt =
                                new System.IO.StreamReader(csDecrypt)
                        )
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }

    public class RedTeam
    {
        private readonly System.DateTime _start;

        private System.DateTime _breakpoint;

        private System.TimeSpan _elapsedTimeA;

        private System.TimeSpan _elapsedTimeB;

        private string _encryptedRedTeamFlagHex;

        private string _redTeamKeyHex;

        private string _redTeamIVHex;

        private string _blueTeamKeyHex;

        private string _blueTeamIVHex;

        private int _step;

        private string _messageA;

        private string _messageB;

        public void SetMessageA(string messageA)
        {
            _messageA = messageA;
        }

        public void SetMessageB(string messageB)
        {
            _messageB = messageB;
        }

        public RedTeam()
        {
            _start = System.DateTime.Now;
            _breakpoint = _start;
            _elapsedTimeA = System.TimeSpan.Zero;
            _elapsedTimeB = System.TimeSpan.Zero;
            _encryptedRedTeamFlagHex = "";
            _redTeamKeyHex = "";
            _redTeamIVHex = "";
            _blueTeamKeyHex = "";
            _blueTeamIVHex = "";
            _step = 0;
            _messageA = "";
            _messageB = "";
        }

        public void Step1()
        {
            _breakpoint = System.DateTime.Now;
            _elapsedTimeA = (_breakpoint - _start);
            if (_elapsedTimeA.TotalMilliseconds > 0.0 && _step == 0)
            {
                System.Console.WriteLine("Commencing Red Team mission...");
                _step++;
            }
        }

        public void Step2()
        {
            _breakpoint = System.DateTime.Now;
            _elapsedTimeB = (_breakpoint - _start);
            if (
                _elapsedTimeA.TotalMilliseconds <
                _elapsedTimeB.TotalMilliseconds &&
                _step == 1
            )
            {
                System
                    .Console
                    .WriteLine("The Red Team is preparing for the exchange...");
                _encryptedRedTeamFlagHex =
                    "e7ca4e585c86743d573355bc2d73ac1b8d85511e13024b61dacc7c4f2adae604";
                _blueTeamKeyHex =
                    "7688b8e374e868241f61da326be1eb022128f6c33c3ddd89dda1c7303744f4bf";
                _blueTeamIVHex = "80de9488c8ca4c10aebd7dd0475b28ce";
                _step++;
            }
        }

        public void Step3(object blueTeam)
        {
            _breakpoint = System.DateTime.Now;
            _elapsedTimeA = (_breakpoint - _start);
            if (
                _elapsedTimeA.TotalMilliseconds >
                _elapsedTimeB.TotalMilliseconds &&
                _step == 2
            )
            {
                System
                    .Console
                    .WriteLine("Sending decryption key and IV to Blue Team...");

                System.Reflection.MethodInfo methodInfo =
                    blueTeam.GetType().GetMethod("SetMessageA");
                object[] arguments = new object[1];
                arguments[0] = _blueTeamKeyHex;
                methodInfo.Invoke (blueTeam, arguments);

                methodInfo = blueTeam.GetType().GetMethod("SetMessageB");
                arguments[0] = _blueTeamIVHex;
                methodInfo.Invoke (blueTeam, arguments);

                _step++;
            }
        }

        public void Step4()
        {
            _breakpoint = System.DateTime.Now;
            _elapsedTimeB = (_breakpoint - _start);
            if (
                _elapsedTimeA.TotalMilliseconds <
                _elapsedTimeB.TotalMilliseconds &&
                _step == 3
            )
            {
                System.Console.WriteLine("Receiving message from Blue Team...");
                _redTeamKeyHex = _messageA;
                _redTeamIVHex = _messageB;
                _step++;
            }
        }

        public void Step5()
        {
            _breakpoint = System.DateTime.Now;
            _elapsedTimeA = (_breakpoint - _start);
            if (
                _elapsedTimeA.TotalMilliseconds >
                _elapsedTimeB.TotalMilliseconds &&
                _step == 4
            )
            {
                System
                    .Console
                    .WriteLine("Attempting Red Team flag decryption...");
                byte[] cipherText =
                    Cryptographie.StringToByteArray(_encryptedRedTeamFlagHex);
                byte[] key = Cryptographie.StringToByteArray(_redTeamKeyHex);
                byte[] iv = Cryptographie.StringToByteArray(_redTeamIVHex);
                string decryptedRedTeamFlag =
                    Cryptographie
                        .DecryptStringFromBytes_Aes(cipherText, key, iv);
                System
                    .Console
                    .WriteLine("\nDecrypted Red Team flag : {0}",
                    decryptedRedTeamFlag);
                _step = 0;
            }
        }
    }

    public class BlueTeam
    {
        private readonly System.DateTime _VG91dCBlc3QgZGFucyBsYSB0ZXh0dXJl;

        private System.DateTime _Q2FyIGxhIHB1cullIHRyb3AgY29sbGFudGU;

        private System.TimeSpan _ZXN0IHZyYWltZW50IHRy6HMgZOljZXZhbnRl;

        private System.TimeSpan _UGVuc2Ug4CDnYSBhdmFudCBkJ2N1aXNpbmVy;

        private string _ZXQgdHUgcul1c3NpcmFzIHVuZSBib25uZSBwdXLpZQ;

        private string _VHUgdmVycmFzLA;

        private string _Yydlc3QgcGx1cyBmYWNpbGUgcXVlIOdhIGVuIGEgbCdhaXI;

        private string _RXQgc3VydG91dCwgYXBy6HMg52E;

        private string _dHUgbmUgdm91ZHJhcyBwbHVzIGQndW5lIGF1dHJlIHB1cull;

        private int _SidhaSBxdWF0cmUgaG9tbWVzIOAgbGEgbWFpc29u;

        private string _ZXQgaWxzIHNlIHLpZ2FsZW50IHRvdXM;

        private string _SWwgbidlbiByZXN0ZSBqYW1haXM;

        public void SetMessageA(string U2FsdXQ)
        {
            _ZXQgaWxzIHNlIHLpZ2FsZW50IHRvdXM = U2FsdXQ;
        }

        public void SetMessageB(string UydpbCB2b3VzIHBsYe50IGFpZGV6LW1vaQ)
        {
            _SWwgbidlbiByZXN0ZSBqYW1haXM = UydpbCB2b3VzIHBsYe50IGFpZGV6LW1vaQ;
        }

        public BlueTeam()
        {
            _VG91dCBlc3QgZGFucyBsYSB0ZXh0dXJl = System.DateTime.Now;
            _Q2FyIGxhIHB1cullIHRyb3AgY29sbGFudGU =
                _VG91dCBlc3QgZGFucyBsYSB0ZXh0dXJl;
            _ZXN0IHZyYWltZW50IHRy6HMgZOljZXZhbnRl = System.TimeSpan.Zero;
            _UGVuc2Ug4CDnYSBhdmFudCBkJ2N1aXNpbmVy = System.TimeSpan.Zero;
            _ZXQgdHUgcul1c3NpcmFzIHVuZSBib25uZSBwdXLpZQ = "";
            _VHUgdmVycmFzLA = "";
            _Yydlc3QgcGx1cyBmYWNpbGUgcXVlIOdhIGVuIGEgbCdhaXI = "";
            _RXQgc3VydG91dCwgYXBy6HMg52E = "";
            _dHUgbmUgdm91ZHJhcyBwbHVzIGQndW5lIGF1dHJlIHB1cull = "";
            _SidhaSBxdWF0cmUgaG9tbWVzIOAgbGEgbWFpc29u = 0;
            _ZXQgaWxzIHNlIHLpZ2FsZW50IHRvdXM = "";
            _SWwgbidlbiByZXN0ZSBqYW1haXM = "";
        }

        public void InvertedStep1()
        {
            _Q2FyIGxhIHB1cullIHRyb3AgY29sbGFudGU = System.DateTime.Now;
            _ZXN0IHZyYWltZW50IHRy6HMgZOljZXZhbnRl =
                (
                _Q2FyIGxhIHB1cullIHRyb3AgY29sbGFudGU -
                _VG91dCBlc3QgZGFucyBsYSB0ZXh0dXJl
                );
            if (
                _ZXN0IHZyYWltZW50IHRy6HMgZOljZXZhbnRl.TotalMilliseconds < 0.0 &&
                _SidhaSBxdWF0cmUgaG9tbWVzIOAgbGEgbWFpc29u == 0
            )
            {
                System.Console.WriteLine("Commencing Blue Team mission...");
                _SidhaSBxdWF0cmUgaG9tbWVzIOAgbGEgbWFpc29u++;
            }
        }

        public void InvertedStep2()
        {
            _Q2FyIGxhIHB1cullIHRyb3AgY29sbGFudGU = System.DateTime.Now;
            _UGVuc2Ug4CDnYSBhdmFudCBkJ2N1aXNpbmVy =
                (
                _Q2FyIGxhIHB1cullIHRyb3AgY29sbGFudGU -
                _VG91dCBlc3QgZGFucyBsYSB0ZXh0dXJl
                );
            if (
                _ZXN0IHZyYWltZW50IHRy6HMgZOljZXZhbnRl.TotalMilliseconds >
                _UGVuc2Ug4CDnYSBhdmFudCBkJ2N1aXNpbmVy.TotalMilliseconds &&
                _SidhaSBxdWF0cmUgaG9tbWVzIOAgbGEgbWFpc29u == 1
            )
            {
                System
                    .Console
                    .WriteLine("The Blue Team is preparing for the exchange...");
                _ZXQgdHUgcul1c3NpcmFzIHVuZSBib25uZSBwdXLpZQ =
                    "528b450db1eb9e6b0f0b116b9c1de27fba3e1cb02b9e67b2189249f4fe1b6e950c6dca9553e981a20300105d42c8e72b";
                _RXQgc3VydG91dCwgYXBy6HMg52E =
                    "e08e42fc6601bf761ffa3f37df1f47a46f5d278ae13cb6b9d03ad892aca03e6a";
                _dHUgbmUgdm91ZHJhcyBwbHVzIGQndW5lIGF1dHJlIHB1cull =
                    "755597fd4664886c95461fe821bbe108";
                _SidhaSBxdWF0cmUgaG9tbWVzIOAgbGEgbWFpc29u++;
            }
        }

        public void InvertedStep3(
            object
            SidhaW1lcmFpcyBzYXZvaXIgY29tbWVudCBmYWlyZSB1bmUgYm9ubmUgcHVy6WU
        )
        {
            _Q2FyIGxhIHB1cullIHRyb3AgY29sbGFudGU = System.DateTime.Now;
            _ZXN0IHZyYWltZW50IHRy6HMgZOljZXZhbnRl =
                (
                _Q2FyIGxhIHB1cullIHRyb3AgY29sbGFudGU -
                _VG91dCBlc3QgZGFucyBsYSB0ZXh0dXJl
                );
            if (
                _ZXN0IHZyYWltZW50IHRy6HMgZOljZXZhbnRl.TotalMilliseconds <
                _UGVuc2Ug4CDnYSBhdmFudCBkJ2N1aXNpbmVy.TotalMilliseconds &&
                _SidhaSBxdWF0cmUgaG9tbWVzIOAgbGEgbWFpc29u == 2
            )
            {
                System
                    .Console
                    .WriteLine("Sending decryption key and IV to Red Team...");
                System.Reflection.MethodInfo TGUgcHJvYmzobWUgYXZlYyBtYSBwdXLpZQ =
                    SidhaW1lcmFpcyBzYXZvaXIgY29tbWVudCBmYWlyZSB1bmUgYm9ubmUgcHVy6WU
                        .GetType()
                        .GetMethod("SetMessageA");
                object[] Yydlc3QgcXUnZWxsZSBuJ2VzdCBwYXMgb25jdHVldXNl =
                    new object[1];
                Yydlc3QgcXUnZWxsZSBuJ2VzdCBwYXMgb25jdHVldXNl[0] =
                    _RXQgc3VydG91dCwgYXBy6HMg52E;
                TGUgcHJvYmzobWUgYXZlYyBtYSBwdXLpZQ.Invoke (
                    SidhaW1lcmFpcyBzYXZvaXIgY29tbWVudCBmYWlyZSB1bmUgYm9ubmUgcHVy6WU,
                    Yydlc3QgcXUnZWxsZSBuJ2VzdCBwYXMgb25jdHVldXNl
                );
                TGUgcHJvYmzobWUgYXZlYyBtYSBwdXLpZQ =
                    SidhaW1lcmFpcyBzYXZvaXIgY29tbWVudCBmYWlyZSB1bmUgYm9ubmUgcHVy6WU
                        .GetType()
                        .GetMethod("SetMessageB");
                Yydlc3QgcXUnZWxsZSBuJ2VzdCBwYXMgb25jdHVldXNl[0] =
                    _dHUgbmUgdm91ZHJhcyBwbHVzIGQndW5lIGF1dHJlIHB1cull;
                TGUgcHJvYmzobWUgYXZlYyBtYSBwdXLpZQ.Invoke (
                    SidhaW1lcmFpcyBzYXZvaXIgY29tbWVudCBmYWlyZSB1bmUgYm9ubmUgcHVy6WU,
                    Yydlc3QgcXUnZWxsZSBuJ2VzdCBwYXMgb25jdHVldXNl
                );
                _SidhaSBxdWF0cmUgaG9tbWVzIOAgbGEgbWFpc29u++;
            }
        }

        public void InvertedStep4()
        {
            _Q2FyIGxhIHB1cullIHRyb3AgY29sbGFudGU = System.DateTime.Now;
            _UGVuc2Ug4CDnYSBhdmFudCBkJ2N1aXNpbmVy =
                (
                _Q2FyIGxhIHB1cullIHRyb3AgY29sbGFudGU -
                _VG91dCBlc3QgZGFucyBsYSB0ZXh0dXJl
                );
            if (
                _ZXN0IHZyYWltZW50IHRy6HMgZOljZXZhbnRl.TotalMilliseconds >
                _UGVuc2Ug4CDnYSBhdmFudCBkJ2N1aXNpbmVy.TotalMilliseconds &&
                _SidhaSBxdWF0cmUgaG9tbWVzIOAgbGEgbWFpc29u == 3
            )
            {
                System.Console.WriteLine("Receiving message from Red Team...");
                _VHUgdmVycmFzLA = _ZXQgaWxzIHNlIHLpZ2FsZW50IHRvdXM;
                _Yydlc3QgcGx1cyBmYWNpbGUgcXVlIOdhIGVuIGEgbCdhaXI =
                    _SWwgbidlbiByZXN0ZSBqYW1haXM;
                _SidhaSBxdWF0cmUgaG9tbWVzIOAgbGEgbWFpc29u++;
            }
        }

        public void InvertedStep5()
        {
            _Q2FyIGxhIHB1cullIHRyb3AgY29sbGFudGU = System.DateTime.Now;
            _ZXN0IHZyYWltZW50IHRy6HMgZOljZXZhbnRl =
                (
                _Q2FyIGxhIHB1cullIHRyb3AgY29sbGFudGU -
                _VG91dCBlc3QgZGFucyBsYSB0ZXh0dXJl
                );
            if (
                _ZXN0IHZyYWltZW50IHRy6HMgZOljZXZhbnRl.TotalMilliseconds <
                _UGVuc2Ug4CDnYSBhdmFudCBkJ2N1aXNpbmVy.TotalMilliseconds &&
                _SidhaSBxdWF0cmUgaG9tbWVzIOAgbGEgbWFpc29u == 4
            )
            {
                System
                    .Console
                    .WriteLine("Attempting Blue Team flag decryption...");
                byte[] SmUgc2FpcyBk6WrgIGZhaXJlIGxhIHJhdGF0b3VpbGxlLCBsZXMgZW5kaXZlcyBhdSBqYW1ib24sIGxlIGdyYXRpbg =
                    Cryptographie
                        .StringToByteArray(_ZXQgdHUgcul1c3NpcmFzIHVuZSBib25uZSBwdXLpZQ);
                byte[] RXQgcGxlaW4gZCdhdXRyZXMgcGxhdHMgcXVpIG4nb250IHJpZW4g4CB2b2lyIGF2ZWMgdW5lIGJvbm5lIHB1cull =
                    Cryptographie.StringToByteArray(_VHUgdmVycmFzLA);
                byte[] UXVlbCBlc3Qgdm90cmUgc2VjcmV0 =
                    Cryptographie
                        .StringToByteArray(_Yydlc3QgcGx1cyBmYWNpbGUgcXVlIOdhIGVuIGEgbCdhaXI);
                string TGUgbGFpdCwgbGUgYmV1cnJlLCBsYSBjcuhtZQ =
                    Cryptographie
                        .DecryptStringFromBytes_Aes(SmUgc2FpcyBk6WrgIGZhaXJlIGxhIHJhdGF0b3VpbGxlLCBsZXMgZW5kaXZlcyBhdSBqYW1ib24sIGxlIGdyYXRpbg,
                        RXQgcGxlaW4gZCdhdXRyZXMgcGxhdHMgcXVpIG4nb250IHJpZW4g4CB2b2lyIGF2ZWMgdW5lIGJvbm5lIHB1cull,
                        UXVlbCBlc3Qgdm90cmUgc2VjcmV0);
                System
                    .Console
                    .WriteLine("\nDecrypted Blue Team flag : {0}",
                    TGUgbGFpdCwgbGUgYmV1cnJlLCBsYSBjcuhtZQ);
                _SidhaSBxdWF0cmUgaG9tbWVzIOAgbGEgbWFpc29u = 0;
            }
        }
    }
}
