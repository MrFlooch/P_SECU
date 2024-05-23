/// ETML
/// Author: Felix Sierro
/// Date: 19.03.2024
/// Updated: 14.05.2024
/// Description: Application de chiffrement de mot de passe

using System;
using System.Collections.Generic;
using System.IO;

namespace P_SECU
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Encryption encryptLoad = new Encryption();
            encryptLoad.Load();
        }        
    }
}
